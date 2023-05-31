namespace Lagoon.UI.Components;


/// <summary>
///  This interface should be implemented by object which will be add to an LgWorkScheduler component (Lagoon.UI) 
///  with the support of Drag and Drop
/// </summary>
public interface IWorkSchedulerDraggableData : IWorkSchedulerData
{

    /// <summary>
    /// Get or set the flag to indicate if a task or milestone can be moved by drag and drop 
    /// on the horizontal axe (allow to move a Task/Milestone in the row)
    /// </summary>
    public bool AllowHorizontalMove { get; set; }

    /// <summary>
    /// Get or set the flag to indicate if a task or milestone can be moved by drag and drop 
    /// on the vertical axe (allow to move a Task/Milestone between rows)
    /// </summary>
    public bool AllowVerticalMove { get; set; }

}