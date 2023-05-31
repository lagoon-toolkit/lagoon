using Lagoon.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace Lagoon.Hubs;


/// <summary>
/// Notification hub
/// </summary>
[AllowAnonymous]
public class BackgroundTaskHub : Hub
{
    /// <summary>
    /// Background task service
    /// </summary>
    internal BackgroundTaskService _bgts;

    /// <summary>
    /// Hub initialization
    /// </summary>
    /// <param name="bgts">Backgroudn task service</param>
    public BackgroundTaskHub(BackgroundTaskService bgts)
    {
        _bgts = bgts;
    }

    /// <summary>
    /// Call cancel running task into server service
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    public void CancelTask(Guid taskId)
    {
        _bgts.CancelTask(taskId);
    }

    /// <summary>
    /// Call update task sate into server service
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    public void UpdateTaskState(Guid taskId)
    {
        _bgts.UpdateTaskStateAsync(Context.ConnectionId, taskId);
    }

    /// <summary>
    /// Free the task result from the memory.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    public void FreeTaskResult(Guid taskId)
    {
        _bgts.FreeTaskResult(taskId);
    }

}