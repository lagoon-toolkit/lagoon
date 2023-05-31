namespace Lagoon.UI.Components.Forms;


/// <summary>
/// Component which want to detect data modification with LgFormTracker parent component 
/// should implement this interface
/// </summary>
public interface IFormTrackerComponent
{

    /// <summary>
    /// Return the modification state
    /// </summary>
    /// <returns></returns>
    public bool IsModified();

    /// <summary>
    /// Explicitly set the modification state
    /// </summary>
    /// <param name="change"></param>
    public void SetModifiedState(bool change);

    /// <summary>
    /// Get or set a flag indicating if the component should care or not about data modification.
    /// </summary>
    /// <remarks>
    /// Should be a [Parameter] on implementation
    /// </remarks>
    public bool IgnoreFormTracking { get; set; }

}
