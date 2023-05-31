namespace Lagoon.UI.Components;


/// <summary>
/// Supported timeline mode
/// </summary>
public enum TimelineMode
{

    /// <summary>
    /// Year on fist line, month on second line
    /// </summary>
    YearMonth,

    /// <summary>
    /// Month on first line, days on second line
    /// </summary>
    MonthDay,

    ///// <summary>
    ///// Week number on first line, day on second line
    ///// </summary>
    //WeekDay,

    /// <summary>
    /// Day on first line, Am/Pm on second line
    /// </summary>
    DayAmPm,

    /// <summary>
    /// Days on first line, hours on second line
    /// </summary>
    DayHour,

    /// <summary>
    /// Month on first line, week number on second line
    /// </summary>
    MonthWeek
}

/// <summary>
/// Supported timeline display mode
/// </summary>
public enum TimelineDisplayMode
{
    /// <summary>
    /// Display timeline in a single row
    /// </summary>
    OneRow,

    /// <summary>
    /// Display timeline on two row 
    /// </summary>
    TwoRows
}

/// <summary>
/// LgWorkScheduler display modes
/// </summary>
public enum DisplayMode
{
    /// <summary>
    /// Bars and Milestones with project list
    /// </summary>
    Timeline,
    
    /// <summary>
    /// Calendar mode (x: hours, y: month/days or other timeline)
    /// </summary>
    Agenda
}

/// <summary>
/// Group state
/// </summary>
public enum GroupState
{
    /// <summary>
    /// The group is open
    /// </summary>
    Open,

    /// <summary>
    /// The group is close
    /// </summary>
    Close
}

/// <summary>
/// Group display mode (row or column)
/// </summary>
public enum GroupDisplayMode
{
    /// <summary>
    /// Show group as row
    /// </summary>
    Row,

    /// <summary>
    /// Shouw groups as column
    /// </summary>
    Column
}

/// <summary>
/// The drag step type.
/// </summary>
public enum DragStepType
{

    /// <summary>
    /// No step imposed during drag event
    /// </summary>
    Free = 0,

    /// <summary>
    /// During drag event taks/timeline can only be drag from hour to hour
    /// </summary>
    Hour,

    /// <summary>
    /// During drag event taks/timeline can only be drag from day to day
    /// </summary>
    Day,

    /// <summary>
    /// Use the specified time as drag step (<see cref="LgWorkScheduler{TItem}.DragStepSize"/>)
    /// </summary>
    Manual

}
