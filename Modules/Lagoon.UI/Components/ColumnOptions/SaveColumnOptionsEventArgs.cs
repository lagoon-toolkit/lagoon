namespace Lagoon.UI.Components;

/// <summary>
/// Contains save 'column options' event data.
/// </summary>
public class SaveColumnOptionsEventArgs :EventArgs
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="displayedColumnsByOrder">list of displayed Columns by order</param>
    public SaveColumnOptionsEventArgs(List<ColumnOption> displayedColumnsByOrder)
    {
        DisplayedColumnsByOrder = displayedColumnsByOrder;
    }

    /// <summary>
    /// Gets list of displayed Columns by order
    /// </summary>
    public List<ColumnOption> DisplayedColumnsByOrder { get; }
}
