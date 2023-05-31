namespace Lagoon.Server.Services;


internal class BackgroundTask<TResult> : BackgroundTask
{

    #region constructor

    /// <summary>
    /// Background task initialization
    /// </summary>
    /// <param name="bgtService">Background task service</param>
    public BackgroundTask(BackgroundTaskService bgtService) :
        base(bgtService)
    { }

    #endregion

    #region methods

    /// <summary>
    /// Run task
    /// </summary>
    public void Run<TService>(IServiceProvider serviceProvider, Func<TService, CancellationToken, Progress, Task<TResult>> func)
    {
        HandleRunningTask(RunInScopeAsync(serviceProvider, func));
    }

    /// <summary>
    /// Run the task with a parameter.
    /// </summary>
    /// <typeparam name="TIn">The parameter.</typeparam>
    /// <typeparam name="TService">The service.</typeparam>
    /// <param name="serviceProvider">service provider.</param>
    /// <param name="func">Method to run.</param>
    /// <param name="arg">The argument of the method.</param>
    internal void Run<TService, TIn>(IServiceProvider serviceProvider, Func<TService, CancellationToken, Progress, TIn, Task<TResult>> func, TIn arg)
    {
        HandleRunningTask(RunInScopeAsync(serviceProvider, func, arg));
    }

    /// <summary>
    /// Run task in scope
    /// </summary>
    private async Task<TResult> RunInScopeAsync<TService>(IServiceProvider serviceProvider, Func<TService, CancellationToken, Progress, Task<TResult>> func)
    {
        await Task.Yield();
        using (var scope = serviceProvider.CreateScope())
        {
            TService service = scope.ServiceProvider.GetRequiredService<TService>();
            return await func(service, CancellationTokenSource.Token, Progress);
        }
    }


    /// <summary>
    /// Run task in scope
    /// </summary>
    private async Task<TResult> RunInScopeAsync<TService, TIn>(IServiceProvider serviceProvider, Func<TService, CancellationToken, Progress, TIn, Task<TResult>> func, TIn arg)
    {
        await Task.Yield();
        using (var scope = serviceProvider.CreateScope())
        {
            TService service = scope.ServiceProvider.GetRequiredService<TService>();
            return await func(service, CancellationTokenSource.Token, Progress, arg);
        }
    }

    /// <summary>
    /// Get the result of the task.
    /// </summary>
    /// <returns>The result of the task.</returns>
    internal protected override async Task<TaskEndEventArgs> GetBackgroundTaskResultAsync()
    {
        return new TaskEndEventArgs<TResult>(await (Task<TResult>)Task);
    }

    #endregion

}

/// <summary>
/// Background task 
/// </summary>
internal class BackgroundTask
{
    #region fields

    private Task _task;

    #endregion

    #region properties

    /// <summary>
    /// Gets background task service
    /// </summary>
    public BackgroundTaskService BackgroundTaskService { get; }

    /// <summary>
    /// Gets cancellation token source
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Gets background task guid
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Gets hub connection id
    /// </summary>
    internal string HubConnectionId { get; set; }

    /// <summary>
    /// Gets Progress
    /// </summary>
    public Progress Progress { get; }

    /// <summary>
    /// Gets Task
    /// </summary>
    public Task Task => _task;

    #endregion

    #region constructor

    /// <summary>
    /// Background task initialization
    /// </summary>
    /// <param name="bgtService">Background task service</param>
    public BackgroundTask(BackgroundTaskService bgtService)
    {
        BackgroundTaskService = bgtService;
        Progress = new BackgroundTaskProgress(this);
        CancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Background task initialization
    /// </summary>
    /// <param name="taskId">TaskId</param>
    /// <param name="hubConnectionId">Hub connection id</param>
    public void Initialize(Guid taskId, string hubConnectionId)
    {
        Guid = taskId;
        HubConnectionId = hubConnectionId;
    }

    #endregion

    #region methods

    /// <summary>
    /// Run task
    /// </summary>
    public void Run<TService>(IServiceProvider serviceProvider, Func<TService, CancellationToken, Progress, Task> func)
    {
        HandleRunningTask(RunInScopeAsync(serviceProvider, func));
    }

    /// <summary>
    /// Run task in scope
    /// </summary>
    private async Task RunInScopeAsync<TService>(IServiceProvider serviceProvider, Func<TService, CancellationToken, Progress, Task> func)
    {
        await Task.Yield();

        using (var scope = serviceProvider.CreateScope())
        {
            TService service = scope.ServiceProvider.GetRequiredService<TService>();
            await func(service, CancellationTokenSource.Token, Progress);
        }
    }

    /// <summary>
    /// Link the running task and handle the end.
    /// </summary>
    protected void HandleRunningTask(Task task)
    {
        _task = task;
        _task.ContinueWith(OnTaskEnded);
    }

    /// <summary>
    /// End task callback
    /// </summary>
    /// <param name="task">Task</param>
    private void OnTaskEnded(Task task)
    {
        BackgroundTaskService.OnTaskEndedAsync(this);
    }

    /// <summary>
    /// Get the result of the task.
    /// </summary>
    /// <returns>The result of the task.</returns>
    internal protected virtual async Task<TaskEndEventArgs> GetBackgroundTaskResultAsync()
    {
        await _task;
        return null;
    }

    #endregion

}
