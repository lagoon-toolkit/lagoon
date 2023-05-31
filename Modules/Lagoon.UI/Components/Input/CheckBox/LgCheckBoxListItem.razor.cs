namespace Lagoon.UI.Components;

/// <summary>
/// A checkbox list item component.
/// </summary>
/// <typeparam name="TValue">The value's type.</typeparam>
public class LgCheckBoxListItem<TValue> : LgComponentBase, ILgCheckBox<TValue>
{

    #region fields

    /// <summary>
    /// The reference to the checkbox list parent.
    /// </summary>
    private LgCheckBoxList<TValue> _checkBoxList;

    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    private string _elementId = GetNewElementId();

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets the reference to the parent's checkbox list.
    /// </summary>
    /// <remarks>Serves as an entrypoint to the registration of the component in the parent's items collection.</remarks>
    [CascadingParameter]
    public LgCheckBoxList<TValue> CheckBoxList
    {
        get => _checkBoxList;
        set
        {
            if (_checkBoxList != value)
            {
                _checkBoxList = value;
                _checkBoxList.AddItem(this);
            }
        }
    }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the disabled attribute.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [Parameter]
    public TValue Value { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the tooltip.
    /// </summary>
    [Parameter]
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }


    #endregion

    #region ILgCheckBox interface

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    string ILgCheckBox<TValue>.ElementId => _elementId;

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        _checkBoxList?.RemoveItem(this);
        base.Dispose(disposing);
    }

    #endregion

}