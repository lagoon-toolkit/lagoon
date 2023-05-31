using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// A checkbox list component.
/// </summary>
public partial class LgCheckBoxList<TValue> : LgInputCollectionRenderBase<TValue>
{
    #region fields

    /// <summary>
    /// The collection of items.
    /// </summary>
    private readonly List<LgCheckBoxListItem<TValue>> _items = new();

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef { get; set; }

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the checkbox type.
    /// </summary>
    [Parameter]
    public CheckBoxKind CheckBoxKind { get; set; }

    /// <summary>
    /// Gets or sets the display's orientation.
    /// </summary>
    [Parameter]
    public DisplayOrientation DisplayOrientation { get; set; }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    [Parameter]
    public RenderFragment Items { get; set; }

    /// <summary>
    /// Gets or sets the label's position.
    /// </summary>
    [Parameter]
    public CheckBoxTextPosition TextPosition { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Adds an items to the collection.
    /// </summary>
    /// <param name="item">The items to add.</param>
    public void AddItem(LgCheckBoxListItem<TValue> item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void RemoveItem(LgCheckBoxListItem<TValue> item)
    {
        if (_items.Contains(item))
        {
            _items.Remove(item);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Tests whether an item is selected.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns><c>true</c> if the item is selected; otherwise, <c>false</c>.</returns>
    protected bool IsItemSelected(LgCheckBoxListItem<TValue> item)
    {
        return Value != null && Value.Contains(item.Value);
    }

    /// <summary>
    /// Selects an item.
    /// </summary>
    /// <param name="item">The item to select.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected Task SelectItemAsync(LgCheckBoxListItem<TValue> item)
    {
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = item });
    }

    ///<inheritdoc/>
    protected override async Task ChangeValueAsync(object value)
    {
        LgCheckBoxListItem<TValue> item = (LgCheckBoxListItem<TValue>)value;
        if (value is null || item.Disabled)
        {
            return;
        }
        if (CurrentValue is null)
        {
            CurrentValue = new HashSet<TValue>();
        }
        else
        {
            SavePreviousValue(CurrentValue);
        }            
        if (CurrentValue.Contains(item.Value))
        {
            CurrentValue.Remove(item.Value);                
        }
        else
        {
            CurrentValue.Add(item.Value);                
        }
        if (ValueChanged.HasDelegate)
        {
            // We only set value to null when the value is binded to don't loose reference on non binded lists
            if (CurrentValue.Count() == 0)
            {
                CurrentValue = default;
            }
            await ValueChanged.TryInvokeAsync(App, Value);
        }
        EditContext?.NotifyFieldChanged(FieldIdentifier.Value);
    }

    #endregion

    #region render

    /// <inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.AddCascadingValueComponent(1, this, Items);
            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", "form-input");
            builder.AddAttribute(4, "role", "listbox");
            int index = RenderAccessibilityAttribute(builder, 100);
            // Add attribute for validation
            if (AdditionalAttributes != null)
            {
                foreach (KeyValuePair<string, object> attribute in AdditionalAttributes)
                {
                    builder.AddAttribute(index++, attribute.Key, attribute.Value);
                }
            }
            bool firstOption = true;
            foreach (LgCheckBoxListItem<TValue> item in _items)
            {
                int sequence = 0;
                builder.OpenRegion(sequence++);
                ((ILgCheckBox<TValue>)item).OnRenderComponent(builder, ReadOnly, false,
                    IsItemSelected(item), AdditionalAttributes,
                     EventCallback.Factory.Create(this, () => SelectItemAsync(item)),
                     DisplayOrientation, CheckBoxKind, TextPosition, Disabled, value =>
                     {
                         if (firstOption)
                         {
                             firstOption = false;
                             ElementRef = value;
                         }
                     },
                     null, null);
                builder.CloseRegion();
            }
            builder.CloseElement();
        }
        else
        {
            // Readonly render
            if (Value is null)
            {
                builder.OpenElement(0, "span");
                builder.AddMultipleAttributes(2, AdditionalAttributes);
                builder.AddAttribute(4, "class", $"rbg-ro");
                RenderAccessibilityAttribute(builder, 100);
                builder.AddContent(6, "emptyReadonlyValue".Translate());
                builder.CloseElement();
            }
            else
            {
                builder.AddCascadingValueComponent(0, this, Items);
                foreach (LgCheckBoxListItem<TValue> item in _items.Where(x => Value.Contains(x.Value)))
                {
                    int sequence = 1;
                    builder.OpenRegion(sequence++);
                    builder.OpenElement(0, "span");
                    builder.AddMultipleAttributes(1, AdditionalAttributes);
                    builder.AddAttribute(2, "class", "custom-control custom-control-ro");
                    builder.AddMultipleAttributes(3, GetTooltipAttributes(item.Tooltip, item.TooltipIsHtml));
                    builder.AddAttribute(4, "aria-readonly", "true");
                    builder.AddContent(5, item.Text.CheckTranslate());
                    builder.CloseElement(); // span
                    builder.CloseRegion();
                }
            }
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("form-group-chk", "form-group", CssClass);
        builder.AddIf(ReadOnly, "form-group-ro");
        builder.AddIf(Disabled, "disabled");
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return false;
    }
    #endregion

    #region parsing

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out ICollection<TValue> result, out string validationErrorMessage)
    {
        throw new NotImplementedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }

    #endregion

}