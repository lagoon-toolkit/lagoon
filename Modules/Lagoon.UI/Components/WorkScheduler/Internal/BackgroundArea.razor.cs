namespace Lagoon.UI.Components.WorkScheduler.Internal;

/// <summary>
/// A background area.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class BackgroundArea<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    #region Cascading parameter

    /// <summary>
    /// Get or set the parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    #endregion

    /// <summary>
    /// The agenda mode.
    /// </summary>
    [Parameter]
    public bool AgendaMode { get; set; }

    #region Private vars

    // To track the actual zoomLevel
    private double _zoomLevel = -1;

    // To track the timeline's start date
    private DateTime _from;

    // To track the timeline's end date
    private DateTime _to;

    // To track the showWeekend flaf
    private bool _showWeekend;

    // To track the current timeline mode
    private TimelineMode _timelineMode;

    private string _totalHeight;

    // Flag used to know if the view has to be re-rendered
    private bool _shouldRender;

    #endregion

    #region Initialization

    /// <summary>
    /// On parameter set track some var state to handle the ShouldRender manually
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
        if (_timelineMode != WorkScheduler.Timeline)
        {
            _timelineMode = WorkScheduler.Timeline;
            _shouldRender = true;
        }
        if (_totalHeight != WorkScheduler.TotalHeight)
        {
            _totalHeight = WorkScheduler.TotalHeight;
            _shouldRender = true;
        }
        return base.OnParametersSetAsync();
    }

    /// <summary>
    /// Only render this component if used properties provided by the cascading parameter has changed
    /// </summary>
    protected override bool ShouldRender()
    {
        bool shouldRender = _shouldRender;
        if (shouldRender)
        {
            _shouldRender = false;
        }
        return shouldRender;
    }

    #endregion

}
