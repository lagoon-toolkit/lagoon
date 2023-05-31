namespace Lagoon.UI.Components;

/// <summary>
/// Tab container
/// </summary>
public partial class LgTabContainer : LgCustomTabContainer
{

    #region render fragments

    /// <summary>
    /// Content to place at the beginning of the header.
    /// </summary>
    [Parameter]
    public RenderFragment BeforeHeaderContent { get; set; }

    /// <summary>
    /// Content to place at the end of the header.
    /// </summary>
    [Parameter]
    public RenderFragment AfterHeaderContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the tabs can be closed
    /// </summary>
    [Parameter]
    public bool AllowClose { get; set; }

    /// <summary>
    /// Gets or sets if the tabs can be moved
    /// </summary>
    [Parameter]
    public bool AllowDragDrop { get; set; }

    /// <summary>
    /// Show in the toolbar, the dropdown menu of the list of openned tabs.
    /// </summary>
    [Parameter]
    public bool ShowTabList { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Drop tab event
    /// </summary>
    [Parameter]
    public EventCallback<DropTabEventArgs> OnDropTab { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    public LgTabContainer()
    {
        TooltipPosition = TooltipPosition.Bottom;
    }

    #endregion

    #region methods

    /// <summary>
    /// Open tab by its Uri
    /// </summary>
    /// <param name="uri">URI of the page to open</param>
    /// <param name="closable">Indicate if the table is closable.</param>
    public Task OpenTabAsync(string uri, bool closable = true)
    {
        return OpenTabAsync(uri, null, closable);
    }

    /// <summary>
    /// Drop tab management
    /// </summary>
    /// <param name="args"></param>
    private async Task DropTabAsync(DropTabEventArgs args)
    {
        LoadingTabDataList.Move(args.Key, args.DropIndex);
        if (OnDropTab.HasDelegate)
        {
            await OnDropTab.TryInvokeAsync(App, args);
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("tab-container");
        builder.Add(CssClass);
    }

    #endregion
}
