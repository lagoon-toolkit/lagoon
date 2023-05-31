namespace Lagoon.UI.Components.WorkScheduler.Internal;


/// <summary>
/// Internal component for the LgWorkScheduler used to draw row data
/// </summary>
public partial class SchedulerViewItems<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    /// <summary>
    /// Get or set the parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    /// <summary>
    /// Get or set the data to render
    /// </summary>
    /// <value></value>
    [Parameter]
    public IEnumerable<IGrouping<object, TItem>> Data { get; set; }

    // Shortcut for LgWorkScheduler RowHeight
    private int RowHeight => WorkScheduler.RowHeight;

}