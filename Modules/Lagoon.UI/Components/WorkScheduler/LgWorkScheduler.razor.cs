using Lagoon.UI.Components.Forms;
using Lagoon.UI.Components.WorkScheduler.Configuration;
using System.Collections.ObjectModel;

namespace Lagoon.UI.Components;

/// <summary>
/// Component used to display workload in a schedule view
/// </summary>
public partial class LgWorkScheduler<TItem> : LgComponentBase, IFormTrackerComponent where TItem : IWorkSchedulerData
{

    #region Fields

    // Main container ref. Used to retrieve available space and adapt layout size
    private ElementReference _mainContainer;

    // Shceduler container which contain scrolls
    private ElementReference _schedulerContainer;

    // Project container ref. Used to synchro scroll position
    private ElementReference _projectInnerContainer;

    // Timeline container ref. Used to synchro scroll position
    private ElementReference _timelineContainer;

    // Element used to start project list resizing
    private ElementReference _resizeProjectGripper;

    // Right container
    private ElementReference _timelineAndScheduleContainer;

    // Project container
    private ElementReference _projectOuterContainer;

    // JS Script must be able to call c# function
    private DotNetObjectReference<LgWorkScheduler<TItem>> _dotNetObjRef;

    // Row visible count (depending on available height and row height)
    private int _rowVisible = 10;

    // Vertical scroll position. Used to compute visible rows
    private int _scrollPosition = 0;

    // To allow scroll by pixel
    private double _paddingScroll = 0;

    // Available space for drawing schedule area
    private int _lastAvailableHeight = -1;

    // Height used by all rows
    internal string TotalHeight
    {
        get
        {
            if (DisplayMode == DisplayMode.Timeline)
            {
                int visibleRow = CountVisibleRows(false);
                double adjust = 0;
                // Scroll is sycnhro to padding, so the first row is alway show completly
                // we have to adapt the container height to allow scroll to the last element
                if (VirtualScrolling && _lastAvailableHeight > 0 && _rowVisible * RowHeight < _lastAvailableHeight - TimelineHeight)
                {
                    adjust = (_lastAvailableHeight - TimelineHeight) - (_rowVisible * RowHeight);
                }
                return $"{(visibleRow * RowHeight) + adjust}px";
            }
            else
            {
                return $"{(AgendaHoursTo.Subtract(AgendaHoursFrom).TotalMinutes / AgendaRowStep) * RowHeight}px";
            }
        }
    }

    // ref to lgworkscheduler javascript object manager
    private JsObjectRef _jsRef;

    // additionnal log
    private readonly bool _debug = false;

    // used to track grouping state 
    private readonly Dictionary<string, GroupState> _groupingState = new();

    // The current element which is currently moved
    private TItem _currentDraggedItem;

    internal Guid Id = Guid.NewGuid();

    #endregion

    #region Parameters

    /// <summary>
    /// Get or set the callback used to sort the data to display
    /// </summary>
    [Parameter]
    public Func<IEnumerable<TItem>, IEnumerable<TItem>> OnSort { get; set; }

    /// <summary>
    /// Get or set the timeline mode
    /// </summary>
    [Parameter]
    public TimelineMode Timeline { get; set; }

    /// <summary>
    /// LgWorkScheduler display mode
    /// </summary>
    [Parameter]
    public DisplayMode DisplayMode { get; set; } = DisplayMode.Timeline;

    /// <summary>
    /// Get or set the display mode for the timeline
    /// </summary>
    /// <value><see cref="TimelineDisplayMode.TwoRows"/> by default</value>
    [Parameter]
    public TimelineDisplayMode TimelineDisplayMode { get; set; } = TimelineDisplayMode.TwoRows;

    /// <summary>
    /// Get or set the display mode for the timeline
    /// </summary>
    /// <value><see cref="TimelineDisplayMode.TwoRows"/> by default</value>
    [Parameter]
    public GroupDisplayMode GroupDisplayMode { get; set; } = GroupDisplayMode.Row;

    /// <summary>
    /// Used when <c>DisplayMode</c>=<c>DisplayMode.Agenda</c> to specify vertical axis increments in minutes
    /// </summary>
    /// <value>30 minutes by default</value>
    [Parameter]
    public int AgendaRowStep { get; set; } = 30;

    /// <summary>
    /// Used when <c>DisplayMode</c>=<c>DisplayMode.Agenda</c> to specify vertical axis start time
    /// </summary>
    /// <value>0h as default value</value>
    [Parameter]
    public TimeSpan AgendaHoursFrom { get; set; }

    /// <summary>
    /// Used when <c>DisplayMode</c>=<c>DisplayMode.Agenda</c> to specify vertical axis stop time
    /// </summary>
    /// <value>23h as default value</value>
    [Parameter]
    public TimeSpan AgendaHoursTo { get; set; }

    /// <summary>
    /// Used when <c>DisplayMode</c>=<c>DisplayMode.Agenda</c> to specify vertical axis highlight start time
    /// </summary>
    /// <value>8h by default</value>
    [Parameter]
    public TimeSpan AgendaHoursFromHighlight { get; set; }

