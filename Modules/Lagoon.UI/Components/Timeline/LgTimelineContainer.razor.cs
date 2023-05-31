namespace Lagoon.UI.Components;


/// <summary>
/// Component used to draw a timeline with steps (clickage, validatable)
/// </summary>
public partial class LgTimelineContainer : LgComponentBase
{

    #region Parameters

    /// <summary>
    /// Get or set the child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Get or set the event called when a step is clicked
    /// </summary>
    [Parameter]
    public EventCallback<string> OnStepClicked { get; set; }

    #endregion

    #region Private vars

    // List of existing child
    private List<LgTimelineStep> _childrens = new();

    #endregion

    #region Internal methods for childrens

    /// <summary>
    /// Add a child
    /// </summary>
    /// <param name="step">Step to remove</param>
    internal void Add(LgTimelineStep step)
    {
        _childrens.Add(step);
    }

    /// <summary>
    /// Remove a child
    /// </summary>
    /// <param name="step">Step to remove</param>
    internal void Remove(LgTimelineStep step)
    {
        _childrens.Remove(step);
        StateHasChanged();
    }

    /// <summary>
    /// Return <c>true</c> if this child is the first item, <c>false</c> otherwise
    /// </summary>
    /// <param name="step">Step to test</param>
    internal bool IsFirstStep(LgTimelineStep step)
    {
        return _childrens.FirstOrDefault() == step;
    }

    /// <summary>
    /// Return <c>true</c> if this child is the last item, <c>false</c> otherwise
    /// </summary>
    /// <param name="step">Step to test</param>
    internal bool IsLastStep(LgTimelineStep step)
    {
        return _childrens.LastOrDefault() == step;
    }

    /// <summary>
    /// Return the step position (for the Grid system)
    /// </summary>
    /// <param name="step">Step for wich we want the position</param>
    /// <returns></returns>
    internal int GetStepPosition(LgTimelineStep step)
    {
        return ((_childrens.IndexOf(step) + 1) * 2) - 1;
    }

    /// <summary>
    /// Return the position of the step in the collection
    /// </summary>
    /// <param name="step">Step to check</param>
    internal int GetStepNumber(LgTimelineStep step)
    {
        return _childrens.IndexOf(step);
    }

    /// <summary>
    /// Check if the previous step is validated
    /// </summary>
    /// <param name="step">Step to check</param>
    /// <returns><c>true</c> if previous step validated, <c>false</c> is not previous step or not validated</returns>
    internal bool IsPrevisousStepValidated(LgTimelineStep step)
    {
        int pos = GetStepNumber(step) - 1;
        if (pos >= 0)
        {
            return _childrens[pos].IsValidated;
        }
        return false;
    }

    /// <summary>
    /// Check if the next step is validated
    /// </summary>
    /// <param name="step">Step to check</param>
    /// <returns><c>true</c> if the next step is validated, <c>false</c> is no next step or not validated</returns>
    internal bool IsNextStepValidated(LgTimelineStep step)
    {
        int pos = GetStepNumber(step) + 1;
        if (pos < _childrens.Count)
        {
            return _childrens[pos].IsValidated;
        }
        return false;
    }

    #endregion

    #region Event

    /// <summary>
    /// On item click fire the <see cref="OnStepClicked"/> if the item is clickable
    /// </summary>
    /// <param name="step">The clicked step</param>
    /// <returns></returns>
    internal async Task OnChildClickAsync(LgTimelineStep step)
    {
        if (OnStepClicked.HasDelegate && step.IsClickable)
        {
            await OnStepClicked.TryInvokeAsync(App, step.Id);
        }
    } 

    #endregion

}
