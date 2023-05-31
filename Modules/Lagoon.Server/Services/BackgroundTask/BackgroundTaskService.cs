using Lagoon.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Lagoon.Server.Services;

/// <summary>
/// Background Task Service
/// </summary>
public class BackgroundTaskService
{
    #region fields

    private Dictionary<Guid, BackgroundTask> _tasksByGuid = new();
    private Dictionary<Guid, TaskEndEventArgs> _taskResults = new();

    #endregion

    #region dependencies

    /// <summary>
    /// Hub (signalr)
    /// </summary>
    private IHubContext<BackgroundTaskHub> _hub;

    /// <summary>
    /// The applicaton.
    /// </summary>
    private ILgApplication _app;

    /// <summary>
    /// Service Provider
    /// </summary>
    private IServiceProvider _serviceProvider;

    #endregion

    #region properties

    /// <summary>
    /// The application.
    /// </summary>
    internal ILgApplication App => _app;

    #endregion

    #region constructor

    /// <summary>
    /// Contructor
    /// </summary>
    /// <param name="hub">Background task hub</param>
    /// <param name="app">LgApplication</param>
    /// <param name="serviceProvider">Service Provider</param>
    public BackgroundTaskService(IHubContext<BackgroundTaskHub> hub, ILgApplication app, IServiceProvider serviceProvider)
    {
        _hub = hub;
        _app = app;
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region methods

    /// <summary>
    /// Run task with progress
    /// </summary>
    /// <param name="controller">Calling Controller</param>
    /// <param name="func">Task to execute.</param>
    /// <param name="arg">Argument to add to the method.</param>
    public void RunTask<TService, TIn, TResult>(ControllerBase controller, Func<TService, CancellationToken, Progress, TIn, Task<TResult>> func, TIn arg)
    {
        // Create a new background task and add it to the collection
        BackgroundTask<TResult> backgroundTask = new(this);
        RegisterBackgroundTask(controller, backgroundTask);
        // Run the task
        backgroundTask.Run(_serviceProvider, func, arg);
    }

    /// <summary>
    /// Run task with progress
    /// </summary>
    /// <param name="controller">Calling Controller</param>
    /// <param name="func">Task to execute.</param>
    public void RunTask<TService, TResult>(ControllerBase controller, Func<TService, CancellationToken, Progress, Task<TResult>> func)
    {
        // Create a new background task and add it to the collection
        BackgroundTask<TResult> backgroundTask = new(this);
        RegisterBackgroundTask(controller, backgroundTask);
        // Run the task
        backgroundTask.Run(_serviceProvider, func);
    }

    /// <summary>
    /// Run task with progress
    /// </summary>
    /// <param name="controller">Calling Controller</param>
    /// <param name="func">Task to execute.</param>
    public void RunTask<TService>(ControllerBase controller, Func<TService, CancellationToken, Progress, Task> func)
    {
        // Create a new background task and add it to the collection
        BackgroundTask backgroundTask = new(this);
        RegisterBackgroundTask(controller, backgroundTask);
        // Run the task
        backgroundTask.Run(_serviceProvider, func);
    }

    /// <summary>
    /// reate a new background task and add it to the collection
    /// </summary>
    /// <param name="controller">The calling controller.</param>
    /// <param name="backgroundTask">The background task.</param>
    private void RegisterBackgroundTask(ControllerBase controller, BackgroundTask backgroundTask)
    {
        Guid taskId = Guid.Parse(controller.Request.Query["TaskId"]);
        string hubConnectionId = controller.Request.Query["HubCoId"];
        backgroundTask.Initialize(taskId, hubConnectionId);
        _tasksByGuid.Add(backgroundTask.Guid, backgroundTask);
    }

    /// <summary>
    /// On progress event 
    /// </summary>
    /// <param name="progress">Progress</param>
    internal Task OnResetProgressAsync(BackgroundTaskProgress progress)
    {
        return _hub.Clients.Client(progress.HubConnectionId).SendAsync("OnResetProgress", progress.TaskId, progress.StartPosition, progress.EndPosition, progress.Message);
    }

    /// <summary>
    /// Send progression with signalR 
    /// </summary>
    /// <param name="hubConnectionId">Hub connection id</param>
    /// <param name="taskId">TaskId</param>
    /// <param name="position">Progress position</param>
    /// <param name="message">Progress message</param>
    /// <param name="updatePosition">Progress must update position</param>
    /// <param name="updateMessage">Progress must update message</param>
    /// <returns></returns>
    internal Task SendProgressionAsync(string hubConnectionId, Guid taskId, int position, string message, bool updatePosition, bool updateMessage)
    {
        // Send with SignalR
        return _hub.Clients.Client(hubConnectionId).SendAsync("OnReportProgress", taskId, position, message, updatePosition, updateMessage);

    }

    /// <summary>
    /// On ending task
    /// </summary>
    /// <param name="backgroundTask">Background task</param>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    internal async void OnTaskEndedAsync(BackgroundTask backgroundTask)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            Guid taskId = backgroundTask.Guid;
            TaskEndEventArgs taskResult;
            try
            {
                _tasksByGuid.Remove(taskId);
                // Get the method result and handle the potential task exception
                taskResult = await backgroundTask.GetBackgroundTaskResultAsync();
            }
            catch (Exception ex)
            {
                _app.TraceException(ex);
                // Save the error as task result
                taskResult = new(_app.GetContactAdminMessage(ex, true));
            }
            // Keep the result until the client ask the deletion
            _taskResults.Add(taskId, taskResult);
            // Alert the client that the task is done
            await _hub.Clients.Client(backgroundTask.HubConnectionId).SendAsync("OnTaskEnd", taskId, taskResult?.GetJson());
        }
        catch (Exception ex)
        {
            _app.TraceException(ex);
        }
    }

    /// <summary>
    /// Cancel runing task
    /// </summary>
    /// <param name="taskId">taskId</param>
    internal void CancelTask(Guid taskId)
    {
        if (_tasksByGuid.TryGetValue(taskId, out BackgroundTask backgroundTask))
        {
            backgroundTask.CancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// Free the task result from the memory.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    internal void FreeTaskResult(Guid taskId)
    {
        _taskResults.Remove(taskId);
    }

    /// <summary>
    /// Update a task state after hub reconnection
    /// </summary>
    /// <param name="hubConnectionId">Hub connection id</param>
    /// <param name="taskId">Task Id</param>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    internal async void UpdateTaskStateAsync(string hubConnectionId, Guid taskId)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            // Try to find if the task is always running
            if (_tasksByGuid.TryGetValue(taskId, out BackgroundTask bgt))
            {
                // Reconnect the background task to the right pipe
                bgt.HubConnectionId = hubConnectionId;
            }
            else
            {
                // Try to find the task result
                if (!_taskResults.TryGetValue(taskId, out TaskEndEventArgs taskResult))
                {
                    // The task can't be found in running tasks or in tasks done
                    taskResult = new TaskEndEventArgs("The remote task can't be found.");
                }
                // Send the result to the client
                await _hub.Clients.Client(hubConnectionId).SendAsync("OnTaskEnd", taskId, taskResult?.GetJson());
            }
        }
        catch (Exception ex)
        {
            _app.TraceException(ex);
        }
    }

    #endregion
}
