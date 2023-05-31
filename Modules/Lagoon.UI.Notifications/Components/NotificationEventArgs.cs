namespace Lagoon.UI.Components;

/// <summary>
/// Notification item click event args.
/// </summary>
public class NotificationEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Item 
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="item"></param>
    public NotificationEventArgs(TItem item)
    {
        Item = item;
    }

}
