namespace Lagoon.UI.Components;

/// <summary>
/// Css unit 
/// </summary>
public enum GridCssSizeUnit
{
    /// <summary>
    /// One fraction of the unused space.
    /// </summary>
    Auto,
    /// <summary>
    /// Fraction of the unused space.
    /// </summary>
    Fr,
    /// <summary>
    /// A pixel size.
    /// </summary>
    Px,
    /// <summary>
    /// A percent size. 
    /// </summary>
    Percent
}

/// <summary>
/// Editing mode
/// </summary>
public enum GridEditMode
{
    /// Disable edit
    None,
    /// Edit managed on cell
    Cell,
    /// Edit managed on row
    Row
}

/// <summary>
/// Layout of grid columns.
/// </summary>
public enum GridLayoutMode
{
    /// <summary>
    /// No column width ajustment.
    /// </summary>
    None,
    /// <summary>
    /// Resize the grid columns to fit the header held in each column.
    /// </summary>
    FitHeader,
    /// <summary>
    /// Resize the grid columns to fit the data held in each column.
    /// </summary>
    FitData,
    /// <summary>
    /// Resize the grid columns so they fit in the available table width.
    /// </summary>
    FitColumns
}

/// <summary>
/// Allowed gridview actions
/// </summary>
[Flags]
public enum GridFeature
{
    /// <summary>
    /// No actions
    /// </summary>
    None = 0,
    /// <summary>
    /// Allow column filtering
    /// </summary>
    Filter = 1,
    /// <summary>
    /// Allow column sorting
    /// </summary>
    Sort = 2,
    /// <summary>
    /// Allow paging
    /// </summary>
    Paging = 4,
    /// <summary>
    /// Allow count the number of items corresponding to the filter.
    /// </summary>
    Count = 8,
    /// <summary>
    /// Allow profile
    /// </summary>
    Profile = 16,
    /// <summary>
    /// Allow column resizing
    /// </summary>
    Resize = 32,
    /// <summary>
    /// Allow column moving
    /// </summary>
    Move = 64,
    /// <summary>
    /// Allow column moving
    /// </summary>
    Freeze = 128,
    /// <summary>
    /// Allow the end-user to group lines
    /// </summary>
    Group = 256,
    /// <summary>
    /// Allow the end-user to change the visibility
    /// </summary>
    Visibility = 512,
    /// <summary>
    /// Default grid features
    /// </summary>
    Default = Filter | Sort | Paging | Count | Profile | Resize | Move | Freeze | Group | Visibility,
    /// <summary>
    /// Allow the end user to add new lines
    /// </summary>
    Add = 1024,
    /// <summary>
    /// Allow the delete option in selection
    /// </summary>
    Delete = 2048,
    /// <summary>
    /// All is allowed
    /// </summary>
    All = Default | Add | Delete
}

/// <summary>
/// Grid style type
/// </summary>
public enum GridStyleType
{
    /// <summary>
    /// Grid
    /// </summary>
    Grid,
    /// <summary>
    /// Card
    /// </summary>
    Card
}
