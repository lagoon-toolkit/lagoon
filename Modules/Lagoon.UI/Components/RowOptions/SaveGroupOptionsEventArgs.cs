namespace Lagoon.UI.Components;

/// <summary>
/// Contains save 'group options' event data.
/// </summary>
public class SaveGroupOptionsEventArgs : EventArgs
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="groupsByOrder">list of groups</param>
    public SaveGroupOptionsEventArgs(List<GroupOption> groupsByOrder)
    {
        GroupsByOrder = groupsByOrder;
    }

    /// <summary>
    /// Gets list of groups
    /// </summary>
    public List<GroupOption> GroupsByOrder { get; }
}
