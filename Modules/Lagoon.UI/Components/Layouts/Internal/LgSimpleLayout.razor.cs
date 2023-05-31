namespace Lagoon.UI.Components.Layouts.Internal;

/// <summary>
/// Main part of default Lagoon layout.
/// </summary>
public partial class LgSimpleLayout : LgComponentBase, IPageManager
{

    #region cascading parameters

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the route data. Contains the information to display a page.
    /// </summary>
    protected RouteData RouteData { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Event called when the page is closing.
    /// </summary>
    public event Func<PageClosingSourceEvent, Func<Task>, Task<bool>> OnRaiseClosingEventAsync;

    /// <summary>
    /// Event called when the page is closed.
    /// </summary>
    public event Func<Task> OnRaiseCloseEventAsync;

    /// <summary>
    /// Event called when the tab is selected.
    /// </summary>
    event Func<Task> IPageManager.OnRaiseActivatedEventAsync
    {
        add
        {
            //Non applicable in this context
        }
        remove
        {
            //Non applicable in this context
        }
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        RouteData = AppComponent.CurrentRouteData;
        AppComponent.OnNavigatingTo += NavigatingToAsync;
        AppComponent.OnNavigateTo += NavigateToAsync;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        AppComponent.OnNavigateTo -= NavigateToAsync;
        AppComponent.OnNavigatingTo -= NavigatingToAsync;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Check if the current page can't be close before goind to the next page.
    /// </summary>
    /// <param name="arg">Event arguments.</param>
    /// <returns>The running task.</returns>
    private async Task NavigatingToAsync(NavigatingToEventArgs arg)
    {
        // Execute the page OnClosing event
        if (OnRaiseClosingEventAsync is not null)
        {
            arg.Cancel = !await OnRaiseClosingEventAsync.Invoke(PageClosingSourceEvent.PageClose, 
                async () => { App.NavigateTo(arg.Uri); await Task.CompletedTask; });
        }
        // Execute the page OnClose event
        if (!arg.Cancel && OnRaiseCloseEventAsync is not null)
        {
            await OnRaiseCloseEventAsync.Invoke();
        }
    }

    /// <summary>
    /// Load the page from the route data.
    /// </summary>
    /// <param name="e">Informstions about the page to display.</param>
    private async Task NavigateToAsync(NavigateToEventArgs e)
    {
        RouteData = e.RouteData;
        StateHasChanged();
        await Task.CompletedTask;
    }

    #endregion

}