    /// <summary>
    /// Used when <c>DisplayMode</c>=<c>DisplayMode.Agenda</c> to specify vertical axis highlight stop time
    /// </summary>
    /// <value>19h by default</value>
    [Parameter]
    public TimeSpan AgendaHoursToHighlight { get; set; }

    /// <summary>
    /// Get or set the timeline height in px. 50px by default
    /// </summary>
    [Parameter]
    public int TimelineHeight { get; set; } = 50;

    /// <summary>
    /// Get or set a value indicating if weekend should be displayed or not
    /// </summary>
    /// <value><c>true</c> by default</value>
    [Parameter]
    public bool ShowWeekend { get; set; } = true;

    /// <summary>
    /// Get or set the start date displayed.
    /// If not set the <c>From</c> value will be retrieved from datasource
    /// </summary>
    [Parameter]
    public DateTime? From { get; set; } = null;

    /// <summary>
    /// Get or set the end date displayed
    /// If not set the <c>To</c> value will be retrieved from datasource
    /// </summary>
    [Parameter]
    public DateTime? To { get; set; } = null;

    /// <summary>
    /// Get or set the amount of pixels representing one hour
    /// </summary>
    [Parameter]
    public double ZoomLevel { get; set; } = 4;

    /// <summary>
    /// Get or set the value in pixel of project list width. 200px by default
    /// </summary>
    [Parameter]
    public int ProjectListWidth { get; set; } = 200;

    /// <summary>
    /// Get or set the value in pixel of a row height. 50px by default
    /// </summary>
    [Parameter]
    public int RowHeight { get; set; } = 50;

    /// <summary>
    /// Get or set the value in pixel of a row padding. 7px by defult
    /// </summary>
    [Parameter]
    public int PaddingRow { get; set; } = 7;

    /// <summary>
    /// Get or set data
    /// </summary>
    [Parameter]
    public ICollection<TItem> Items { get; set; }

    /// <summary>
    /// Event fired when a Task or Milestone is clicked
    /// </summary>
    [Parameter]
    public EventCallback<TItem> OnItemClick { get; set; }

    /// <summary>
    /// Event fired when a click in an empty zone (without Task or Milestone)
    /// </summary>
    /// <value>Return an <see cref="OnEmptyClickEventArgs" /></value>
    [Parameter]
    public EventCallback<OnEmptyClickEventArgs> OnEmptyClick { get; set; }

    /// <summary>
    /// Event fired when a task or milestone has been moved by Drag and Drop
    /// </summary>
    /// <value>Return an <see cref="OnDragCompletedEventArgs{TItem}"  /></value>
    [Parameter]
    public EventCallback<OnDragCompletedEventArgs<TItem>> OnDragCompleted { get; set; }

    /// <summary>
    /// When drag / drop is enabled you can adjust the moving step (free, hour by hour, ...)
    /// Default is <see cref="DragStepType.Free"/>
    /// </summary>
    [Parameter]
    public DragStepType DragStepType { get; set; } = DragStepType.Free;

    /// <summary>
    /// Get or set the drag step size when <see cref="DragStepType"/> is set to <see cref="DragStepType.Manual"/>
    /// </summary>
    [Parameter]
    public TimeSpan? DragStepSize { get; set; }

    /// <summary>
    /// Get or set a flag to show / hide mouse position. 
    /// </summary>
    /// <value><c>true</c> by default</value>
    [Parameter]
    public bool ShowMouseIndicator { get; set; } = true;

    /// <summary>
    /// Get or set a value indicating if virtual scrolling functionnality is enable or not. 
    /// Should be used when you have a lot of data to display
    /// </summary>
    /// <value><c>false</c> by default</value>
    //[Parameter] //Do not uncomment
    private bool VirtualScrolling { get; set; } = false;

    /// <summary>
    /// Get or set a value indicating if the project list should be resizable by dragging an element
    /// </summary>
    /// <value><c>true</c> by default</value>
    [Parameter]
    public bool ShowProjectResizing { get; set; } = true;

    /// <summary>
    /// Field to use for groupping rows
    /// </summary>
    [Parameter]
    public Func<TItem, object> RowField { get; set; }

    /// <summary>
    /// Get or set the list of GroupBy properties to apply on <see cref="Items" /> data
    /// </summary>
    /// <value></value>
    [Parameter]
    public IEnumerable<Func<TItem, object>> GroupBy { get; set; }

    /// <summary>
    /// Field to use for groupping rows
    /// </summary>
    [Parameter]
    public IEnumerable<Func<TItem, object>> OrderBy { get; set; }

    /// <summary>
    /// Get or set the margin identation used to display grouped data (30px by default)
    /// </summary>
    [Parameter]
    public int GroupMargin { get; set; } = 30;

    /// <summary>
    /// Get or set the default group state <see cref="GroupState"/>. (<see cref="GroupState.Close"/> by default)
    /// </summary>
    [Parameter]
    public GroupState DefaultGroupsState { get; set; } = GroupState.Close;

