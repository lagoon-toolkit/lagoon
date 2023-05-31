namespace Lagoon.UI.Components;

/// <summary>
/// Notification menu 
/// </summary>
public partial class LgNotificationMenuItem<TItem> : LgComponentBase, IDisposable where TItem : NotificationVmBase
{

    #region fields

    // List of items
    private List<TItem> _items;

    #endregion

    #region dependencies injection

    /// <summary>
    /// Client notification service
    /// </summary>
    [Inject]
    private INotificationService<TItem> ClientNotificationService { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Get or seets the content item template
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> NotificationTemplate { get; set; }

    /// <summary>
    /// Get or sets the bottom menu content.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the notification icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; } = IconNames.All.BellFill;

    /// <summary>
    /// Gets or sets the notification menu text
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// OnClick item event
    /// </summary>
    [Parameter]
    public EventCallback<NotificationEventArgs<TItem>> OnNotificationClick { get; set; }

    /// <summary>
    /// Filter notification list
    /// </summary>
    [Parameter]
    public Func<TItem, bool> FilterPredicate { get; set; }

    /// <summary>
    /// Filter notification list
    /// </summary>
    [Parameter]
    public Func<TItem, string> ItemIconName { get; set; }

    /// <summary>
    /// Filter notification list
    /// </summary>
    [Parameter]
    public Func<TItem, string> ItemCssClass { get; set; }

    #endregion

    #region init

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ClientNotificationService.OnNotificationChange += OnNotificationChange;

    }

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            await JS.ScriptIncludeAsync("_content/Lagoon.UI.Notifications/js/main.min.js");
            LoadItems(await ClientNotificationService.GetNotificationsAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            ShowWarning("#NotificationSynchError");
        }
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            try
            {
                await ((ClientNotificationService<TItem>)ClientNotificationService).InitializeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        ClientNotificationService.OnNotificationChange -= OnNotificationChange;
        base.Dispose(disposing);
    }

    #endregion

    #region methods

    /// <summary>
    /// Refresh notification list
    /// </summary>
    /// <param name="notifications">List of notification</param>
    private void OnNotificationChange(List<TItem> notifications)
    {
        LoadItems(notifications);
        StateHasChanged();
    }

    /// <summary>
    /// Get all data from the local storage.
    /// </summary>
    /// <returns>All data from the controller.</returns>
    public void LoadItems(List<TItem> notifications)
    {
        // Init notification list filtered
        _items = notifications;
        if (FilterPredicate is not null)
        {
            _items = _items.Where(FilterPredicate).ToList();
        }
        _items.Sort();
    }

    /// <summary>
    /// Gets the CSS class to apply to the notification.
    /// </summary>
    /// <returns>The CSS class.</returns>
    private string GetItemCssClass(TItem item)
    {
        return ItemCssClass?.Invoke(item);
    }

    /// <summary>
    /// Gets the iconname to apply to the notification.
    /// </summary>
    /// <returns>The CSS class.</returns>
    private string GetItemIconName(TItem item)
    {
        return ItemIconName?.Invoke(item);
    }
    #endregion

    #region events

    /// <summary>
    /// On click notification item
    /// </summary>
    /// <param name="item">item clicked</param>
    internal async Task ClickItemAsync(TItem item)
    {
        if (OnNotificationClick.HasDelegate)
        {
            await OnNotificationClick.TryInvokeAsync(App, new NotificationEventArgs<TItem>(item));
        }
    }

    /// <summary>
    /// Get the unread notifications count.
    /// </summary>
    /// <returns></returns>
    private string GetTag()
    {
        int unreadCount = 0;
        if (_items is not null)
        {
            foreach (NotificationVmBase notification in _items)
            {
                if (!notification.IsRead)
                {
                    unreadCount++;
                }
            }
        }
        if (unreadCount > 0)
        {
            return unreadCount.ToString();
        }
        else
        {
            return null;
        }
    }

    #endregion
}
