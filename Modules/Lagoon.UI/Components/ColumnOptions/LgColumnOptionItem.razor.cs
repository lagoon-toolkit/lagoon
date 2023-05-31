namespace Lagoon.UI.Components;

/// <summary>
/// Options for a column.
/// </summary>
public partial class LgColumnOptionItem : LgAriaComponentBase
{

    #region cascading parameter

    /// <summary>
    /// Gets or sets column options
    /// </summary>
    [CascadingParameter]
    public LgColumnOptions ColumnOptions { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets column option data
    /// </summary>
    [Parameter]
    public ColumnOption DisplayedColumn { get; set; }

    /// <summary>
    /// Gets or sets if the frozen is active
    /// </summary>
    [Parameter]
    public bool AllowFrozen { get; set; } = true;

    /// <summary>
    /// Gets or sets if the visibility management is active
    /// </summary>
    [Parameter]
    public bool AllowVisibility { get; set; } = true;

    #endregion

}
