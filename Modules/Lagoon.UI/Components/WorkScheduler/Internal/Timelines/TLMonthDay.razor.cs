namespace Lagoon.UI.Components.WorkScheduler.Internal.Timelines;


/// <summary>
/// Timeline with Month on first row, day number on second row
/// </summary>
/// <typeparam name="TItem">IWorkSchedulerData</typeparam>
public partial class TLMonthDay<TItem> : ComponentBase where TItem : IWorkSchedulerData
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
    /// <returns><c>true</c> if the view should be re-render</returns>
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
    private static bool IsToday(DateTime d)
    {
        return d.Year == DateTime.Now.Year && d.Month == DateTime.Now.Month && d.Day == DateTime.Now.Day;
    }

    /// <summary>
    /// How many days to display for this year/month (with From / To constraints)
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="month">Mont</param>
    /// <returns>Days count excluding From/To start area</returns>
    private int GetDisplayDays(int year, int month)
    {
        int dayInMonth = DateTime.DaysInMonth(year, month);
        if (WorkScheduler.From.Value.Year == year && WorkScheduler.From.Value.Month == month && WorkScheduler.From.Value.Day != 1)
        {
            return dayInMonth - WorkScheduler.From.Value.Day + 1;
        }
        else if (WorkScheduler.To.Value.Year == year && WorkScheduler.To.Value.Month == month && WorkScheduler.To.Value.Day != dayInMonth)
        {
            return WorkScheduler.To.Value.Day;
        }
        else
        {
            return dayInMonth;
        }
    } 

    #endregion

}
