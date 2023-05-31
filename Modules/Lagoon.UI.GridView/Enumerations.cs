namespace Lagoon.UI.Components.Internal;

internal enum GridRenderContext
{
    None,
    Header,
    HeaderGroup,
    Filter,
    Body
}

internal enum GridColumnType
{
    None,
    Boolean,
    DateTime,
    Enum,
    Numeric,
    String
}

/// <summary>
/// Columns export mode
/// </summary>
public enum ExportColumnsMode
{
    /// <summary>
    /// Only displayed columns
    /// </summary>
    DisplayedColumns = 0,
    /// <summary>
    /// All available columns
    /// </summary>
    AllColumns
}

/// <summary>
/// Columns export mode
/// </summary>
public enum ExportRowMode
{
    /// <summary>
    /// Only filtered rows
    /// </summary>
    FilteredRows = 0,
    /// <summary>
    /// All rows
    /// </summary>
    AllRows
}
