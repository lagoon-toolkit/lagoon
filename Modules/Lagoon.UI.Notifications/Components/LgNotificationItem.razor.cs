namespace Lagoon.UI.Components;

/// <summary>
/// Notification item for the menu
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LgNotificationItem<TItem> : LgComponentBase where TItem : NotificationVmBase
{


    #region fields

    /// <summary>
    /// LgNotificationMenuItem parent component
    /// </summary>
    [CascadingParameter]
    private LgNotificationMenuItem<TItem> LgNotificationMenuItem { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the notification item 
    /// </summary>
    [Parameter]
    public TItem Item { get; set; }

    /// <summary>
    /// On remove item event
    /// </summary>
    [Parameter]
    public EventCallback<NotificationEventArgs<TItem>> OnDelete { get; set; }

    /// <summary>
    /// On click item event
    /// </summary>
    [Parameter]
    public EventCallback<NotificationEventArgs<TItem>> OnClick { get; set; }

    /// <summary>
    /// Notification iconname
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Filter notification list
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    #endregion

    #region dependencies injection

    /// <summary>
    /// Client notification service
    /// </summary>
    [Inject]
    private INotificationService NotificationService { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// On remove notification item
    /// </summary>
    protected Task DeleteItemAsync()
    {
        return NotificationService.DeleteItemAsync(Item.NotificationUserId);
    }

    /// <summary>
    /// On mark as read or unread notification item
    /// </summary>
    protected Task SetReadUnReadItemAsync(bool isRead)
    {
        return NotificationService.SetReadStateItemAsync(Item.NotificationUserId, isRead);
    }

    /// <summary>
    /// On click notification item
    /// </summary>
    protected async Task ClickItemAsync()
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.TryInvokeAsync(App, new NotificationEventArgs<TItem>(Item));
        }
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("lg-notif-itemtpl");
        builder.AddIf(!string.IsNullOrEmpty(CssClass), CssClass);
        builder.AddIf(Item.IsRead, "lg-notif-read", "lg-notif-notread");
    }
    #endregion

}
