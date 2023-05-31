namespace Lagoon.UI.Components.WorkScheduler.Internal.Timelines;


/// <summary>
/// Timeline with Year on first level and month on second level
/// </summary>
/// <typeparam name="TItem">WorkScheduler item type</typeparam>
public partial class TLYearMonth<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    #region Cascading parameters

    /// <summary>
    /// Get or set the parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    /// <summary>
    /// Get or set the WsTimeline manager
    /// </summary>
    [CascadingParameter]
    private WsTimeline<TItem> WsTimeline { get; set; }

    #endregion

    #region Initialisation

    /// <summary>
    /// Handle the ShouldRender manually with a call to the timeline manager (WsTimeline)
    /// </summary>
    /// <returns><c>true</c> if the </returns>
    protected override bool ShouldRender()
    {
        return WsTimeline.GetShouldRender();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Return the number of day in a year with From / To limits
    /// </summary>
    /// <param name="year">Date's year</param>
    /// <returns>Days count</returns>
    private int DaysInYear(int year)
    {
        DateTime from;
        DateTime to;

        if (year == WorkScheduler.From.Value.Year)
        {
            from = WorkScheduler.From.Value;
        }
        else
        {
            from = new DateTime(year, 1, 1);
        }

        if (year == WorkScheduler.To.Value.Year)
        {
            to = WorkScheduler.To.Value;
        }
        else
        {
            to = new DateTime(year, 12, 31);
        }

        int totalDays = (to - from).Days;
        if (!WorkScheduler.ShowWeekend)
            totalDays -= LgWorkScheduler<TItem>.NbWeekend(from, to);

        return totalDays;
    }

    /// <summary>
    /// Check if the provide date is in the same month than actual date
    /// </summary>
    /// <param name="d">Date to check again Now</param>
    /// <returns></returns>
    private static bool IsCurrentMonth(DateTime d)
    {
        return d.Year == DateTime.Now.Year && d.Month == DateTime.Now.Month;
    }

    /// <summary>
    /// Compute the number of days in a month with Form / To limits
    /// </summary>
    /// <param name="year">Date's year</param>
    /// <param name="month">Date's month</param>
    private int DaysInMonth(int year, int month)
    {
        DateTime from;
        DateTime to;

        if (year == WorkScheduler.From.Value.Year && WorkScheduler.From.Value.Month == month)
        {
            from = WorkScheduler.From.Value;
        }
        else
        {
            from = new DateTime(year, month, 1);
        }

        if (year == WorkScheduler.To.Value.Year && WorkScheduler.To.Value.Month == month)
        {
            to = WorkScheduler.To.Value;
        }
        else
        {
            to = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        int totalDays = (to - from).Days + 1;
        if (!WorkScheduler.ShowWeekend)
            totalDays -= LgWorkScheduler<TItem>.NbWeekend(from, to);

        return totalDays;
    }

    #endregion

}
