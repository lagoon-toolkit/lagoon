namespace Lagoon.UI.Components.WorkScheduler.Internal.Timelines;


/// <summary>
/// Workscheduler timeline manager
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class WsTimeline<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    #region Cascading Parameters

    /// <summary>
    /// Get or set the parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    #endregion

    #region Privates vars

    // To track the actual zoomLevel
    private double _zoomLevel = -1;

    // To track the timeline's start date
    private DateTime _from;

    // To track the timeline's end date
    private DateTime _to;

    // To track the showWeekend flaf
    private bool _showWeekend;

    // To track the height if the timeline
    private int _timelineHeight;

    // To track render state by childrens
    private bool _shouldRender = true;

    // To track the current timeline mode
    private TimelineMode _timelineMode;

    // To track the current timeline display mode
    private TimelineDisplayMode _timelineDisplayMode;

    #endregion

    /// <summary>
    /// When the WsTimeline parameter set complete, check with the cached values have been updated
    /// and if update, we have to render the timeline again
    /// </summary>
    protected override Task OnParametersSetAsync()
    {
        if (_zoomLevel != WorkScheduler.ZoomLevel)
        {
            _zoomLevel = WorkScheduler.ZoomLevel;
            _shouldRender = true;
        }
        if (_from != WorkScheduler.From.Value)
        {
            _from = WorkScheduler.From.Value;
            _shouldRender = true;
        }
        if (_to != WorkScheduler.To.Value)
        {
            _to = WorkScheduler.To.Value;
            _shouldRender = true;
        }
        if (_showWeekend != WorkScheduler.ShowWeekend)
        {
            _showWeekend = WorkScheduler.ShowWeekend;
            _shouldRender = true;
        }
        if (_timelineHeight != WorkScheduler.TimelineHeight)
        {
            _timelineHeight = WorkScheduler.TimelineHeight;
            _shouldRender = true;
        }
        if (_timelineMode != WorkScheduler.Timeline)
        {
            _timelineMode = WorkScheduler.Timeline;
            _shouldRender = true;
        }
        if (_timelineDisplayMode != WorkScheduler.TimelineDisplayMode)
        {
            _timelineDisplayMode = WorkScheduler.TimelineDisplayMode;
            _shouldRender = true;
        }
        return base.OnParametersSetAsync();
    }

    #region internal methods used by childrens via a CascadingValue

    /// <summary>
    /// Get the ShouldRender state.
    /// Rq: The ShouldRender flag is reset when this method is consumed.
    /// </summary>
    /// <returns></returns>
    internal bool GetShouldRender()
    {
        bool souldRender = _shouldRender;
        if (_shouldRender)
        {
            _shouldRender = false;
        }
        return souldRender;
    }

    #endregion

}
