using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// A checkbox component.
/// </summary>
public partial class LgCheckBox<TValue> : LgInputRenderBase<TValue>, ILgCheckBox<TValue>
{
    #region interfaces

    /// <summary>
    /// Checkbox Client ID.
    /// </summary>
    string ILgCheckBox<TValue>.ElementId => ElementId;

    /// <summary>
    /// CssClass is already rendered by LgInputRenderBase.
    /// </summary>
    string ILgCheckBox<TValue>.CssClass => null;

    #endregion

    #region fields

    /// <summary>
    /// The different states values for the supported types.
    /// </summary>
    private static readonly Dictionary<Type, Func<(TValue, TValue, TValue)>> _stateValuesDico = new()
    {
        [typeof(object)] = () => ((TValue)(object)true, (TValue)(object)false, (TValue)(object)null),
        [typeof(bool?)] = () => ((TValue)(object)true, (TValue)(object)false, (TValue)(object)null),
        [typeof(bool)] = () => ((TValue)(object)true, (TValue)(object)false, (TValue)(object)true)
    };

    /// <summary>
    /// The different state values.
    /// </summary>
    private (TValue CheckedValue, TValue UncheckedValue, TValue IndeterminateValue) _stateValues;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets an object that contains data about the control.
    /// </summary>
    [Parameter]
    public object Item { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets a callback called before the value is changed.
    /// </summary>
    [Parameter]
    public EventCallback<ChangingEventArgs<TValue>> OnChanging { get; set; }

    /// <summary>
    /// Gets or sets the flag specifying the use of the indeterminate attribute.
    /// </summary>
    [Parameter]
    public bool AllowIndeterminateState { get; set; }

    /// <summary>
    /// Gets or sets the checkbox type.
    /// </summary>
    [Parameter]
    public CheckBoxKind CheckBoxKind { get; set; }

    /// <summary>
    /// Gets or sets the label's position.
    /// </summary>
    [Parameter]
    public CheckBoxTextPosition TextPosition { get; set; }

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!_stateValuesDico.TryGetValue(typeof(TValue), out Func<(TValue, TValue, TValue)> f))
        {
            throw new InvalidOperationException("lgCheckboxErrType".Translate());
        }

        _stateValues = f();
    }

    /// <summary>
    /// Sets a JavaScript property of the HTML element.
    /// </summary>
    /// <param name="property">The property to set.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected async Task SetElementPropertyAsync<T>(string property, T value)
    {
        if (!ReadOnly)
        {
            await JS.InvokeVoidAsync("Lagoon.LgCheckBox.SetProperty", ElementRef, property, value);
        }
    }

    #endregion

    #region events

    /// <summary>
    /// Invoked when the value is changed.
    /// </summary>
    internal async Task OnValueChangeAsync()
    {            
        ChangingEventArgs<TValue> args = new(CurrentValue, ConvertedValue(), Item);
        if (OnChanging.HasDelegate)
        {
            await OnChanging.TryInvokeAsync(App, args);
        }            
        await BaseChangeValueAsync(new ChangeEventArgs { Value = args.NewValue });
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        TValue boolValue = (TValue)value;
        if (!CurrentValue.Equals(boolValue))
        {
            CurrentValue = boolValue;
        }
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        // The indeterminate UI state is only valid for bool? types.
        if (typeof(TValue) == typeof(bool?))
        {
            await SetElementPropertyAsync("indeterminate", EqualityComparer<TValue>.Default.Equals(CurrentValue, _stateValues.IndeterminateValue));
        }
    }

    #endregion

    #region render

    /// <inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        LgCssClassBuilder cssBuilderInput = new("custom-control-input", InputCssClass);

        EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
        ((ILgCheckBox<TValue>)this).OnRenderComponent(builder, ReadOnly, IsRequired,
            comparer.Equals(CurrentValue, _stateValues.CheckedValue), AdditionalAttributes,
            EventCallback.Factory.Create(this, OnValueChangeAsync),
            DisplayOrientation.Horizontal, CheckBoxKind, TextPosition, Disabled, value => { ElementRef = value; },
            AriaLabel, AriaLabelledBy);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("form-group-chk", "form-group", CssClass);
        builder.AddIf(ReadOnly, "form-group-ro");
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return false;
    }
    #endregion

    #region parsing

    /// <summary>
    /// Return value converted to the checkbox format
    /// </summary>        
    /// <returns></returns>
    private TValue ConvertedValue()
    {
        EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;            
        // We retrieve the next status according to the current one and the indeterminate setting.
        if (comparer.Equals(CurrentValue, _stateValues.CheckedValue))
        {
            return _stateValues.UncheckedValue;
        }
        else if (comparer.Equals(CurrentValue, _stateValues.UncheckedValue) && AllowIndeterminateState)
        {
            return _stateValues.IndeterminateValue;
        }
        else
        {
            return _stateValues.CheckedValue;
        }
    }

    /// <inheritdoc/>
    protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
    {
        throw new NotImplementedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }

    ///<inheritdoc/>
    internal override bool TryParseValueFromString(string value, out TValue parsedValue)
    {
        parsedValue= ConvertedValue();
        return true;            
    }

    #endregion
}