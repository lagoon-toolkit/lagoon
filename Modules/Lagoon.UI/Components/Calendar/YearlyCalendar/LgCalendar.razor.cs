namespace Lagoon.UI.Components;

/// <summary>
/// Calendar component
/// </summary>
public partial class LgCalendar : LgComponentBase
{

    #region Public properties

    /// <summary>
    /// Gets or sets the list view Id
    /// </summary>
    [Parameter]
    public string Id { get; set; } = $"cld{Guid.NewGuid():N}";

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the calendar start date
    /// </summary>
    [Parameter]
    public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

    /// <summary>
    /// Gets or sets the calendar end date
    /// </summary>
    [Parameter]
    public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year , 12, 31);

    /// <summary>
    /// List of indicator which be displayed on calendar
    /// </summary>
    [Parameter]
    public IEnumerable<CalendarIndicator> Indicators { get; set; }

    /// <summary>
    /// Get or seets the item template (card for example)
    /// </summary>
    [Parameter]
    public RenderFragment<DateTime> DayContent { get; set; }

    /// <summary>
    /// Gets or sets the button Onclick OnClickDayEventArgs
    /// </summary>
    [Parameter]
    public EventCallback<ClickDayEventArgs> OnDayClick { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("yearly-calendar-content", CssClass);
    }

    /// <summary>
    /// Get list of month with associated year between two date
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns></returns>
    public static IEnumerable<Tuple<string, int, int>> MonthsBetween(DateTime startDate,DateTime endDate)
    {
        DateTime iterator;
        DateTime limit;

        if (endDate > startDate)
        {
            iterator = new DateTime(startDate.Year, startDate.Month, 1);
            limit = endDate;
        }
        else
        {
            iterator = new DateTime(endDate.Year, endDate.Month, 1);
            limit = startDate;
        }
        // TODO a changer
        var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        while (iterator <= limit)
        {
            yield return Tuple.Create(
                dateTimeFormat.GetMonthName(iterator.Month), iterator.Month,iterator.Year
            );
            iterator = iterator.AddMonths(1);
        }
    }

    /// <summary>
    /// On cell day click
    /// </summary>
    /// <param name="date">Clicked date</param>
    protected Task CellClickAsync(DateTime date)
    {
        return ExecuteActionAsync(OnDayClick, (wc) => new ClickDayEventArgs(wc, date));
    }

    #endregion
}
