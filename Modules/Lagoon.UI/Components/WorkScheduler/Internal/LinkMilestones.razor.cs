namespace Lagoon.UI.Components.WorkScheduler.Internal;


/// <summary>
/// Component used to draw a line between the first and last milestones
/// </summary>
/// <typeparam name="TItem">Data type</typeparam>
public partial class LinkMilestones<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    /// <summary>
    /// Get or set the parent component
    /// </summary>
    [CascadingParameter]
    private LgWorkScheduler<TItem> WorkScheduler { get; set; }

    /// <summary>
    /// Current row data used to extract first and last milestones
    /// </summary>
    [Parameter]
    public IEnumerable<TItem> Data { get; set; }

}
