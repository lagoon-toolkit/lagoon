namespace Lagoon.Helpers;

/// <summary>
/// Date part order
/// </summary>
public enum ExportDateOrder
{
    /// <summary>
    /// Day-Month-Year
    /// </summary>
    DayMonthYear,

    /// <summary>
    /// Month-Day-Year
    /// </summary>
    MonthDayYear,

    /// <summary>
    /// Year-Month-Day
    /// </summary>
    YearMonthDay
}

/// <summary>
/// Framing type for record exports
/// </summary>
public enum ExportDelimiteMode
{
    /// <summary>
    /// Don't add quote around values.
    /// </summary>
    None,

    /// <summary>
    /// Add quote around text values.
    /// </summary>
    Alpha,

    /// <summary>
    /// Add quote around all values.
    /// </summary>
    All
}
