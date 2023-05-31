namespace Lagoon.UI.Components;

/// <summary>
/// User notification view model base
/// </summary>
public class NotificationVmBase : IComparable<NotificationVmBase>
{
    #region properties

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    public Guid Id { get; set; } 

    /// <summary>
    /// Gets or sets the notification title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the notification description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the notification date.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the notification update date.
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// Gets or sets the target id.
    /// </summary>
    public Guid? TargetId { get; set; }

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    public Guid NotificationUserId { get; set; }

    /// <summary>
    /// Gets or sets if the notification is read.
    /// </summary>
    public bool IsRead { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Compare notifications
    /// </summary>
    /// <param name="other">An object to compare with this instance</param>
    /// <returns></returns>
    public virtual int CompareTo(NotificationVmBase other)
    {
        if (other is null) return 1;
        return -DateTime.Compare(CreationDate, other.CreationDate);
    }

    #endregion
}
