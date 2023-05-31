namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Tab content.
/// </summary>
public partial class LgTabContainerBody : LgComponentBase
{

    #region cascading parameters

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    /// <summary>
    /// Gets or sets tab container
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgCustomTabContainer TabContainer { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        TabContainer.OnRefreshBody += OnRefresh;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        TabContainer.OnRefreshBody -= OnRefresh;
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override bool ShouldRender()
    {
        return !TabContainer.IsLoading;
    }

    /// <summary>
    /// Called when the header need to be render.
    /// </summary>
    private void OnRefresh()
    {
        StateHasChanged();
    }


    /// <summary>
    /// Return the CSS classes for the tab pane.
    /// </summary>
    /// <returns>The CSS classes for the tab pane.</returns>
    private static string GetTabPaneCssClass(LgTabRenderData tab)
    {
        var builder = new LgCssClassBuilder("tab-pane h-100 fade");
        builder.AddIf(tab.IsActive, "show active");
        return builder.ToString();
    }

    #endregion

}
