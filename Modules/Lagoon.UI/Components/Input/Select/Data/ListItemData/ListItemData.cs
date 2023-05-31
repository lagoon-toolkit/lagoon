namespace Lagoon.UI.Components.Internal;

internal class ListItemData<TValue> : IListItemData<TValue>
{

    #region properties

    /// <summary>
    /// Gets the CSS classes for the item.
    /// </summary>
    public string CssClass { get; set; }

    /// <summary>
    /// Gets if the item is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets the icon name for the item.
    /// </summary>
    public string IconName { get; set; }

    /// <summary>
    /// Geth the display text for the item.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Get the tooltip for the item.
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// Get the item value.
    /// </summary>
    public TValue Value { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public ListItemData()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="item"></param>
    public ListItemData(IListItemData<TValue> item)
    {
        CssClass = item.GetCssClass();
        Disabled = item.GetDisabled();
        IconName = item.GetIconName();
        Text = item.GetText();
        Tooltip = item.GetTooltip();
        Value = item.GetValue();
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="text">Text.</param>
    public ListItemData(TValue value, string text)
    {
        Value = value;
        Text = text;
    }

    #endregion

    #region interface IListItemData

    string IListItemData<TValue>.GetCssClass()
    {
        return CssClass;
    }

    bool IListItemData<TValue>.GetDisabled()
    {
        return Disabled;
    }

    string IListItemData<TValue>.GetIconName()
    {
        return IconName;
    }

    string IListItemData<TValue>.GetText()
    {
        return Text;
    }

    string IListItemData<TValue>.GetTooltip()
    {
        return Tooltip;
    }

    TValue IListItemData<TValue>.GetValue()
    {
        return Value;
    }

    #endregion


}
