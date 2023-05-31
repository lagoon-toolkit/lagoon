namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Tab container for filters.
/// </summary>
public partial class LgFilterTabContainer : LgComponentBase
{

    #region cascading parameters

    /// <summary>
    /// The FilterBox component that contains the tab container.
    /// </summary>
    [CascadingParameter]
    public ILgFilterBox FilterBox { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The FilterEditor component containing the tab container.
    /// </summary>
    public ILgFilterEditor FilterEditor => FilterBox.FilterEditor;

    /// <summary>
    /// Gets or sets the DOM element reference to focus.
    /// </summary>
    protected ElementReference FocusElementRef { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Get or sets the content if there is no tabs.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the content of the rules tab.
    /// </summary>
    [Parameter]
    public RenderFragment TabRules { get; set; }

    /// <summary>
    /// Gets or sets the content of the value selection tab.
    /// </summary>
    [Parameter]
    public RenderFragment TabSelection { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        // Focus active tab
        if(firstRender)
        {
            await JS.FocusAsync(FocusElementRef, "a.nav-link.active");
        }
    }

    /// <summary>
    /// Gets the CSS class for the tab button.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns>The CSS class for the tab button.</returns>
    private string GetTabButtonCssClass(FilterTab tab)
    {
        LgCssClassBuilder builder = new("nav-link");
        builder.AddIf(tab == FilterEditor.SelectedTab, "active");
        return builder.ToString();
    }

    /// <summary>
    /// Gets the CSS class for the tab body.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns>The CSS class for the tab body.</returns>
    private string GetTabBodyCssClass(FilterTab tab)
    {
        LgCssClassBuilder builder = new("filter-tab");
        builder.AddIf(tab != FilterEditor.SelectedTab, "hide");
        return builder.ToString();
    }

    /// <summary>
    /// Gets the label of the tab.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns>The label of the tab.</returns>
    /// <exception cref="InvalidOperationException">The tab is unknown.</exception>
    private static string GetTabButtonTitle(FilterTab tab)
    {
        return tab switch
        {
            FilterTab.Rules => "FilterTabRules".Translate(),
            FilterTab.Selection => "FilterTabSelection".Translate(),
            _ => throw new InvalidOperationException()
        };
    }


    /// <summary>
    /// Get the render content of the tab.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns>The render fragment.</returns>
    /// <exception cref="InvalidOperationException">If the tab is unknown.</exception>
    private RenderFragment GetTabContent(FilterTab tab)
    {
        return tab switch
        {
            FilterTab.Rules => TabRules,
            FilterTab.Selection => TabSelection,
            _ => throw new InvalidOperationException()
        };
    }

    /// <summary>
    /// Show the body of the selected tab.
    /// </summary>
    /// <param name="tab">The tab to show.</param>
    private void ShowTab(FilterTab tab)
    {
        FilterEditor.SelectedTab = tab;
    }

    /// <summary>
    /// Display tab following key pressed
    /// </summary>
    /// <param name="args"></param>
    /// <param name="tab"></param>
    private void OnKeyUp(KeyboardEventArgs args, FilterTab tab)
    {
        if(args.Code == "Space")
        {
            ShowTab(tab);
        }
    }

    #endregion

}
