namespace Lagoon.UI.Components;

/// <summary>
/// Sort direction
/// </summary>
public enum DataSortDirection
{
    /// <summary>
    /// No sort
    /// </summary>
    None,

    /// <summary>
    /// Ascending sort
    /// </summary>
    Asc,

    /// <summary>
    /// Descending sort
    /// </summary>
    Desc
}

/// <summary>
/// Format select export
/// </summary>
public enum ExportFormat
{

    /// <summary>
    /// The item text.
    /// </summary>
    Text,

    /// <summary>
    /// The item value.
    /// </summary>
    Value

}

/// <summary>
/// Format enum export
/// </summary>
public enum ExportEnumFormat
{

    /// <summary>
    /// Enum DisplayName (with translation support by using "#DicoKey").
    /// </summary>
    DisplayName,

    /// <summary>
    /// Enum value as string.
    /// </summary>
    Text,

    /// <summary>
    /// Enum value as int.
    /// </summary>
    Value

}

/// <summary>
/// Calculation type 
/// </summary>
public enum DataCalculationType
{
    /// <summary>
    /// Not active
    /// </summary>
    None = 0,
    /// <summary>
    /// Number of elements
    /// </summary>
    Count,
    /// <summary>
    /// Sum
    /// </summary>
    Sum,
    /// <summary>
    /// Average
    /// </summary>
    Average,
    /// <summary>
    /// Min value
    /// </summary>
    Min,
    /// <summary>
    /// Max value
    /// </summary>
    Max
}
