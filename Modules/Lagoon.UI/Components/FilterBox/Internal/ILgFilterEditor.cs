namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Filter editor.
/// </summary>
public interface ILgFilterEditor
{

    /// <summary>
    /// Gets or sets the selected tab.
    /// </summary>
    FilterTab SelectedTab { get; set; }

    /// <summary>
    /// Gets the tabs shown in the editor.
    /// </summary>
    IEnumerable<FilterTab> Tabs { get; }

}
