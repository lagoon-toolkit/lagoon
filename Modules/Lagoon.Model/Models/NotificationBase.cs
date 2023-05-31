namespace Lagoon.Model.Models;


/// <summary>
/// Notification data
/// </summary>
public class NotificationBase
{

    #region properties

    /// <summary>
    /// Gets or sets the notification id.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

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
    public DateTime CreationDate { get; set; } = DateTime.Now;

    #endregion

}
