namespace Lagoon.UI.Components;

/// <summary>
/// Pending action view model
/// </summary>
public class NotificationPendingActionVm
{
    #region properties

    /// <summary>
    /// Gets or sets the pending action.
    /// </summary>
    public NotificationPendingAction Action { get; set; }

    /// <summary>
    /// Gets or sets the notification update date.
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    public Guid NotificationUserId { get; set; }

    /// <summary>
    /// Gets or sets if the notification is read.
    /// </summary>
    public bool IsRead { get; set; }

    #endregion

}

/// <summary>
/// Pending action type
/// </summary>
public enum NotificationPendingAction
{
    /// <summary>
    /// Delete action
    /// </summary>
    Delete,
    /// <summary>
    /// Update action
    /// </summary>
    Update
}
