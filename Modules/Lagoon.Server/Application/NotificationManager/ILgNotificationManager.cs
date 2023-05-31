using Lagoon.Model.Models;


namespace Lagoon.Server.Application;

/// <summary>
///  Interface for LgNotificationManager component used to get/set notifications data
/// </summary>
/// <typeparam name="TVmNotification">User notification view model</typeparam>
/// <typeparam name="TNotification">Notification model</typeparam>
public interface ILgNotificationManager<TVmNotification, TNotification> where TVmNotification : NotificationVmBase where TNotification : NotificationBase
{
    /// <summary>
    /// Send a notification to a user
    /// </summary>
    /// <param name="notification">Notification object</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    Task SendNotificationAsync(TNotification notification, Guid userId);

    /// <summary>
    ///  Send a notification to a list of users
    /// </summary>
    /// <param name="notification">Notification</param>
    /// <param name="userIds">User Id list</param>
    /// <returns></returns>
    Task SendNotificationAsync(TNotification notification, IEnumerable<Guid> userIds);

    /// <summary>
    /// Update read indicator for a notification user id
    /// </summary>
    /// <param name="notificationUserId">Notification User Id</param>
    /// <param name="isRead">Notification is read flag</param>
    /// <param name="userId">User Id</param>
    /// <param name="updateDate">Notification update date</param>
    /// <returns></returns>
    Task UpdateNotificationReadStateAsync(Guid notificationUserId, bool isRead, Guid userId, DateTime? updateDate);

    /// <summary>
    /// Update read indicator for a notification user id
    /// </summary>
    /// <param name="notificationUserId">Notification User Id</param>
    /// <param name="isRead">Notification is read flag</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    Task UpdateNotificationReadStateAsync(Guid notificationUserId, bool isRead, Guid userId);

    /// <summary>
    /// Update read indicator for a list of notification user id
    /// </summary>
    /// <param name="notificationUserIds">Notification User Id</param>
    /// <param name="isRead">Notification is read flag</param>
    /// <param name="updateDate">Notification update date</param>
    /// <returns></returns>
    Task UpdateNotificationReadStateAsync(List<Guid> notificationUserIds, bool isRead, DateTime? updateDate);

    /// <summary>
    /// Synchronize notificaton with pending action received
    /// </summary>
    /// <param name="pendingAction">pending action</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    internal Task SyncNotificationsFromPendingActions(NotificationPendingActionVm pendingAction, Guid userId);

    /// <summary>
    /// Delete a notification
    /// </summary>
    /// <param name="notificationUserId">Notificaton User Id</param>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    Task DeleteUserNotificationAsync(Guid notificationUserId, Guid userId);

    /// <summary>
    /// Delete a list of notification user
    /// </summary>
    /// <param name="notificationUserIds">List of notificaton User Id</param>
    /// <returns></returns>
    Task DeleteUserNotificationAsync(List<Guid> notificationUserIds);

    /// <summary>
    /// Retrieve a list of notification for one user
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns></returns>
    Task<List<TVmNotification>> GetNotificationsAsync(Guid userId);

    /// <summary>
    /// Get notifications updated / added after the last notification update date
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="lastNotificationUpdt">last notification update date</param>
    /// <returns></returns>
    Task<List<TVmNotification>> GetNotificationsAsync(Guid userId, DateTime lastNotificationUpdt);
}
