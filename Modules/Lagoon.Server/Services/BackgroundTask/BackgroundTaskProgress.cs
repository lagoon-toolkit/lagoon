namespace Lagoon.Server.Services;

/// <summary>
/// Remote progress
/// </summary>
internal class BackgroundTaskProgress : Progress
{
    #region fields

    private BackgroundTask _backgroundTask;

    private string _lastMessage;

    private int _lastPercent;

    #endregion

    #region properties

    /// <summary>
    /// Gets hub connection id
    /// </summary>
    public string HubConnectionId => _backgroundTask.HubConnectionId;


    /// <summary>
    /// Gets task id
    /// </summary>
    public Guid TaskId => _backgroundTask.Guid;

    #endregion

    #region constructor

    /// <summary>
    /// Remote progress  initialization
    /// </summary>
    /// <param name="bgt">Background task</param>
    public BackgroundTaskProgress(BackgroundTask bgt)
    {
        _backgroundTask = bgt;
    }

    #endregion

    #region methods

#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void OnResettingAsync()
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await _backgroundTask.BackgroundTaskService.OnResetProgressAsync(this);
        }
        catch (Exception ex)
        {
            _backgroundTask.BackgroundTaskService.App.TraceException(ex);
        }
    }

    protected override void OnResetting()
    {
        base.OnResetting();
        if (_backgroundTask is not null)
        {
            OnResettingAsync();
        }
    }

#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void ReportAsync(int position, string message, bool updatePosition, bool updateMessage)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            int percent = position * 100 / EndPosition;

            if((updatePosition && percent != _lastPercent) || (updateMessage && message != _lastMessage))
            {
                _lastPercent = percent;
                _lastMessage = message;

                await _backgroundTask.BackgroundTaskService.SendProgressionAsync(HubConnectionId, _backgroundTask.Guid, position, message, updatePosition, updateMessage);
            }
        }
        catch (Exception ex)
        {
            _backgroundTask.BackgroundTaskService.App.TraceException(ex);
        }
    }

    protected override void Report(int position, string message, bool updatePosition, bool updateMessage, bool autoEnd = true)
    {
        ReportAsync(position, message, updatePosition, updateMessage);
        base.Report(position, message, updatePosition, updateMessage, autoEnd);
    }

    #endregion

}
