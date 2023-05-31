using System.ComponentModel.DataAnnotations.Schema;

namespace Lagoon.Model.Models;


/// <summary>
/// The notification data base class.
/// </summary>    
public class NotificationUserWithUserBase<TNotification, TUser, TKey>
    where TUser : ILgIdentityUser
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    [ForeignKey(nameof(User))]
    public TKey UserId { get; set; }

    /// <summary>
    /// The user linked to the notification.
    /// </summary>
    public virtual TUser User { get; set; }

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    [ForeignKey(nameof(Notification))]
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Notification object
    /// </summary>
    public virtual TNotification Notification { get; set; }

    /// <summary>
    /// Gets or sets if the notification is read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the notification update date.
    /// </summary>
    public DateTime UpdateDate { get; set; }

}
