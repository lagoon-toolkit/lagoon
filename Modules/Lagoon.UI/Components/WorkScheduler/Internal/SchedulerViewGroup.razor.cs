namespace Lagoon.UI.Components.WorkScheduler.Internal;


/// <summary>
/// Internal component used to draw the Schedule area
/// </summary>
/// <typeparam name="TItem">Data item type</typeparam>
public partial class SchedulerViewGroup<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    /// <summary>
    /// Get or set the parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    /// <summary>
    /// Get or set the current grouping level
    /// </summary>
    [Parameter]
    public int GroupByLevel { get; set; }

    /// <summary>
    /// Get or set the current group key
    /// </summary>
    [Parameter]
    public string GroupKey { get; set; }

    /// <summary>
    /// Current group. If null Items will be used as first grouping element
    /// </summary>
    [Parameter]
    public IGrouping<object, TItem> CurrentGroup { get; set; }

    /// <summary>
    /// Shortcut for LgWorkScheduler RowHeight
    /// </summary>
    private int RowHeight => WorkScheduler.RowHeight;

}
