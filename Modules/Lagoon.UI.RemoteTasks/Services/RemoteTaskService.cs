using Lagoon.UI.Application;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.WebSockets;

namespace Lagoon.UI.Components;

/// <summary>
/// Remote tasks service
/// </summary>
public class RemoteTaskService
{

    #region dependencies injections

    // Access token for hub connection
    private IAccessTokenProvider _accessTokenProvider;
    // Http client
    private IHttpClientFactory _httpClientFactory;
    // Hub connection to signalr
    private HubConnection _hubConnection;
    // Navigation manager
    private NavigationManager _navigationManager;

    // Authentication state
    private AuthenticationStateProvider _authenticationStateProvider;

    #endregion

    #region constants

    /// <summary>
    /// Gets or sets the hub uri
    /// </summary>
    private const string HUB_URI = "hubs/remoteTaskHub";

    #endregion

    #region fields

    /// <summary>
    /// The current application.
    /// </summary>
    private LgApplication _app;

    /// <summary>
    /// Dictionnary with all running task 
    /// </summary>
    private Dictionary<Guid, RemoteTaskState> _remoteTaskStateByGuid = new();

    private static object _lock = new();

    private HubConnectionState _hubState;

    #endregion

    #region constructor

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="app">The current application.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="navigationManager">The navigation manager.</param>
    /// <param name="authenticationStateProvider">The access token provider used for hub connection.</param>
    /// <param name="accessTokenProvider">The token provider.</param>
    public RemoteTaskService(LgApplication app, IHttpClientFactory httpClientFactory, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, IAccessTokenProvider accessTokenProvider)
    {
        _app = app;
        _httpClientFactory = httpClientFactory;
        _navigationManager = navigationManager;
        _authenticationStateProvider = authenticationStateProvider;
        _accessTokenProvider = accessTokenProvider;
    }

    #endregion

    #region hub management

    private Task<string> _initializeTask;

    /// <summary>
    /// Service initialization
    /// </summary>
    /// <returns></returns>
    internal Task<string> InitializeAsync()
    {
        if (_initializeTask is null)
        {
            lock (_lock)
            {
                _initializeTask ??= InitHubConnectAsync();
            }
        }
        return _initializeTask;
    }

    /// <summary>
    /// Init hub connection
    /// </summary>
    private async Task<string> InitHubConnectAsync()
    {
        _hubConnection = new HubConnectionBuilder()
         .WithUrl(_navigationManager.ToAbsoluteUri(HUB_URI), options =>
         {
             options.AccessTokenProvider = async () =>
             {
                 AccessTokenResult accessTokenResult = await _accessTokenProvider.RequestAccessToken();
                 accessTokenResult.TryGetToken(out AccessToken accessToken);
                 return accessToken?.Value;
             };
         })
         .WithAutomaticReconnect(new CustomRetryPolicy(this))
         .AddMessagePackProtocol()
         .Build();

        //Refresh Progress 
        _hubConnection.On<Guid, int, int, string>("OnResetProgress", (taskId, startPos, endPos, message) =>
        {
            try
            {
                _remoteTaskStateByGuid[taskId].Progress.Reset(startPos, endPos, message);
            }
            catch (WebSocketException)
            {
                // Don't trace WebSocket exception
            }
            catch (Exception ex)
            {
                _app.TraceException(ex);
            }
        });
        _hubConnection.On<Guid, int, string, bool, bool>("OnReportProgress", (taskId, position, message, updatePosition, updateMessage) =>
        {
            try
            {
                ((RemoteTaskProgress)_remoteTaskStateByGuid[taskId].Progress).RemoteReport(position, message, updatePosition, updateMessage);
            }
            catch (WebSocketException)
            {
                // Don't trace WebSocket exception
            }
            catch (Exception ex)
            {
                _app.TraceException(ex);
            }
        });
        // When the remote task is over
        _hubConnection.On("OnTaskEnd", async (Guid taskId, string jsonResult) =>
        {
            try
            {
                await OnTaskEndAsync(taskId, jsonResult);
            }
            catch (WebSocketException)
            {
                // Don't trace WebSocket exception
            }
            catch (Exception ex)
            {
                _app.TraceException(ex);
            }
        });
        // On reconnected to hub
        _hubConnection.Reconnected += async connectionId =>
        {
            _hubState = _hubConnection.State;
            // Foreach task into client service dictonary
            foreach (RemoteTaskState remoteTaskState in _remoteTaskStateByGuid.Values)
            {
                await _hubConnection.InvokeAsync("UpdateTaskState", remoteTaskState.TaskId);
            }
        };
        // On close connection to hub
        _hubConnection.Closed += ex =>
        {
            _hubState = _hubConnection.State;
            _initializeTask = null;
            _hubConnection = null;
            if (ex is not null && (ex is not WebSocketException))
            {
                _app.TraceException(ex);
            }
            return Task.CompletedTask;
        };
        // On close reconnecting to hub
        _hubConnection.Reconnecting += ex =>
        {
            _hubState = _hubConnection.State;
            if (ex is not null && (ex is not WebSocketException))
            {
                _app.TraceException(ex);
            }
            return Task.CompletedTask;
        };
        // Start hub connection
        await _hubConnection.StartAsync();
        return _hubConnection.ConnectionId;
    }

#region CustomRetryPolicy hub reconnection

    internal class CustomRetryPolicy : IRetryPolicy
    {

