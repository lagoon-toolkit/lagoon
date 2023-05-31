namespace Lagoon.UI.Components;

/// <summary>
/// Tab informations.
/// </summary>
public interface ITab
{

    /// <summary>
    /// Identifier of the tab.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Uri of the tab.
    /// </summary>
    public string Uri { get; }

}
