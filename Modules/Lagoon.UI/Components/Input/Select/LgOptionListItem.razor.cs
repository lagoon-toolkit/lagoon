namespace Lagoon.UI.Components;

/// <summary>
/// A checkbox list item component.
/// </summary>
/// <typeparam name="TItemValue">The value's type.</typeparam>
public partial class LgOptionListItem<TItemValue> : LgAriaComponentBase, IListItemData<TItemValue>
{
    #region fields

    private bool _isItemSelected;

    private TItemValue _oldValue;

    private bool _isInitialized;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the option label.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the option value.
    /// </summary>
    [Parameter]
    public TItemValue Value { get; set; }

    /// <summary>
    /// Gets or sets the disabled attribute.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the option icon.
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the tooltip
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }

    /// <summary>
    /// Gets or sets the option element Class css
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the option element Class css
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Get LgSelect interface
    /// </summary>
    [CascadingParameter]
    public ILgSelect<TItemValue> ILgSelect { get; set; }

    /// <summary>
    /// Guid to force OnParameterSet and update the selection.
    /// </summary>
    [CascadingParameter]
    public Guid SelectionGuid { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets if the current option is selected.
    /// </summary>
    internal bool IsSelected => _isItemSelected;

    /// <summary>
    /// Gets or sets the option element Id
    /// </summary>        
    public string OptionId { get; set; }

    #endregion

    #region interface IListItemData

    string IListItemData<TItemValue>.GetCssClass()
    {
        return CssClass;
    }

    bool IListItemData<TItemValue>.GetDisabled()
    {
        return Disabled;
    }

    string IListItemData<TItemValue>.GetIconName()
    {
        return IconName;
    }

    string IListItemData<TItemValue>.GetText()
    {
        return Text;
    }

    string IListItemData<TItemValue>.GetTooltip()
    {
        return Tooltip;
    }

    TItemValue IListItemData<TItemValue>.GetValue()
    {
        return Value;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if(ILgSelect is null)
        {
            throw new Exception($"You must specify the \"TItemValue\" for the LgOptionListItem : The \"TItemValue\" ({typeof(TItemValue).Name}) differs from the type used, in the parent select component, for the \"Value\" property.");
        }
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _isItemSelected = ILgSelect.InitItemSelection(this);
        if (_isInitialized)
        {
            if (!ILgSelect.ValueEqualityComparer.Equals(Value, _oldValue))
            {
                ILgSelect.UpdateItem(this, _oldValue, Value);
                _oldValue = Value;
            }
        }
        else
        {
            _oldValue = Value;
            _isInitialized = true;
            ILgSelect.AddItem(this);
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        ILgSelect.RemoveItem(this);
        base.Dispose(disposing);
    }

    /// <summary>
    /// Force resfresh 
    /// </summary>
    internal void Refresh()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Item is visible into dropdown
    /// </summary>
    /// <returns></returns>
    protected bool IsVisible()
    {
        return ILgSelect.IsDropdownItemVisible(this);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add(CssClass, "select-option-item", "dropdown-item");
        builder.AddIf(_isItemSelected, "select-option-selected");
        builder.AddIf(Disabled, "select-option-disabled");
    }

    #endregion

    #region events

    /// <summary>
    /// Invoked when on click item
    /// </summary>
    /// <returns></returns>
    protected async Task OnSelectOptionAsync()
    {
        if (!Disabled)
        {
            _isItemSelected = !_isItemSelected;
            await ILgSelect.SelectItemAsync(this);
        }
    }

    #endregion
}