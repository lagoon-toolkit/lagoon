namespace Lagoon.UI.Components.WorkScheduler.Internal.Timelines;

/// <summary>
/// Timeline with Month on first row, Am / Pm on second rows
/// </summary>
/// <typeparam name="TItem">IWorkSchedulerData</typeparam>
public partial class TLDayHour<TItem> : ComponentBase where TItem : IWorkSchedulerData
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

    // Shortcut LgWorkScheduler.TimelineHeight
    private string TimelineRowHeight => $"height:{(int)WorkScheduler.TimelineHeight}px;";

    // Get the width of a day according to ZoomLevel
    private string DayWidth => $"width:{24 * WorkScheduler.ZoomLevel}px;";

    /// <summary>
    /// Check if the date is today
    /// </summary>
    /// <param name="d">Date to check</param>
    /// <returns><c>true</c> if equal today, <c>false</c> otherwise</returns>
    private static bool IsToday(DateTime d)
    {
        return d.Year == DateTime.Now.Year && d.Month == DateTime.Now.Month && d.Day == DateTime.Now.Day;
    } 

    #endregion
}
