namespace Lagoon.UI.Components;

/// <summary>
/// Calendar indicator
/// </summary>
public class CalendarIndicator
{

    /// <summary>
    /// Gets or sets range Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets range label
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets HTML Color code
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Gets or sets Css class
    /// </summary>
    public string CssClass { get; set; }

    /// <summary>
    /// Ranges defined for a calendar indicator
    /// </summary>
    public CalendarIndicatorRangeCollection Ranges { get; set; }
}

/// <summary>
/// Calendar indicator collection
/// </summary>
public class CalendarIndicatorRangeCollection : List<CalendarIndicatorRange>
{

}

/// <summary>
/// Calendar indicator range definition
/// </summary>
public class CalendarIndicatorRange
{
    /// <summary>
    /// Range start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Range end date
    /// </summary>
    public DateTime EndDate { get; set; }
}
