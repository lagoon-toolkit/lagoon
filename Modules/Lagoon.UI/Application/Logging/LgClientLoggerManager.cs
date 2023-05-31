using Lagoon.Core.Application.Logging;
using Lagoon.Internal;
using System.Net.Http.Json;

namespace Lagoon.UI.Application.Logging;

/// <summary>
/// Represents a type used to perform logging.
/// </summary>
public class LgClientLoggerManager : IDisposable
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly LgApplication _app;

    /// <summary>
    /// Current LgApplication configuration instance
    /// </summary>
    private readonly LgClientLoggerOptions _options;

    /// <summary>
    /// Timer used to check periodically if there is local error to send to the server
    /// </summary>
    private readonly Timer _timer;

    /// <summary>
    /// Used to synchronize log write / delete
    /// </summary>
    private static readonly SemaphoreSlim _asyncLock = new(1, 1);

    /// <summary>
    /// Number of failed error sync attempts
    /// </summary>
    private int _resendErrorCount = 0;

    /// <summary>
    /// Key used to store errors in local db storage
    /// </summary>
    private static string _localdbErrorsKey;

    #endregion

    #region constructors

    /// <summary>
    /// Initialization of a new logger.
    /// </summary>
    /// <param name="app">Lagoon application manager</param>

    public LgClientLoggerManager(LgApplication app)
    {
        _app = app;
        _options = app.BehaviorConfiguration.Logger;
        // Update 
        app.Configuration.GetSection("App:ClientLogger").Bind(_options);
        _localdbErrorsKey = app.GetLocalStorageKey("ErrorsContainer");
        // Initialize the timer which try to send local errors to the server
        int delay = _options.SyncErrorDelay * 1000;
        if (delay > 0)
        {
            // We wait at least 60 seconds before the first check
            _timer = new Timer(CheckLocalErrorsAsync, null, Math.Min(delay, 60000), delay);
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Send the client-side exception to LogErrorControlleur (server-side)
    /// Unhandled exceptions (and explicity Logger.call) will call this method
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="time">The time of the event.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="stackTrace">The stack trace associated to the message.</param>
    /// <param name="category">The category</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    internal async void LogAsync(LogLevel logLevel, DateTime time, string message, string stackTrace, string category, bool isAppCategory)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            // Error to send
            ClientLogData error = new()
            {
                LogLevel = logLevel,
                Time = time,
                Message = message,
                StackTrace = stackTrace,
                Category = category,
                IsAppCategory = isAppCategory
            };
            // Send the error
#if false //DEBUG
            if (true)
#else
        if (!await SendErrorToServerAsync(error))
#endif
            {
                // If an error occured, save error to localDb
                await _asyncLock.WaitAsync();
                try
                {
                    SaveErrorLocally(error);
                }
                finally
                {
                    _asyncLock.Release();
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"fail: {nameof(LgClientLoggerManager)}.{nameof(LogAsync)}\n{ex.Message}\nsource: {logLevel}: {message}");
        }
    }

    /// <summary>
    /// Send error to the server
    /// </summary>
    /// <param name="error">Error to send</param>
    /// <returns>True if the error successfully send, false otherwise</returns>
    private async Task<bool> SendErrorToServerAsync(ClientLogData error)
    {
        try
        {
            // Send error to the server
            HttpClient httpClient = _app.HttpClientFactory.CreateAuthenticatedClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Routes.LOG_ADD_URI, error);
            // Check status
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Return the list of error stored locally 
    /// </summary>
    /// <returns>List of error to send to the server</returns>
    public static List<ClientLogData> GetLocalErrors(LgApplication app)
    {
        List<ClientLogData> localErrors;
        bool containKey = app.LocalStorage.ContainKey(_localdbErrorsKey);
        // Retrieve an existing list or create a new one
        localErrors = containKey ? app.LocalStorage.GetItem<List<ClientLogData>>(_localdbErrorsKey) : new List<ClientLogData>();
        return localErrors;
    }

    /// <summary>
    /// Clear the error cache
    /// </summary>
    /// <returns></returns>
    internal static void ClearLocalErrors(LgApplication app)
    {
        bool containKey = app.LocalStorage.ContainKey(_localdbErrorsKey);
        // Retrieve an existing list or create a new one
        if (containKey)
        {
            app.LocalStorage.RemoveItem(_localdbErrorsKey);
        }
    }

    /// <summary>
    /// Save an error to localDB
    /// </summary>
    /// <param name="error">Error to save</param>
    private bool SaveErrorLocally(ClientLogData error)
    {
        try
        {
            // Get all local errors
            List<ClientLogData> localErrors = GetLocalErrors(_app);
            ClientLogData lastError = localErrors.LastOrDefault();
            if (lastError != null && lastError.HasTheSameSource(error))
            {
                // If the previous exception is the same, just increment counter without storing the repeated exception
                lastError.Time = error.Time;
                lastError.Count += 1;
            }
            else
            {
                // Add the new error to the list
                localErrors.Add(error);
            }
            // Make sure not to exceed the maximum number of errors to keep on the client side
            if (localErrors.Count > _options.MaxLocalErrorsCount)
            {
                // Remove the older error
                localErrors.RemoveAt(0);
            }
            _app.LocalStorage.SetItem(_localdbErrorsKey, localErrors);
            return true;
        }
        catch (Exception ex)
        {
#if DEBUG
            _app.TraceCriticalException(new Exception("An exception occured while trying to save an error locally", ex));
#else
            Console.WriteLine($"An exception occured while trying to save an error locally : {ex.Message}");
#endif
            return false;
        }
    }

    /// <summary>
    /// Check if there is local error to send to the server, and if so try to send them
    /// </summary>
    private async void CheckLocalErrorsAsync(object state)
    {
        await _asyncLock.WaitAsync();
        try
        {
            List<ClientLogData> localErrors = GetLocalErrors(_app);
            while (localErrors.Count > 0)
            {
                // Send error to the server                    
                HttpClient httpClient = _app.HttpClientFactory.CreateAuthenticatedClient();
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(Routes.LOG_ADD_LIST_URI, localErrors.Take(10));
                // If successfully sended, clear local logs
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //await localStorage.RemoveItemAsync(LOCALDB_ERRORKEY);
                    localErrors.RemoveRange(0, localErrors.Count > 10 ? 10 : localErrors.Count);
                    if (localErrors.Count > 0)
                    {
                        // Update error cache
                        _app.LocalStorage.SetItem(_localdbErrorsKey, localErrors);
                    }
                    else
                    {
                        // Remove error cache
                        _app.LocalStorage.RemoveItem(_localdbErrorsKey);
                    }
                }
                else
                {
                    throw new Exception("CheckLocalErrors successfully post error but get an unexpected response status code.");
                }
            }
            _resendErrorCount = 0;
        }
        catch (Exception ex)
        {
#if DEBUG
            _app.TraceCriticalException(ex);
#else
            Console.WriteLine($"An error occured while trying to resend error ==> {ex.Message}");
#endif
            // Counts the number of attempts to send errors to the servers
            _resendErrorCount++;
            if (_resendErrorCount > _options.MaxResendErrorAttempts)
            {
                _app.NavigateTo("/LgPageLocalErrors");
            }
        }
        finally
        {
            _asyncLock.Release();
        }
    }

#endregion

#region Free resources

    /// <summary>
    /// Freeing resources.
    /// </summary>
    /// <param name="disposing">Free the managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Dispose();
            _asyncLock.Dispose();
        }
    }

    /// <summary>
    /// Freeing resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
