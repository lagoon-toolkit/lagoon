namespace Lagoon.UI.Components.WorkScheduler.Internal.Timelines;


/// <summary>
/// Timeline with Month on first row, day number on second row
/// </summary>
/// <typeparam name="TItem">IWorkSchedulerData</typeparam>
public partial class TLMonthWeek<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    #region Cascading parameter

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
    /// Check if the date is today
    /// </summary>
    /// <param name="d">Date to check</param>
    /// <returns><c>true</c> if equal today, <c>false</c> otherwise</returns>
    private static bool IsCurrentWeek(DateTime d)
    {
        return d.Year == DateTime.Now.Year && ISOWeek.GetWeekOfYear(d) == ISOWeek.GetWeekOfYear(DateTime.Now);
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
        // TODO SDE Checker si nécessaire
        //if (!LgWorkScheduler.ShowWeekend)
        //    totalDays -= LgWorkScheduler.NbWeekend(from, to);

        return totalDays;
    } 

    #endregion

}
