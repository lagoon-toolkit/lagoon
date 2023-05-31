namespace Lagoon.UI.Components;

/// <summary>
/// Simple implementation of IListItemData with only "Text" and "Value" needed.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface ISimpleListItemData<TValue> : IListItemData<TValue>
{

    /// <summary>
    /// Gets the CSS classes for the item.
    /// </summary>
    new string GetCssClass => null;

    /// <summary>
    /// Gets if the item is disabled.
    /// </summary>
    new bool GetDisabled => false;

    /// <summary>
    /// Gets the icon name for the item.
    /// </summary>
    new string GetIconName => null;

    /// <summary>
    /// Get the tooltip for the item.
    /// </summary>
    new string GetTooltip => null;

}

