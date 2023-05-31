namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Option bubble render
/// </summary>
/// <typeparam name="TItemValue"></typeparam>
public partial class LgOptionBubble<TItemValue> : LgComponentBase
{

    #region parameters

    /// <summary>
    /// Gets or sets the item value
    /// </summary>
    [Parameter]
    public TItemValue Value { get; set; }

    /// <summary>
    /// Gets or sets the item icon name
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the item text
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the item readonly indicator
    /// </summary>
    [Parameter]
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the item tooltip
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Fire when the user click on the remove icon
    /// </summary>
    /// <value></value>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnRemove { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Get LgSelect interface
    /// </summary>
    [CascadingParameter]
    public ILgSelect<TItemValue> ILgSelect { get; set; }

    #endregion

    #region events

    /// <summary>
    ///  Fire when the user click on the item
    /// </summary>
    /// <returns></returns>
    public Task OnClickAsync()
    {
        return ILgSelect.OnClickItemAsync(new OptionEventArgs<TItemValue>(Value));
    }

    /// <summary>
    /// Remove item event
    /// </summary>
    /// <returns></returns>
    public async Task OnRemoveAsync()
    {
        if (OnRemove.HasDelegate)
        {
            await OnRemove.TryInvokeAsync(App, new ChangeEventArgs() { Value = null });
        }
    }
    #endregion
}
