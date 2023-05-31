namespace Lagoon.UI.Components;

/// <summary>
/// Calendar day click event args.
/// </summary>
public class ClickDayEventArgs : ActionEventArgs
{
    /// <summary>
    /// Datetime 
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Initialise a new argument instance.
    /// </summary>
    /// <param name="waitingContext">Waiting context.</param>
    /// <param name="date">Date of the clicked day.</param>
    public ClickDayEventArgs(WaitingContext waitingContext, DateTime date) : base(waitingContext)
    {
        Date = date;
    }

}
