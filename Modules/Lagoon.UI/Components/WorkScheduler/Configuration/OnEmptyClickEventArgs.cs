namespace Lagoon.UI.Components;


/// <summary>
/// Event fired when a click in an empty zone on the WorkScheduler
/// </summary>
public class OnEmptyClickEventArgs
{

    /// <summary>
    /// Get row identifier
    /// </summary>
    public object RowId { get; }

    /// <summary>
    /// The date corresponding to the click position
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Initialize a new <see cref="OnEmptyClickEventArgs"/> 
    /// </summary>
    /// <param name="rowId">Row identifier</param>
    /// <param name="date">Date</param>
    public OnEmptyClickEventArgs(object rowId, DateTime date)
    {
        RowId = rowId;
        Date = date;
    }

}