    /// <summary>
    /// Css class used by default for milestone rendering. Default value "wk-milestone-square".
    /// </summary>
    [Parameter]
    public string DefaultJalonCssClass { get; set; } = "wk-milestone-square";

    /// <summary>
    /// Get or set the flag indicating if a line should join all milestones together
    /// </summary>
    /// <value><c>false</c> bu default</value>
    [Parameter]
    public bool JoinMilestone { get; set; } = false;

    /// <summary>
    /// Get or set the step between hours to display when using the Timeline.DayHour
    /// </summary>
    /// <value><c>3</c> by default</value>
    [Parameter]
    public int HourStep { get; set; } = 3;

    #endregion

    #region Cascading parameters

    /// <summary>
    /// Potential form tracker defined by an ancestor 
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgFormTracker FormTracker { get; set; }

    #endregion

    #region Render fragment

    /// <summary>
    /// Get or set the RenderFragment used to display a header for project 
    /// </summary>
    [Parameter]
    public RenderFragment ProjectHeader { get; set; }

    /// <summary>
    /// Get or set the RenderFragement used to display row in project list area
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> ProjectItems { get; set; }

    /// <summary>
    /// Get or set the RenderFragement used to display Group in project list area
    /// </summary>
    [Parameter]
    public RenderFragment<GroupItem<TItem>> ProjectItemsGroup { get; set; } // IGrouping<object, TItem>

    /// <summary>
    /// Get or set the RenderFragement used to display bar and milestones 
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> SchedulerItems { get; set; }

    /// <summary>
    /// Get or set the RenderFragement used to display bar and milestones 
    /// </summary>
    [Parameter]
    public RenderFragment<GroupItem<TItem>> SchedulerItemsGroup { get; set; } //IGrouping<object, TItem>

    /// <summary>
    /// Get or set the RenderFragement used to display bar and milestones 
    /// </summary>
    [Parameter]
    public RenderFragment<TimeSpan> AgendaHoursRender { get; set; }

    /// <summary>
    /// Get or set the RenderFragment used to display the first level of the timeline
    /// </summary>
    [Parameter]
    public RenderFragment<DateTime> TimelineFirstLevel { get; set; }

    /// <summary>
    /// Get or set the RenderFragment used to display the second level of the timeline
    /// </summary>
    [Parameter]
    public RenderFragment<DateTime> TimelineSecondLevel { get; set; }

    /// <summary>
    /// Should not be used ...
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region Initialisation

