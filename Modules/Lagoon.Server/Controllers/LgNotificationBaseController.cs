using Lagoon.Internal;
using NotificationBase = Lagoon.Model.Models.NotificationBase;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Notification controller.
/// </summary>
[ApiController]
[Route(Routes.NOTIFICATIONS_ROUTE)]
[Authorize()]
[ApiExplorerSettings(IgnoreApi = true)]
public class LgNotificationBaseController<TNotification, TVmNotification> : LgControllerBase where TVmNotification : NotificationVmBase where TNotification : NotificationBase
{

    /// <summary>
    /// Repository
    /// </summary>
    private ILgNotificationManager<TVmNotification, TNotification> _notificationManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="notificationManager"></param>
    public LgNotificationBaseController(ILgNotificationManager<TVmNotification, TNotification> notificationManager)
    {
        _notificationManager = notificationManager;
    }

    #region Getters

    /// <summary>
    /// Retrieve notifications for current user
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetNotifications")]
    public async Task<ActionResult<List<TVmNotification>>> GetNotificationsAsync()
    {
        try
        {
            Guid userId = ContextUserId;
            return Ok(await _notificationManager.GetNotificationsAsync(userId));
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }

    }
    /// <summary>
    /// Retrieve notifications for current user since item update date 
    /// </summary>
    /// <param name="item">notification user object</param>
    /// <returns></returns>
    [HttpPost("GetNotifications")]
    public async Task<ActionResult<List<TVmNotification>>> GetNotificationsAsync(TVmNotification item)
    {
        try
        {
            Guid userId = ContextUserId;
            return Ok(await _notificationManager.GetNotificationsAsync(userId, item.UpdateDate));
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    #endregion

    #region Setters

    /// <summary>
    /// Set read flag for a notification user
    /// </summary>
    /// <param name="userNotificationId">User notification Id</param>
    /// <param name="isRead">Is read indicator</param>
    /// <returns></returns>
    [HttpGet("UpdateNotificationReadState/{userNotificationId:guid}/{isRead}")]
    public async Task<IActionResult> UpdateNotificationReadStateAsync(Guid userNotificationId, bool isRead)
    {
        try
        {
            Guid userId = ContextUserId;
            await _notificationManager.UpdateNotificationReadStateAsync(userNotificationId, isRead, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Set read flag for a notification user list
    /// </summary>
    /// <param name="userNotificationIds">List of User notification Id</param>
    /// <param name="isRead">Is read indicator</param>
    /// <returns></returns>
    [HttpPost("UpdateNotificationReadState/{isRead}")]
    public async Task<IActionResult> UpdateNotificationReadStateAsync(List<Guid> userNotificationIds, bool isRead)
    {
        try
        {
            await _notificationManager.UpdateNotificationReadStateAsync(userNotificationIds, isRead, null);
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }
    /// <summary>
    /// Send pending actions
    /// </summary>
    /// <param name="pendingActions">List of pending actions</param>
    /// <returns></returns>
    [HttpPost("SendPendingActions")]
    public async Task<IActionResult> SendPendingActionsAsync(List<NotificationPendingActionVm> pendingActions)
    {
        try
        {
            Guid userId = ContextUserId;
            foreach (var pendingAction in pendingActions)
            {
                await _notificationManager.SyncNotificationsFromPendingActions(pendingAction, userId);
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }
    #endregion

    #region Delete

    /// <summary>
    /// Delete one notification
    /// </summary>
    /// <param name="userNotificationId">User notification Id</param>
    /// <returns></returns>
    [HttpDelete("DeleteUserNotification/{userNotificationId:guid}")]
    public async Task<IActionResult> DeleteUserNotificationAsync(Guid userNotificationId)
    {
        try
        {
            Guid userId = ContextUserId;
            await _notificationManager.DeleteUserNotificationAsync(userNotificationId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Delete multiple notification user
    /// </summary>
    /// <param name="userNotificationIds">List of User notification Ids</param>
    /// <returns></returns>
    [HttpPost("DeleteUserNotification")]
    public async Task<IActionResult> DeleteUserNotificationAsync(List<Guid> userNotificationIds)
    {
        try
        {
            await _notificationManager.DeleteUserNotificationAsync(userNotificationIds);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    #endregion
}
