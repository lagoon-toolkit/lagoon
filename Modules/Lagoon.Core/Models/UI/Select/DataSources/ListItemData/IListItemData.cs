namespace Lagoon.UI.Components;

/// <summary>
/// Interface to read informations about a list item in a list component.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IListItemData<TValue>
{

    #region methods

    /// <summary>
    /// Gets the CSS classes for the item.
    /// </summary>
    string GetCssClass();

    /// <summary>
    /// Gets if the item is disabled.
    /// </summary>
    bool GetDisabled();

    /// <summary>
    /// Gets the icon name for the item.
    /// </summary>
    string GetIconName();
    
    /// <summary>
    /// Geth the display text for the item.
    /// </summary>
    string GetText();

    /// <summary>
    /// Get the tooltip for the item.
    /// </summary>
    string GetTooltip();

    /// <summary>
    /// Get the item value.
    /// </summary>
    TValue GetValue();

    #endregion

}
