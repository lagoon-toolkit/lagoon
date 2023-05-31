namespace Lagoon.UI.Components;


/// <summary>
/// Component used to allow drag / drag from the outside of an <see cref="LgWorkScheduler{TItem}"/> component
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class LgWorkSchedulerDraggableItem<TItem> : ComponentBase where TItem : IWorkSchedulerData
{

    #region Parameters

    /// <summary>
    /// Get or set the identifier of the task (it will be provided back on <see cref="LgWorkScheduler{TItem}.OnDragCompleted"/> event)
    /// </summary>
    [Parameter]
    public string Id { get; set; }

    /// <summary>
    /// Get or set the duration of the task
    /// </summary>
    [Parameter]
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Get or set an optionnal text displayed in the task to drag and drop
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Get or set an optionnal css class applied on the task to drag
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Get or set the linked <see cref="LgWorkScheduler{TItem}"/> component where the task should be dragged
    /// </summary>
    [Parameter]
    public LgWorkScheduler<TItem> WorkScheduler { get; set; }

    #endregion

    #region Initialisation

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (WorkScheduler is null)
        {
            throw new InvalidOperationException($"You must provide the {nameof(WorkScheduler)} when using {(nameof(LgWorkSchedulerDraggableItem<TItem>))}");
        }
        if (Duration is null)
        {
            throw new InvalidOperationException($"You must provide the {nameof(Duration)} when using {(nameof(LgWorkSchedulerDraggableItem<TItem>))}");
        }
        base.OnInitialized();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Return the size in pixel of the duration
    /// </summary>
    /// <returns></returns>
    private double GetHoursDuration()
    {
        return WorkScheduler.GetDurationWidth(Duration.Value.TotalHours);
    } 

    #endregion

}
