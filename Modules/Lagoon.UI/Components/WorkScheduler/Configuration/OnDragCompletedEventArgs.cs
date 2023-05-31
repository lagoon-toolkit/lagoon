namespace Lagoon.UI.Components.WorkScheduler.Configuration;


/// <summary>
/// Event used to describe the end of a Drag and Drop process
/// </summary>
/// <typeparam name="TItem">Type of item binded to the <see cref="LgWorkScheduler{TItem}"/> </typeparam>
public class OnDragCompletedEventArgs<TItem> : EventArgs
{

    /// <summary>
    /// Get the element from the datasource which has fired the moved event.
    /// Can be null if the item is comming from an external drag area (not already inside the datasource)
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Get the row identifier where the item has been dropped
    /// </summary>
    public object RowId { get; }

    /// <summary>
    /// Get the item identifier (when dragging an item outside the datasource)
    /// </summary>
    public string DroppedItemId { get; }

    /// <summary>
    /// The new start date from the moved item.
    /// </summary>
    public DateTime NewStart { get; }

    /// <summary>
    /// The new end date from the moved item. Can be null if it's an milesti=one
    /// </summary>
    public DateTime? NewEnd{ get; }

    /// <summary>
    /// Get the flag indicating if the moved item come from an existing position
    /// or an external area (which is not already inside the wks datasource)
    /// </summary>
    /// <remarks><see cref="DroppedItemId"/> to get the dropped item id</remarks>
    public bool IsNew { get; }

    /// <summary>
    /// Get or set the flag indicating if the move should be cancelled
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Initialize a new OnDragCompletedEventArgs event
    /// </summary>
    /// <param name="item">The moved item from the datasource</param>
    /// <param name="rowId">The row identifier where the item has been dropped</param>
    /// <param name="newStart">The new start position</param>
    /// <param name="newEnd">The new end position</param>
    /// <param name="isNew"><c>true</c> if this item come from the datasource, <c>false</c> if this item come from external dragging</param>
    /// <param name="droppedItemId">Identifier for an external dragged item</param>
    public OnDragCompletedEventArgs(TItem item, object rowId, DateTime newStart, DateTime? newEnd, bool isNew = false, string droppedItemId = null)
    {
        Item = item;
        RowId = rowId;
        NewStart = newStart;
        NewEnd = newEnd;
        IsNew = isNew;
        DroppedItemId = droppedItemId;
    }

}
