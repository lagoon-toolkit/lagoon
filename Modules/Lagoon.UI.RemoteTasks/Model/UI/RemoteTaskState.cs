namespace Lagoon.UI.Components;


/// <summary>
/// Remote task state with result.
/// </summary>
public class RemoteTaskState<TResult> : RemoteTaskState
{

    #region fields

    private TResult _result;

    /// <summary>
    /// End task callback 
    /// </summary>
    private Func<RemoteTaskState<TResult>, Task> _onEndCallback;

    #endregion

    #region properties

    /// <summary>
    /// Result of the method.
    /// </summary>
    public TResult Result => _result;

    #endregion

    #region constructor

    /// <summary>
    /// Remote task state initialization
    /// </summary>
    /// <param name="crts">Client remoite service</param>
    /// <param name="onEndCallback">End of task callback</param>
    public RemoteTaskState(RemoteTaskService crts, Func<RemoteTaskState<TResult>, Task> onEndCallback)
        : base(crts, OnEndWithResult)
    {
        _onEndCallback = onEndCallback;
    }

    private static Task OnEndWithResult(RemoteTaskState state)
    {
        RemoteTaskState<TResult> instance = (RemoteTaskState<TResult>)state;
        return instance._onEndCallback(instance);
    }

    #endregion

    #region methods

    /// <summary>
    /// Set the result for the remote task.
    /// </summary>
    /// <param name="jsonResult">JSON encoded result.</param>
    protected internal override void SetJsonResult(string jsonResult)
    {
        TaskEndEventArgs<TResult> args = System.Text.Json.JsonSerializer.Deserialize<TaskEndEventArgs<TResult>>(jsonResult);
        SetErrorMessage(args.ErrorMessage);
        _result = args.Result;
    }

    #endregion

}

/// <summary>
/// Remote task state without result.
/// </summary>
public class RemoteTaskState
{
    #region fields

    /// <summary>
    /// Client remote task service
    /// </summary>
    private RemoteTaskService _crts;

    /// <summary>
    /// task error message
    /// </summary>
    private string _errorMessage;

    /// <summary>
    /// Hub connection id
    /// </summary>
    private string _hubConnectionId;

    #endregion

    #region properties

    /// <summary>
    /// Gets error message
    /// </summary>
    public string ErrorMessage => _errorMessage;

    /// <summary>
    /// Indicate if the task is ended with an error.
    /// </summary>
    public bool HasError => _errorMessage is not null;

    /// <summary>
    /// Hub connection id
    /// </summary>
    internal string HubConnectionId => _hubConnectionId;

    /// <summary>
    /// End task callback 
    /// </summary>
    internal Func<RemoteTaskState, Task> OnEndCallback { get; }

    /// <summary>
    /// Gets progress 
    /// </summary>
    public Progress Progress { get; }

    /// <summary>
    /// Gets task id
    /// </summary>
    internal Guid TaskId { get; }

    #endregion

    #region constructor

    /// <summary>
    /// Remote task state initialization
    /// </summary>
    /// <param name="crts">Client remoite service</param>
    /// <param name="onEndCallback">End of task callback</param>
    public RemoteTaskState(RemoteTaskService crts, Func<RemoteTaskState, Task> onEndCallback)
    {
        TaskId = Guid.NewGuid();
        _crts = crts;
        OnEndCallback = onEndCallback;
        Progress = new RemoteTaskProgress();
    }

    #endregion

    #region methods

    /// <summary>
    /// Throw a new "UserException" if an error has occured on the remote process. 
    /// </summary>
    /// <exception cref="UserException"></exception>
    public void ThrowIfErrorOccured()
    {
        if (HasError)
        {
            throw new UserException(ErrorMessage);
        }
    }

    /// <summary>
    /// Cancel task
    /// </summary>
    public void Cancel()
    {
        _crts.CancelTask(TaskId);
    }

    /// <summary>
    /// Set the hub connection id.
    /// </summary>
    /// <param name="hubConnectionId"></param>
    internal void SetHubConnectionId(string hubConnectionId)
    {
        _hubConnectionId = hubConnectionId;
    }

    /// <summary>
    /// Set the result for the remote task.
    /// </summary>
    /// <param name="jsonResult">JSON encoded result.</param>
    protected internal virtual void SetJsonResult(string jsonResult)
    {
        // Deserialize error if the traitment failed
        if(!string.IsNullOrEmpty(jsonResult))
        {
            TaskEndEventArgs args = System.Text.Json.JsonSerializer.Deserialize<TaskEndEventArgs>(jsonResult);
            SetErrorMessage(args.ErrorMessage);
        }
    }

    /// <summary>
    /// Set task error message
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    protected void SetErrorMessage(string errorMessage)
    {
        _errorMessage = errorMessage;
    }

    #endregion

}
