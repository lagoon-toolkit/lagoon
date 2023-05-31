using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Numeric input box component.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class LgNumericBox<TValue> : LgInputRenderBase<TValue>
{

    #region fields

    /// <summary>
    /// The zero value for the TValue type.
    /// </summary>
    private static TValue _zero = (dynamic)0;

    /// <summary>
    /// Gets or sets if the reset button must be shown.
    /// </summary>
    private bool _showResetButton;

    /// <summary>
    /// Gets or set if the padding method to calculate padding must be called
    /// </summary>
    private bool _initPrefixSuffixPadding;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the error message used when displaying an a parsing error.
    /// </summary>
    [Parameter] public string ParsingErrorMessage { get; set; } = "#lgNumBoxErrFormat";

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnInput { get; set; }

    /// <summary>
    /// Gets or sets if the OnChange event is disabled.
    /// </summary>
    /// <remarks>
    /// Used to prevent error when the control is removed.
    /// </remarks>
    [Parameter]
    public bool OnChangeDisabled { get; set; }

    /// <summary>
    /// Gets or sets a callback called on the onfocus event.
    /// </summary>
    [Parameter]
    public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>
    /// Gets or sets a callback called on the onkey event.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKey { get; set; }

    /// <summary>
    /// Gets or sets decimals number input format.
    /// </summary>
    [Parameter]
    public int Decimals { get; set; }

    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    [Parameter]
    public string DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the input prefix.
    /// </summary>
    [Parameter]
    public string Prefix { get; set; }

    /// <summary>
    /// Indicate if prefix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType PrefixType { get; set; }

    /// <summary>
    /// Gets or sets the input suffix.
    /// </summary>
    [Parameter]
    public string Suffix { get; set; }

    /// <summary>
    /// Indicate if suffix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType SuffixType { get; set; }

    /// <summary>
    /// To show/hide the increase/decrease button
    /// </summary>
    [Obsolete("Just define an \"IncrementStep\" different from 0 to show the increment buttons.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Parameter]
    public bool ShowIncrementButtons
    {
        get => !(IncrementStep is null || IncrementStep.Equals(_zero)) && !Disabled;
        set
        {
            if (value)
            {
                if (!ShowIncrementButtons)
                {
                    IncrementStep = (dynamic)1;
                }
            }
            else
            {
                if (ShowIncrementButtons)
                {
                    IncrementStep = _zero;
                }
            }
        }
    }

    /// <summary>
    /// Increment step applied with increase/decrease buttons.
    /// </summary>
    [Parameter]
    public TValue IncrementStep { get; set; }

    #endregion

    #region private properties

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    #endregion

    #region contructors

    /// <summary>
    /// Component intialisation
    /// </summary>
    public LgNumericBox()
    {
        Decimals = typeof(TValue).GetDefaultDecimalDigits();
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _showResetButton = ShowResetButton(GetValueAttribute(CurrentValueAsString));
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeAsync<string>("Lagoon.JsUtils.onFilterKey", ElementRef, "", "-., 1234567890");
        if (_initPrefixSuffixPadding)
        {
            JS.InvokeVoid("Lagoon.LgTextBox.InitPrefixSuffixPadding", ElementRef);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        bool hasPrefix = !string.IsNullOrEmpty(Prefix);
        bool hasSuffix = !string.IsNullOrEmpty(Suffix);
        string value = GetValueAttribute(CurrentValueAsString);
        if (!ReadOnly)
        {
            bool renderIncrement = !(IncrementStep is null || IncrementStep.Equals(_zero)) && !Disabled;
            bool _renderHasPrefix = hasPrefix;
            bool _renderHasSuffix = hasSuffix || renderIncrement || _showResetButton;
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "form-input");
            // Icon/text input prefix
            if (_renderHasPrefix)
            {
                _initPrefixSuffixPadding = true;
                builder.OpenElement(100, "span");
                builder.AddAttribute(101, "class", "form-input-prefix");
                if (hasPrefix)
                {
                    if (PrefixType == InputLabelType.IconName)
                    {
                        builder.OpenComponent<LgIcon>(200);
                        builder.AddAttribute(201, nameof(LgIcon.CssClass), "prefix");
                        builder.AddAttribute(202, nameof(LgIcon.IconName), Prefix);
                        builder.CloseComponent(); // 200
                    }
                    else
                    {
                        // We trim the prefix to allow space in read-only render
                        builder.OpenElement(200, "span");
                        builder.AddAttribute(201, "class", "prefix");
                        builder.AddContent(202, Prefix.TrimEnd());
                        builder.CloseElement(); // 200
                    }
                }
                builder.CloseElement(); // div - 100
            }
            // Icon/text input suffix
            if (_renderHasSuffix)
            {
                _initPrefixSuffixPadding = true;
                builder.OpenElement(300, "div");
                builder.AddAttribute(301, "class", "form-input-suffix");
                // reset button
                if (_showResetButton)
                {
                    builder.OpenComponent<LgResetButtonComponent>(450);
                    builder.AddAttribute(451, nameof(LgResetButtonComponent.ResetButtonText), ResetButtonText);
                    builder.AddAttribute(452, nameof(LgResetButtonComponent.ResetButtonAriaLabel), ResetButtonAriaLabel);
                    builder.AddAttribute(453, nameof(LgResetButtonComponent.CssClass), $"reset-btn");
                    builder.AddAttribute(454, nameof(LgResetButtonComponent.OnClickReset), EventCallback.Factory.Create(this, OnResetAsync));
                    builder.CloseComponent(); // 450
                }

                if (renderIncrement)
                {
                    // Btn container
                    builder.OpenElement(3000, "div");
                    builder.AddAttribute(3001, "class", "form-input-tools");
                    // Increase button
                    builder.OpenComponent<LgIcon>(3000);
                    builder.AddAttribute(3001, nameof(LgIcon.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnBtnIncreaseAsync));
                    builder.AddAttribute(3002, nameof(LgIcon.IconName), IconNames.All.CaretUpFill);
                    builder.AddAttribute(3004, nameof(LgIcon.CssClass), "ico-up");
                    builder.CloseComponent();
                    // Decrease button
                    builder.OpenComponent<LgIcon>(3005);
                    builder.AddAttribute(3006, nameof(LgIcon.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, OnBtnDecreaseAsync));
                    builder.AddAttribute(3007, nameof(LgIcon.IconName), IconNames.All.CaretDownFill);
                    builder.AddAttribute(3008, nameof(LgIcon.CssClass), "ico-down");
                    builder.CloseComponent();
                    // close Btn container
                    builder.CloseElement();
                }
                if (hasSuffix)
                {
                    if (SuffixType == InputLabelType.IconName)
                    {
                        builder.OpenComponent<LgIcon>(400);
                        builder.AddAttribute(401, nameof(LgIcon.CssClass), "suffix");
                        builder.AddAttribute(402, nameof(LgIcon.IconName), Suffix);
                        builder.CloseComponent(); // 400
                    }
                    else
                    {
                        // We trim the suffix to allow space in read-only render
                        builder.OpenElement(400, "span");
                        builder.AddAttribute(401, "class", "suffix");
                        builder.AddContent(402, Suffix.TrimEnd());
                        builder.CloseElement(); // 400
                    }
                }
                builder.CloseElement(); // div - 300
            }
            // Input part
            builder.OpenElement(500, "input");
            builder.AddAttribute(501, "type", "text");
            builder.AddMultipleAttributes(502, AdditionalAttributes);
            builder.AddAttribute(503, "placeholder", Placeholder.CheckTranslate());
            builder.AddAttribute(505, "class", "form-control ntb");
            builder.AddAttribute(506, "title", Tooltip.CheckTranslate());
            builder.AddAttribute(507, "value", value);
            if (Disabled)
            {
                builder.AddAttribute(508, "disabled", Disabled.ToString());
            }
            if (!OnChangeDisabled)
            {
                builder.AddAttribute(509, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnChangeAsync));
            }
            builder.AddAttribute(510, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnFocusAsync));
            builder.AddAttribute(511, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyAsync));
            if (ResetButtonEnabled || OnInput.HasDelegate || OnChangeDisabled)
            {
                builder.AddAttribute(512, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, OnInputAsync));
            }
            builder.AddAttribute(513, "id", ElementId);
            RenderAccessibilityAttribute(builder, 700);
            builder.AddElementReferenceCapture(514, capturedRef => ElementRef = capturedRef);
            builder.CloseElement(); // input - 500
            builder.CloseElement(); // div - 0
        }
        else
        {
            // Readonly mode
            // We don't render the icons prefix/suffix, and we concatenate the prefix, the value and the suffix
            if (!string.IsNullOrEmpty(value))
            {
                StringBuilder sbValue = null;
                if (hasPrefix && PrefixType == InputLabelType.Text)
                {
                    sbValue = new(Prefix);
                    sbValue.Append(value);
                }
                if (hasSuffix && SuffixType == InputLabelType.Text)
                {
                    sbValue ??= new(value);
                    sbValue.Append(Suffix);
                }
                if (sbValue is not null)
                {
                    value = sbValue.ToString();
                }
            }
            // Readonly render
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", $"ntb-ro");
            builder.OpenElement(2, "span");
            builder.AddMultipleAttributes(3, AdditionalAttributes);
            builder.AddAttribute(4, "title", Tooltip.CheckTranslate());
            RenderAccessibilityAttribute(builder, 100);
            builder.AddContent(5, value);
            builder.CloseElement(); // span - 2
            builder.CloseElement(); // div - 0

        }
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return !string.IsNullOrEmpty(Placeholder) || base.HasActiveLabel();
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(!HasLabel(), "input-without-lbl");
    }

    #endregion

    #region formats methods

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
    {
        if (BindConverter.TryConvertTo(Numeric.CleanNumericValue(value), CultureInfo.CurrentCulture, out result))
        {
            validationErrorMessage = null;
            return true;
        }
        else
        {
            validationErrorMessage = string.Format(ParsingErrorMessage.CheckTranslate(), FieldIdentifier?.FieldName);
            return false;
        }
    }

    /// <summary>
    /// Formats the value as a string. Derived classes can override this to determine the formating used for <c>CurrentValueAsString</c>.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A string representation of the value.</returns>
    protected override string FormatValueAsString(TValue value)
    {
        if (value is IFormattable formattableValue)
        {
            // Format with the display format and the current culture
            return formattableValue.ToString(DisplayFormat ?? $"N{Decimals:D}", null);
        }
        else
        {
            // Use the default format for the type
            return value?.ToString();
        }
    }

    #endregion

    #region Event methods        

    /// <summary>
    /// Increase current value
    /// </summary>
    private Task OnBtnIncreaseAsync()
    {
        object newValue = (dynamic)CurrentValue + (dynamic)IncrementStep;
        newValue ??= IncrementStep;
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = newValue });
    }

    /// <summary>
    /// Decrease current value
    /// </summary>
    private Task OnBtnDecreaseAsync()
    {
        object newValue = (dynamic)CurrentValue - (dynamic)IncrementStep;
        newValue ??= _zero;
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = newValue });
    }

    /// <summary>
    /// Invoked when input content change
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnInputAsync(ChangeEventArgs args)
    {
        _showResetButton = ShowResetButton((string)args.Value);
        if (OnInput.HasDelegate)
        {
            await OnInput.TryInvokeAsync(App, args);
        }
        if (OnChangeDisabled)
        {
            await BaseChangeValueAsync(args);
        }
    }

    /// <summary>
    /// Invoked when content change (lost focus)
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnChangeAsync(ChangeEventArgs args)
    {
        await BaseChangeValueAsync(args);
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value?.ToString();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked when user focus the input
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnFocusAsync(FocusEventArgs args)
    {
        if (OnFocus.HasDelegate)
        {
            await OnFocus.TryInvokeAsync(App, args);
        }
    }

    /// <summary>
    /// Invoked when user key up into the input
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnKeyAsync(KeyboardEventArgs args)
    {
        if (OnKey.HasDelegate)
        {
            await OnKey.TryInvokeAsync(App, args);
        }
    }

    /// <inheritdoc/>
    internal override Task OnResetValueAsync()
    {
        _showResetButton = false;
        CurrentValue = default;
        return BaseChangeValueAsync(new ChangeEventArgs { Value = null });
    }

    #endregion

}
