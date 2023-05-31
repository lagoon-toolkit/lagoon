namespace Lagoon.UI.Components;

/// <summary>
/// Notification service
/// </summary>
public interface INotificationService<TItem> : INotificationService where TItem : NotificationVmBase
{

    /// <summary>
    /// Event raise the list of notification is updated.
    /// </summary>
    event Action<List<TItem>> OnNotificationChange;

    /// <summary>
    /// Retrieve a notifications list from the indexedDb notification store.
    /// </summary>
    /// <returns>List of notification item (viewmodel).</returns>
    Task<List<TItem>> GetNotificationsAsync();

}