    /// <summary>
    /// Create a reference to this component instance and register a js callback when gripper position change
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Dotnet ref for JS/C# invokation
        _dotNetObjRef = DotNetObjectReference.Create(this);
        // Register from tracker
        InitFormTracker();
    }

    /// <summary>
    /// Attach an event listener to detect window size change and synchronise scrollbar
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Init javascript part
            _jsRef = await JS.ScriptGetNewRefAsync("Lagoon.LgWorkScheduler.Init", _dotNetObjRef, VirtualScrolling, ShowMouseIndicator, TimelineHeight, RowHeight, PaddingRow, GetDragStep(), ZoomLevel * 24, DisplayMode == DisplayMode.Agenda,
                        _mainContainer, _schedulerContainer, _timelineContainer, _projectOuterContainer, _projectInnerContainer, _timelineAndScheduleContainer, _resizeProjectGripper, Id);
        }
        // _jsRef could be null on disposing
        if (_jsRef != null)
        {
            // Run a js function to center text inside timeline
            JS.InvokeVoid("Lagoon.LgWorkScheduler.CenterTimeline", _jsRef);
            if (!firstRender)
            {
                JS.InvokeVoid("Lagoon.LgWorkScheduler.RefreshDragDrop", _jsRef, RowHeight, PaddingRow, GetDragStep(), ZoomLevel * 24, DisplayMode == DisplayMode.Agenda);
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion

    #region IFormTrackerComponent 

    /// <summary>
    /// Internal flag used to follow pending modification
    /// </summary>
    private bool _hasPendingModifications = false;

    /// <inheritdoc />
    [Parameter]
    public bool IgnoreFormTracking { get; set; }

    /// <inheritdoc />
    public bool IsModified()
    {
        return !IgnoreFormTracking && _hasPendingModifications;
    }

    /// <inheritdoc />
    public void SetModifiedState(bool state)
    {
        if (!IgnoreFormTracking)
        {
            _hasPendingModifications = state;
            FormTracker?.FormTrackerHandler?.RaiseFieldChanged();
        }
    }

    /// <summary>
    /// Register to a potential LgFormTracker parent component
    /// </summary>
    private void InitFormTracker()
    {
        if (!IgnoreFormTracking)
        {
            FormTracker?.RegisterComponent(this);
            // If the source is an ObservableCollection, subscribes to collection change events
            if (Items is ObservableCollection<TItem> ObservableItems)
            {
                ObservableItems.CollectionChanged += ObservableItems_CollectionChanged;
                foreach (TItem item in ObservableItems)
                {
                    // If the properties implements INotifyPropertyChanged subscribe to 
                    // PropertyChanged event
                    if (item is INotifyPropertyChanged npc)
                    {
                        npc.PropertyChanged += Item_PropertyChanged;
                    }
                }
            }
        }
    }

    #endregion

    #region ObservableCollection

    /// <summary>
    /// When a TItem is added or removed from/to the <see cref="Items"/> collection, fire event to a potential LgFormTracker parent component
    /// </summary>
    /// <param name="sender">Unsed</param>
    /// <param name="e">Unsed</param>
    private void ObservableItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Don't fire an event if there already previous pending modification
        if (!IgnoreFormTracking && !_hasPendingModifications)
        {
            _hasPendingModifications = true;
            FormTracker?.SetExplicitModificationStateAsync();
        }
    }

    /// <summary>
    /// When the property of a TItem change, fire event to a potential LgFormTracker parent component
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Unused</param>
    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // Don't fire an event if there already previous pending modification
        if (!IgnoreFormTracking && !_hasPendingModifications)
        {
            _hasPendingModifications = true;
            FormTracker?.SetExplicitModificationStateAsync();
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Try to set the horizontal scrollbar to current day
    /// </summary>
    public async Task ScrollToTodayAsync()
    {
        await JS.InvokeVoidAsync("Lagoon.LgWorkScheduler.ScrollToToday", _jsRef);
    }

    /// <summary>
    /// Set the horizontal scrollbar to the end (max right)
    /// </summary>
    public void ScrollToEnd()
    {
        JS.InvokeVoid("Lagoon.LgWorkScheduler.ScrollToEnd", _jsRef);
    }

    /// <summary>
    /// Set the horizontal scrollbar to the start (left)
    /// </summary>
    public void ScrollToStart()
    {
        JS.InvokeVoid("Lagoon.LgWorkScheduler.ScrollToStart", _jsRef);
    }

    /// <summary>
    /// Return the horizontal position of a task (depending on it's From date, 1st date displayed and weekend displayed or not)
    /// </summary>
    /// <param name="item">Item for which we want the horizontal position in px</param>
    /// <returns></returns>
    public double GetTaskPosition(TItem item)
    {
        return GetDatePosition(item.From);
    }

    /// <summary>
    /// Return the horizontal position of a Date (depending on 1st date displayed and weekend displayed or not)
    /// </summary>
    /// <param name="start">Date</param>
    /// <returns></returns>
    public double GetDatePosition(DateTime start)
    {
        double hours = start.Subtract(From.Value).TotalHours;
        if (!ShowWeekend)
        {

            int nbweekend = NbWeekend(From.Value, start);
            hours -= (nbweekend * 24);

        }
        return hours * ZoomLevel;
    }

    /// <summary>
    /// Return the width of a task (depending on it's duration and weekend displayed or not)
    /// </summary>
    /// <param name="item">Item for which we want the duration in px</param>
    /// <returns>A duration in px according to ZoomLevel</returns>
    public double GetTaskWidth(TItem item)
    {
        return GetDurationWidth(item.From, item.To.Value);
    }

    /// <summary>
    /// Return the width in px from 2 days according to ZoomLevel
    /// </summary>
    /// <param name="from">DateMin</param>
    /// <param name="to">DateMax</param>
    /// <returns>Duration in pixels</returns>
    public double GetDurationWidth(DateTime from, DateTime to)
    {
        double hours = to.Subtract(from).TotalHours;
        if (!ShowWeekend)
        {
            int nbweekend = NbWeekend(from, to);
            hours -= nbweekend * 24;
        }
        return GetDurationWidth(hours);
    }

    /// <summary>
    /// Return the size (width or height, depending if Timeline or Agenda mode) in px from a duration in hour
    /// </summary>
    /// <param name="hours">How many hours?</param>
    /// <returns>Duration in pixels</returns>
    public double GetDurationWidth(double hours)
    {
        if (DisplayMode == DisplayMode.Timeline)
        {
            return hours * ZoomLevel;
        }
        else
        {
            return hours * 60 * RowHeight / AgendaRowStep;
        }
    }

    /// <summary>
    /// Return a DateTime representing the left positionning
    /// </summary>
    /// <param name="leftPosition">The left position of an element</param>
    public DateTime GetDateFromLeftPosition(double leftPosition)
    {
        if (ShowWeekend)
        {
            return From.Value.AddHours(leftPosition / ZoomLevel);
        }
        else
        {
            // We can't simply get date from left position if weekends are hidden
            DateTime currentDT = From.Value;
            double currentLeft = 0;
            while (currentLeft < leftPosition || IsWeekend(currentDT))
            {
                if (!IsWeekend(currentDT))
                {
                    currentLeft += ZoomLevel * 24;
                }
                currentDT = currentDT.AddDays(1);
            }
            return currentDT;
        }
    }

    #endregion

    #region Privates methods

    #region JSInvokables

    /// <summary>
    /// Retrieve vertical scroll position and update visibles rows according to scroll position
    /// </summary>
    /// <param name="scrollPosition">Vertical scroll position</param>
    /// <param name="height">Main container height</param>
    [JSInvokable]
    public void OnScrollChanged(int scrollPosition, int height)
    {
        _lastAvailableHeight = height;
        if (VirtualScrolling && _scrollPosition != scrollPosition)
        {
            // Store actual scroll position
            _scrollPosition = scrollPosition;
            _paddingScroll = _scrollPosition;
            // This method is called from js, we need to apply changes
            StateHasChanged();
        }
    }

    /// <summary>
    /// When available height is update, redraw the view
    /// </summary>
    /// <param name="height">New available height</param>
    [JSInvokable]
    public void OnHeightChanged(int height)
    {
        int rowVisible = (height - TimelineHeight) / RowHeight;
        _lastAvailableHeight = height;
        if (rowVisible != _rowVisible)
        {
            // Store how many rows are visible
            _rowVisible = rowVisible;
            // This method is called from js, we need to apply changes
            StateHasChanged();
        }
    }

    /// <summary>
    /// Invoked from JS when clicking in a row without task or milestone
    /// </summary>
    /// <param name="position">Click position. Used to compute de DateTime</param>
    /// <param name="key">RowId identifier</param>
    [JSInvokable]
    public async Task OnInternalEmptyItemClickedAsync(long position, object key)
    {
        if (OnEmptyClick.HasDelegate)
        {
            if (DisplayMode == DisplayMode.Timeline)
            {
                await OnEmptyClick.TryInvokeAsync(App, new OnEmptyClickEventArgs(key, From.Value.AddHours(position / ZoomLevel)));
            }
            else
            {
                var clickedDate = From.Value.AddHours(position / ZoomLevel);
                var clickedTime = new DateTime(long.Parse(key.ToString()));
                await OnEmptyClick.TryInvokeAsync(App, new OnEmptyClickEventArgs(null, new DateTime(clickedDate.Year, clickedDate.Month,clickedDate.Day, clickedTime.Hour, clickedTime.Minute, clickedTime.Second)));
            }
        }
    }

    /// <summary>
    /// if a <see cref="DragStepType"/> is provided return the minimal step when dragging an element  
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">if the <see cref="DragStepType"/> is not know</exception>
    public double GetDragStep()
    {
        if (DragStepType == DragStepType.Manual && !DragStepSize.HasValue)
        {
            throw new InvalidOperationException($"You must set an {nameof(DragStepSize)} when using {(nameof(DragStepType))} = {(nameof(DragStepType.Manual))}");
        }
        if (DisplayMode == DisplayMode.Timeline)
        {
            // When Timeline mode, step are constraint by the zoomLevel
            return DragStepType switch
            {
                DragStepType.Free => 0,
                DragStepType.Day => ZoomLevel * 24,
                DragStepType.Hour => ZoomLevel,
                DragStepType.Manual => DragStepSize.Value.TotalHours * ZoomLevel,
                _ => throw new NotImplementedException(),
            };
        }
        else
        {
            // When Agenda mode, step are constraint by the main step size and row height
            return DragStepType switch
            {
                DragStepType.Free => RowHeight / AgendaRowStep,
                DragStepType.Day => 24 * 60 * RowHeight / AgendaRowStep,
                DragStepType.Hour => 60 * RowHeight / AgendaRowStep,
                DragStepType.Manual => DragStepSize.Value.TotalMinutes * RowHeight / AgendaRowStep,
                _ => throw new NotImplementedException(),
            };
        }
    }

    /// <summary>
    /// Track the element from wich the drag as started
    /// </summary>
    /// <param name="item"></param>
    internal void SetCurrentDraggedItem(TItem item)
    {
        _currentDraggedItem = item;
    }

    /// <summary>
    /// Called by js when an item as been moves
    /// </summary>
    /// <param name="rowIdOrNewTop">The RowId in Timeline mode or the new top in agenda mode</param>
    /// <param name="newLeft">The position where the item has been dropped</param>
    /// <param name="externalItemIdentifier"><c>true</c> if the dropped item come from an external item, <c>false</c> if the dropped item come from the datasource</param>
    /// <param name="taskHeight">The height of the task.</param>
    /// <returns>The Task</returns>
    [JSInvokable]
    public async Task<bool> CompleteDragProcessAsync(string rowIdOrNewTop, double newLeft, string externalItemIdentifier, double? taskHeight)
    {
        DateTime newDate = GetDateFromLeftPosition(newLeft);
        if (DisplayMode == DisplayMode.Agenda)
        {
            // in agenda mode, the hour part of the newDate is depending on the topPosition (where the user drop the item)
            // compute the hours based on the first time displayed in the agenda and the size of a step (RowHeight vs AgendaRowStep)
            DateTime refDt = new(newDate.Year, newDate.Month, newDate.Day, AgendaHoursFrom.Hours, AgendaHoursFrom.Minutes, AgendaHoursFrom.Seconds);
            var newTop = double.Parse(rowIdOrNewTop);
            var minutesFromStart = (int)((newTop / RowHeight) * AgendaRowStep);
            newDate = refDt.AddMinutes(minutesFromStart);
        }
        // If the dropped item is coming from the datasource
        if (string.IsNullOrEmpty(externalItemIdentifier))
        {
            DateTime? newEnd = null;
            if (_currentDraggedItem.To.HasValue)
            {
                double endOffset = _currentDraggedItem.To.Value.Subtract(_currentDraggedItem.From).TotalMilliseconds;
                newEnd = newDate.AddMilliseconds(endOffset);
            }
            // Fire OnDragCompleted event if subscriber (to allow to cancel the move event)
            if (OnDragCompleted.HasDelegate)
            {
                var dragEventComplete = new OnDragCompletedEventArgs<TItem>(_currentDraggedItem, rowIdOrNewTop, newDate, newEnd);
                await OnDragCompleted.TryInvokeAsync(App, dragEventComplete);
                if (dragEventComplete.Cancel)
                {
                    return false;
                }
            }
            // Update item position from the data source
            _currentDraggedItem.From = newDate;
            _currentDraggedItem.To = newEnd;
        }
        // Item coming from an external souce
        else
        {
            DateTime? newEnd = null;
            if (taskHeight != null)
            {
                var r = taskHeight.Value * AgendaRowStep / RowHeight;
                //TimeSpan t = new TimeSpan(0, r, 0);
                newEnd = newDate.AddMinutes(r);
            }
            // Fire OnDragCompleted event if subscriber (to allow to cancel the move event)
            if (OnDragCompleted.HasDelegate)
            {
                var dragEventComplete = new OnDragCompletedEventArgs<TItem>(_currentDraggedItem, rowIdOrNewTop, newDate, newEnd, true, externalItemIdentifier);
                await OnDragCompleted.TryInvokeAsync(App, dragEventComplete);
                if (dragEventComplete.Cancel)
                {
                    return false;
                }
            }
        }
        _currentDraggedItem = default;
        // JSInvokable, the OnDragCompleted EventCallback is not enought to refresh the view
        StateHasChanged();
        return true;
    }

    /// <summary>
    /// Drag aborted (outside of planning area)
    /// </summary>
    [JSInvokable]
    public void CancelDragProcess()
    {
        ShowWarning("#WksMoveCancelled");
    }

    /// <summary>
    /// TODO WKS A revoir avec l'ItemClick
    /// </summary>
    [JSInvokable]
    public async Task OnDragItemClickAsync()
    {
        if (OnItemClick.HasDelegate && _currentDraggedItem != null)
        {
            await OnItemClick.TryInvokeAsync(App, _currentDraggedItem);
        }
    }

    #endregion

    #region Group / Row management

    /// <summary>
    /// Get visible items. In VirtualScrolling mode, visible items are based on scroll position
    /// </summary>
    /// <param name="groupByRowField">if <c>true</c> the datasource will be grouped on RowField + return first + counted as one row. Otherwise it will return all visible items + counted as one row</param>
    /// <returns>Visible items from the datasource</returns>
    internal IEnumerable<TItem> GetVisibleItems(bool groupByRowField = true)
    {
        int rowIndex = 0;
        int toSkip = (int)Math.Round((double)_scrollPosition / RowHeight, MidpointRounding.AwayFromZero);
        foreach (IGrouping<object, TItem> group in GetSortedItems().GroupBy(RowField))
        {
            if (!VirtualScrolling || (VirtualScrolling && rowIndex >= toSkip && rowIndex <= toSkip + _rowVisible))
            {
                if (groupByRowField)
                {
                    yield return group.First();
                }
                else
                {
                    foreach (TItem item in group)
                    {
                        yield return item;
                    }
                }
            }
            rowIndex++;
        }
    }

    /// <summary>
    /// Return the number of visible row
    /// </summary>
    /// <param name="ignoreGroupState">If <c>true</c> ignore groupe state (Open/Close)</param>
    /// <param name="groupLevel">Current grouping level</param>
    /// <param name="currentGroup">Current group</param>
    /// <param name="groupKey">Current groupKey</param>
    /// <returns>Return the number of visible row</returns>
    private int CountVisibleRows(bool ignoreGroupState = true, int groupLevel = 0, IGrouping<object, TItem> currentGroup = null, string groupKey = null)
    {
        if (GroupBy != null && GroupBy.Any())
        {
            if (groupLevel < GroupBy.Count())
            {
                int total = 0;
                IEnumerable<TItem> __currentGroup = (IEnumerable<TItem>)currentGroup ?? Items;
                foreach (IGrouping<object, TItem> group in __currentGroup.GroupBy(GroupBy.Skip(groupLevel).First()))
                {
                    string key = groupKey + "¤" + group.Key.ToString();
                    if (IsGroupOpen(key) || ignoreGroupState)
                    {
                        total += CountVisibleRows(ignoreGroupState, groupLevel + 1, group, key) + 1;
                    }
                    else
                    {
                        total += 1;
                    }
                }
                return total;
            }
            if (IsGroupOpen(groupKey) || ignoreGroupState)
            {
                // Return the number of items inside this group
                return currentGroup.GroupBy(RowField).Select(e => e.First()).Count();
            }
            return 0;
        }
        else
        {
            // Row count without grouping data
            return Items.GroupBy(RowField).Select(e => e.First()).Count();
        }
    }

    /// <summary>
    /// Returns all binded data with orderby applied if handled by the dev
    /// </summary>
    /// <returns>List of TItems</returns>
    private IEnumerable<TItem> GetSortedItems()
    {
        if (OnSort != null)
        {
            return OnSort.Invoke(Items);
        }
        else
        {
            return Items;
        }
    }

    /// <summary>
    /// Collapse/Expand a group by it's key
    /// </summary>
    /// <param name="key">Group key identifier</param>
    internal void CollapseGroup(string key)
    {
        if (_groupingState.ContainsKey(key))
        {
            // Permut group state
            _groupingState[key] = _groupingState[key] == GroupState.Open ? GroupState.Close : GroupState.Open;
        }
        else
        {
            _groupingState.Add(key, DefaultGroupsState == GroupState.Open ? GroupState.Close : GroupState.Open);
        }
        // Call from a children (ProjectView or SchedulerView). Need to refresh with new grouping state
        StateHasChanged();
    }

    /// <summary>
    /// Check if a group is opened
    /// </summary>
    /// <param name="key">Group key to check</param>
    /// <returns><c>true</c> for opened group, <c>false</c> for collapsed group</returns>
    internal bool IsGroupOpen(string key)
    {
        if (_groupingState.ContainsKey(key))
        {
            return _groupingState[key] == GroupState.Open;
        }
        else
        {
            return DefaultGroupsState == GroupState.Open;
        }
    }

    #endregion

    #region Time management

    // TODO A revoir avec les timelines
    internal double DayWidth(int? col = null)
    {
        switch (Timeline)
        {
            case TimelineMode.MonthDay:
                return 24 * ZoomLevel;
            case TimelineMode.MonthWeek:
                if (col == 0)
                {
                    int dayCount = (ShowWeekend ? 7 : 5) - (int)From.Value.DayOfWeek + 1;
                    return dayCount * 24 * ZoomLevel;
                }
                else
                {
                    return (ShowWeekend ? 7 : 5) * 24 * ZoomLevel;
                }
            case TimelineMode.DayAmPm:
                return 12 * ZoomLevel;

            case TimelineMode.DayHour:
                return HourStep * ZoomLevel;

            case TimelineMode.YearMonth:
                DateTime current = From.Value.AddMonths(col.Value);
                int days = DaysInMonth(current.Year, current.Month);
                return days * 24 * ZoomLevel;
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Return the number of column alternance to match timeline column.
    /// Depending on timeline mode
    /// </summary>
    /// <value>Number of columns to display</value>
    internal int AlternanceColumnCount
    {
        get
        {
            int days = To.Value.Subtract(From.Value).Days;
            if (!ShowWeekend)
            {
                days -= NbWeekend(From.Value, To.Value);
            }
            switch (Timeline)
            {
                case TimelineMode.MonthDay:
                    return days;

                case TimelineMode.DayAmPm:
                    return days * 2 + 1;

                case TimelineMode.DayHour:
                    return (days + 1) * (24 / HourStep);

                case TimelineMode.MonthWeek:
                    return Convert.ToInt32((To.Value - From.Value).TotalDays / 7);

                case TimelineMode.YearMonth:
                    int months = 0;
                    DateTime current = From.Value;
                    while (current < To.Value)
                    {
                        months++;
                        current = current.AddMonths(1);
                    }
                    return months;
            }
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Return the width of the timeline in px. Depending on From/To datetime, zoom level and weekend displayed or not
    /// </summary>
    /// <value>The width of the timeline in px</value>
    internal double TotalTimelineWidth
    {
        get
        {
            int days = To.Value.Subtract(From.Value).Days;
            if (!ShowWeekend)
            {
                days -= NbWeekend(From.Value, To.Value);
            }

            return (days + 1) * 24 * ZoomLevel;
        }
    }

    private int DaysInMonth(int year, int month)
    {
        DateTime from;
        DateTime to;

        if (year == From.Value.Year && From.Value.Month == month)
        {
            from = From.Value;
        }
        else
        {
            from = new DateTime(year, month, 1);
        }

        if (year == To.Value.Year && To.Value.Month == month)
        {
            to = To.Value;
        }
        else
        {
            to = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        int totalDays = (to - from).Days + 1;
        if (!ShowWeekend)
        {
            totalDays -= NbWeekend(from, to);
        }

        return totalDays;
    }

    /// <summary>
    /// Check if a date is a weekend
    /// </summary>
    /// <param name="d">Date to check</param>
    /// <returns><c>true</c> if 'd' is a weekend, false otherwise</returns>
    internal static bool IsWeekend(DateTime d)
    {
        return d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Return weekend count for a particular month
    /// </summary>
    /// <param name="year">Year date part</param>
    /// <param name="month">Month date part</param>
    /// <param name="startingDay">An optionnal starting day</param>
    /// <returns>Weekend count</returns>
    internal static int NbWeekend(int year, int month, int? startingDay = null)
    {
        int weekendCount = 0;
        int days = DateTime.DaysInMonth(year, month);
        if (startingDay != null)
        {
            days -= startingDay.Value;
        }

        DateTime d = new(year, month, 1);
        for (int i = 1; i <= days; i++)
        {
            if (IsWeekend(d))
            {
                weekendCount++;
            }

            d = d.AddDays(1);
        }
        return weekendCount;
    }

    /// <summary>
    /// Return weekend count between to date
    /// </summary>
    /// <param name="from">Start date</param>
    /// <param name="to">End date</param>
    /// <returns>Weekend count</returns>
    internal static int NbWeekend(DateTime from, DateTime to)
    {
        int weekendCount = 0;
        DateTime current = from;
        do
        {
            if (IsWeekend(current))
            {
                weekendCount++;
            }

            current = current.AddDays(1);
        }
        while (current < to);
        return weekendCount;
    }

    /// <summary>
    /// Check if t is between highlighted row
    /// </summary>
    /// <param name="t">Time to check</param>
    /// <returns></returns>
    internal string IsAgendaRowHighlight(TimeSpan t)
    {
        if (t >= AgendaHoursFromHighlight && t <= AgendaHoursToHighlight)
        {
            return "wk-rowhour-highlight";
        }
        return null;
    }

    /// <summary>
    /// Compute the position of a Task for "Agenda" display mode
    /// </summary>
    /// <param name="item">Item for which we want the position</param>
    internal string GetAgendaTaskPosition(TItem item)
    {
        if (!item.To.HasValue)
        {
            throw new InvalidOperationException($"All tasks must have a {(nameof(IWorkSchedulerData.To))} value");
        }
        return GetAgendaTaskPosition(item.From, item.To.Value);
    }

    internal string GetAgendaTaskPosition(DateTime from, DateTime to)
    {
        double top, left, height, width;
        DateTime refDt = new(from.Year, from.Month, from.Day, AgendaHoursFrom.Hours, AgendaHoursFrom.Minutes, AgendaHoursFrom.Seconds);
        top = from.Subtract(refDt).TotalMinutes;
        top = top * RowHeight / AgendaRowStep;
        // To compute the left position, ignore h:m:s from the task since it's used only to compute the top position
        left = GetDatePosition(new DateTime(from.Year, from.Month, from.Day));
        width = 24 * ZoomLevel;
        height = to.Subtract(from).TotalMinutes * RowHeight / AgendaRowStep;
        return $"top:{top}px; left:{left}px; width:{width}px; height:{height}px";
    }

    /// <summary>
    /// For TimelineMode.DayHour add a separator in SchedulerView to highlight day separation
    /// </summary>
    /// <param name="col">Actual column index</param>
    /// <returns>A css class name or empty</returns>
    internal string GetSeparator(int col)
    {
        if (Timeline == TimelineMode.DayHour && ((col + 1) % 8 == 0))
        {
            return "wst-timeline-block-separator";
        }
        return null;
    }

    /// <summary>
    /// Return draggable info (used by javascript/_initDragAndDrop)
    /// </summary>
    /// <param name="item">Item for which we want movable option</param>
    /// <returns>serialized draggable option</returns>
#pragma warning disable CA1822 // Mark members as static: Not working in Razor view due to generic TypeParam
    internal string GetDataMove(TItem item)
#pragma warning restore CA1822 // Mark members as static
    {
        if (item is IWorkSchedulerDraggableData movableItem && (movableItem.AllowHorizontalMove || movableItem.AllowVerticalMove))
        {
            return $"{(movableItem.AllowHorizontalMove ? "x" : null)}|{(movableItem.AllowVerticalMove ? "y" : null)}";
        }
        return null;
    }

    /// <summary>
    /// Fire an event is an item is clicked
    /// </summary>
    /// <param name="item">The item which has been clicked</param>
    /// <returns></returns>
    private async Task ItemClickedAsync(TItem item)
    {
        if (OnItemClick.HasDelegate)
        {
            await OnItemClick.TryInvokeAsync(App, item);
        }
    }

    /// <summary>
    /// Track the element wich as fire a mousedown event
    /// </summary>
    /// <param name="item">Item wich fire the mousedown event</param>
    internal async Task OnStartDragItemAsync(TItem item)
    {
        if (item is IWorkSchedulerDraggableData movableItem && (movableItem.AllowVerticalMove || movableItem.AllowHorizontalMove))
        {
            // Track the item for which a drag process is in progress
            SetCurrentDraggedItem(item);
        }
        else
        {
            // Assume item click
            await ItemClickedAsync(item);
        }
    }

    #endregion

    #endregion

    #region Dispose

    /// <summary>
    /// Free event handlers
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        FormTracker?.UnregisterComponent(this);
        if (JS is IJSInProcessRuntime ijs)
        {
            ijs.InvokeVoid("Lagoon.LgWorkScheduler.Dispose", _jsRef);
        }
        _jsRef.Dispose();
        _jsRef = null;
        _dotNetObjRef.Dispose();
    }


    #endregion

}