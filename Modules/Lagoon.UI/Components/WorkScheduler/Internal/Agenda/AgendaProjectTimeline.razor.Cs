namespace Lagoon.UI.Components.WorkScheduler.Internal;

/// <summary>
/// Internal component used to draw a timeline inside project area
/// </summary>
/// <typeparam name="TItem">Item data type</typeparam>
public partial class AgendaProjectTimeline<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    #region Cascading parameters

    /// <summary>
    /// Parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    #endregion

    #region Private vars

    // Track state of AgendaHoursFrom
    private TimeSpan _agendaHoursFrom;
    // Track state of AgendaHoursTo
    private TimeSpan _agendaHoursTo;
    // Track state of AgendaHoursFromHighlight
    private TimeSpan _agendaHoursFromHighlight;
    // Track state of AgendaHoursToHighlight 
    private TimeSpan _agendaHoursToHighlight;
    // Track state of AgendaRowStep 
    private double _agendaRowStep;
    // Track state of RowHeight 
    private int _rowHeight;
    // Manual handling for ShouldRender()
    private bool _shouldRender;

    #endregion

    #region Initialisation

    /// <inheritdoc />
    protected override Task OnParametersSetAsync()
    {
        // optim : re-render this component only if identified prop has changed since the 
        // last render process
        if (_agendaHoursFrom != WorkScheduler.AgendaHoursFrom)
        {
            _agendaHoursFrom = WorkScheduler.AgendaHoursFrom;
            _shouldRender = true;
        }
        if (_agendaHoursTo != WorkScheduler.AgendaHoursTo)
        {
            _agendaHoursTo = WorkScheduler.AgendaHoursTo;
            _shouldRender = true;
        }
        if (_agendaHoursFromHighlight != WorkScheduler.AgendaHoursFromHighlight)
        {
            _agendaHoursFromHighlight = WorkScheduler.AgendaHoursFromHighlight;
            _shouldRender = true;
        }
        if (_agendaHoursToHighlight != WorkScheduler.AgendaHoursToHighlight)
        {
            _agendaHoursToHighlight = WorkScheduler.AgendaHoursToHighlight;
            _shouldRender = true;
        }
        if (_rowHeight != WorkScheduler.RowHeight) {
            _rowHeight = WorkScheduler.RowHeight;
            _shouldRender = true;
        }
        if (_agendaRowStep != WorkScheduler.AgendaRowStep)
        {
            _agendaRowStep = WorkScheduler.AgendaRowStep;
            _shouldRender = true;
        }
        return base.OnParametersSetAsync();
    }

    /// <summary>
    /// Handle the ShouldRender manually with a call to the timeline manager (WsTimeline)
    /// </summary>
    /// <returns><c>true</c> if the </returns>
    protected override bool ShouldRender()
    {
        var shouldRender = _shouldRender;
        if (shouldRender)
        {
            _shouldRender = false;
        }
        return shouldRender;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Return the timespan TimeSpan as HH:MM
    /// </summary>
    /// <param name="t">Value to format</param>
    /// <returns>TimeSpan formated as HH:MM</returns>
    private static string TsToStr(TimeSpan t)
    {
        StringBuilder stb = new();
        stb.Append(t.Hours >= 10 ? t.Hours : $"0{t.Hours}");
        stb.Append(':');
        stb.Append(t.Minutes >= 10 ? t.Minutes : $"0{t.Minutes}");
        return stb.ToString();
    } 

    #endregion

}
