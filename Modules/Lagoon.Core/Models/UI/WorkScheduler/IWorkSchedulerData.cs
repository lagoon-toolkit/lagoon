namespace Lagoon.UI.Components;


/// <summary>
/// This interface should be implemented by object which will be add to an LgWorkScheduler component (Lagoon.UI) 
/// </summary>
public interface IWorkSchedulerData
{

    /// <summary>
    /// The start date of a task or milestone
    /// </summary>
    /// <value></value>
    DateTime From { get; set; }

    /// <summary>
    /// The end date of a Task. Leave null to use milestone instead of task
    /// </summary>
    /// <value></value>
    DateTime? To { get; set; }


}
