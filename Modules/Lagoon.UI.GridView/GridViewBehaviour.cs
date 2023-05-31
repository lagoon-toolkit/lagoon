namespace Lagoon.UI.Application;


/// <summary>
/// Global GridView options
/// </summary>
public class GridViewOptions
{

    /// <summary>
    /// Gets or sets the value indicating the add line is shown on top or not
    /// </summary>
    public bool AddItemOnTop { get; set; }

    /// <summary>
    /// Default number of the row by page when the paging feature is activated.
    /// </summary>
    /// <remarks>If 0 or less, the minimum value of <see cref="PaginationSizeSelector" /> is used.</remarks>
    public int DefaultPageSize { get; set; }

    /// <summary>
    /// Force the display of tooltips on the Gridviews headers.
    /// </summary>
    public bool DisplayDefaultTooltipHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets if the input page selector is shown
    /// </summary>
    public bool DisplayInputPageSelector { get; set; } = true;

    /// <summary>
    /// Gets or sets if the success message is shown when cell is edited
    /// </summary>
    public bool DisplaySuccessSaveMessage { get; set; } = true;

    /// <summary>
    /// Get or set the value indicating if GridView are exportable
    /// </summary>
    public bool Exportable { get; set; }

    /// <summary>
    /// Get or set the value indicating if the GridView should export data with only displayed columns
    /// </summary>
    public bool ExportDisplayedColumns { get; set; }

    /// <summary>
    /// Get or set the value indicating if the GridView should export data with filter by default
    /// </summary>
    public bool ExportWithFilters { get; set; }

    /// <summary>
    /// Gets or sets the value indicating if the GridView pager is shown at the top of the grid
    /// </summary>
    public bool PagerOnTop { get; set; }

    /// <summary>
    /// Default row number selector
    /// </summary>
    /// <remarks>0 define All value</remarks>
    public int[] PaginationSizeSelector { get; set; } = new int[] { 5, 10, 25, 50, 100 };

    /// <summary>
    /// Gets or sets the storage profiles system in the grids.
    /// </summary>
    public GridViewProfileStorage ProfileStorageMode { get; set; } = GridViewProfileStorage.Local;

    /// <summary>
    /// Gets or sets the name of policy authorizing the management of shared profiles in the grids.
    /// </summary>
    public string SharedProfileAdministratorPolicy { get; set; }

    /// <summary>
    /// Show summary filters or not (default value)
    /// </summary>
    public bool ShowSummaryFilters { get; set; }

    /// <summary>
    /// Default tooltip position
    /// </summary>
    /// <remarks>Do not set to <c>TooltipPosition.Top</c> or <c>TooltipPosition.Botton</c></remarks>
    public TooltipPosition TooltipPosition { get; set; } = TooltipPosition.Left;

    /// <summary>
    /// Gets or sets profile save mode
    /// </summary>
    public GridViewProfileSave ProfileSaveMode { get; set; } = GridViewProfileSave.Auto;

    /// <summary>
    /// Export configuration options
    /// </summary>
    public ExportConfiguration ExportConfiguration { get; set; } = new ExportConfiguration();

    /// <summary>
    /// Gets or sets the inputs configuration for reset button
    /// </summary>
    public LgInputConfiguration ResetInputConfiguration { get; set; } = new LgInputConfiguration();
}

/// <summary>
/// Add GridView options extension to <see cref="ApplicationBehavior"/>
/// </summary>
public static class GridViewBehaviour
{

    /// <summary>
    /// Intialize the default GridView options
    /// </summary>
    internal static GridViewOptions Options = new();

    /// <summary>
    /// Allow GridView options modification on <see cref="ApplicationBehavior"/>
    /// </summary>
    /// <param name="config">Extension method for <see cref="ApplicationBehavior"/></param>
    /// <param name="configure">Action to customize settings</param>
    /// <returns>The current <see cref="ApplicationBehavior"/> to chain call</returns>
    public static ApplicationBehavior GridView(this ApplicationBehavior config, Action<GridViewOptions> configure)
    {
        configure(Options);
        return config;
    }

}
