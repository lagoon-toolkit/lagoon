namespace Lagoon.UI.Components;

/// <summary>
/// Notification service
/// </summary>
public interface INotificationService
{

    /// <summary>
    /// Mark as read or unread a notification item.
    /// </summary>
    /// <param name="notificationUserId">A notification identifier.</param>
    /// <param name="isRead">Indicates whether the notification is marked as read.</param>
    Task SetReadStateItemAsync(Guid notificationUserId, bool isRead);

    /// <summary>
    /// Mark as read or unread a list of notification items.
    /// </summary>
    /// <param name="notificationUserIds">A list of notification identifiers.</param>
    /// <param name="isRead">Indicates whether the notification is marked as read.</param>
    Task SetReadStateItemsAsync(IEnumerable<Guid> notificationUserIds, bool isRead);

    /// <summary>
    /// Remove a notification item.
    /// </summary>
    /// <param name="notificationUserId">A notification identifier.</param>
    Task DeleteItemAsync(Guid notificationUserId);


    /// <summary>
    /// Remove a list of notification items.
    /// </summary>
    /// <param name="notificationUserIds">A list of notification identifiers.</param>
    Task DeleteItemsAsync(IEnumerable<Guid> notificationUserIds);

}
