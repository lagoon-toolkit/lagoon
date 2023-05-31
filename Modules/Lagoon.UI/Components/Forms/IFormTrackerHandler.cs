namespace Lagoon.UI.Components;

/// <summary>
/// Interface to handle the modifications in FormTracker.
/// </summary>
public interface IFormTrackerHandler
{

    #region properties

    /// <summary>
    /// List of the FormTracker.
    /// </summary>
    internal List<LgFormTracker> FormTrackerList { get; }

    /// <summary>
    /// Form tracker of the previous level.
    /// </summary>
    internal IFormTrackerHandler ParentFormTracker { get; }

    #endregion

    #region methods

    /// <summary>
    /// Register a new FormTracker in the FormTrackerHandler.
    /// </summary>
    /// <param name="formTracker"></param>
    internal void RegisterFormTracker(LgFormTracker formTracker)
    {
        FormTrackerList.Add(formTracker);
        ParentFormTracker?.RegisterFormTracker(formTracker);
    }

    /// <summary>
    /// Unregister a new FormTracker in the FormTrackerHandler.
    /// </summary>
    /// <param name="formTracker"></param>
    internal void UnregisterFormTracker(LgFormTracker formTracker)
    {
        ParentFormTracker?.UnregisterFormTracker(formTracker);
        FormTrackerList.Remove(formTracker);
    }

    /// <summary>
    /// Gets if one of the FormTracker contains a pending modification.
    /// </summary>
    /// <returns></returns>
    internal bool HasModification()
    {
        foreach (LgFormTracker formTracker in FormTrackerList)
        {
            if (formTracker.IsModified())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// FormTracker state has changed, the view should be refresh
    /// (to show/hide the pending change indicator)
    /// </summary>
    void RaiseFieldChanged();

    #endregion

}