        private RemoteTaskService _crts;
        public CustomRetryPolicy(RemoteTaskService crts)
        {
            _crts = crts;
        }
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            if (_crts._hubState == HubConnectionState.Connected)
            {
                // The HubState can be connected and we must not return null otherwise the connexion will be closed
                // (and will never be re-openned)
                return TimeSpan.FromSeconds(0);
            }
            else if (retryContext.ElapsedTime.TotalSeconds <= 120) 
            {
                // Wait before next retry
                return TimeSpan.FromSeconds(2);
            }
            else
            {
                // If we've been reconnecting for more than 120 seconds so far, stop reconnecting.
                return null;
            }
        }
    }

#endregion

#endregion

#region methods

    /// <summary>
    /// End task callback
    /// </summary>
    /// <param name="taskId">Task Id.</param>
    /// <param name="jsonResult">Result of the async remote task.</param>
    private async Task OnTaskEndAsync(Guid taskId, string jsonResult)
    {
        if (_remoteTaskStateByGuid.TryGetValue(taskId, out RemoteTaskState remoteTaskState))
        {
            _remoteTaskStateByGuid.Remove(taskId);
            remoteTaskState.SetJsonResult(jsonResult);
            // Ask to the server to release the memory
            await _hubConnection.InvokeAsync("FreeTaskResult", taskId);
            // We complete the progression if it's not done by the developper
            if (!remoteTaskState.Progress.IsEnded && !remoteTaskState.HasError)
            {
                remoteTaskState.Progress.ReportEnd();
            }
            if (remoteTaskState.OnEndCallback is not null)
            {
                await remoteTaskState.OnEndCallback(remoteTaskState);
            }
        }
    }

    /// <summary>
    /// Post TIn data to request URI, end response before the end of the task
    ///     - return the RemoteTaskState with progress indicator
    /// </summary>
    /// <typeparam name="TArg">Object type</typeparam>
    /// <param name="requestUri">Uri to post data to</param>
    /// <param name="model">Object to post</param>
    /// <returns>The remote task state.</returns>
    public Task<RemoteTaskState> TryPostAsync<TArg>(string requestUri, TArg model)
    {
        return TryPostAsync(requestUri, model, null);
    }

    /// <summary>
    /// Post TIn data to request URI, end response before the end of the task
    ///     - return the RemoteTaskState with progress indicator
    /// </summary>
    /// <typeparam name="TArg">Object type</typeparam>
    /// <param name="requestUri">Uri to post data to</param>
    /// <param name="model">Object to post</param>
    /// <param name="onEnd">Callback end of the task</param>
    /// <returns>The remote task state.</returns>
    public Task<RemoteTaskState> TryPostAsync<TArg>(string requestUri, TArg model, Func<RemoteTaskState, Task> onEnd)
    {
        return TryPostWithStateAsync(requestUri, model, new RemoteTaskState(this, onEnd));
    }

    /// <summary>
    /// Post TIn data to request URI, end response before the end of the task
    ///     - return the RemoteTaskState with progress indicator
    /// </summary>
    /// <typeparam name="TArg">Type of the object to post.</typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="requestUri">Uri to post data to</param>
    /// <param name="model">Object to post</param>
    /// <param name="onEnd">Callback end of the task</param>
    /// <returns>The remote task state.</returns>
    public async Task<RemoteTaskState<TResult>> TryPostAsync<TArg, TResult>(string requestUri, TArg model, Func<RemoteTaskState<TResult>, Task> onEnd)
    {
        RemoteTaskState<TResult> state = new(this, onEnd);
        _ = await TryPostWithStateAsync(requestUri, model, state);
        return state;
    }

    /// <summary>
    /// Post TIn data to request URI, end response before the end of the task
    ///     - return the RemoteTaskState with progress indicator
    /// </summary>
    /// <typeparam name="TArg">Type of the object to post.</typeparam>
    /// <param name="requestUri">Uri to post data to</param>
    /// <param name="model">Object to post</param>
    /// <param name="state">The remote task state.</param>
    /// <returns>The remote task state.</returns>
    private async Task<RemoteTaskState> TryPostWithStateAsync<TArg>(string requestUri, TArg model, RemoteTaskState state)
    {
        // Hub connection
        string hubConnectionId = await InitializeAsync();
        state.SetHubConnectionId(hubConnectionId);
        // Get HTTP connection
        bool isAuthenticated = (await _authenticationStateProvider?.GetAuthenticationStateAsync())?.User?.Identity?.IsAuthenticated ?? false;
        HttpClient httpClient = isAuthenticated ? _httpClientFactory.CreateAuthenticatedClient() : _httpClientFactory.CreateAnonymousClient();
        _remoteTaskStateByGuid.Add(state.TaskId, state);
        try
        {
            string uri = $"{requestUri}{(requestUri.Contains('?') ? '&' : '?')}";
            await httpClient.TryPostAsync($"{uri}TaskId={state.TaskId}&HubCoId={hubConnectionId}", model);
        }
        catch (Exception)
        {
            // Don't track the state if the TryPost failed.
            _remoteTaskStateByGuid.Remove(state.TaskId);
            throw;
        }
        return state;
    }

    /// <summary>
    /// Cancel running task
    /// </summary>
    /// <param name="taskId">Task id</param>
    internal void CancelTask(Guid taskId)
    {
        _hubConnection.InvokeAsync("CancelTask", taskId);
    }

#endregion

}
