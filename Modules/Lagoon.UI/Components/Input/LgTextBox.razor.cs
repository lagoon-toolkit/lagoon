using Lagoon.UI.Components.Input.Internal;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;

/// <summary>
/// Input text box component.
/// </summary>
public partial class LgTextBox : LgInputRenderBase<string>
{

    #region fields

    /// <summary>
    /// Allow c# invokation by JS for the current instance (onChange event is handled by DatePicker)
    /// </summary>
    private DotNetObjectReference<LgTextBox> _dotNetObjRef = null;

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
    /// Gets or sets the datalist
    /// </summary>
    [Parameter]
    public List<string> DataList { get; set; }

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
    public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    /// <summary>
    /// Gets or sets the input mask
    /// </summary>
    [Parameter]
    public string InputMask { get; set; }

    /// <summary>
    /// Gets or sets the input mask kind
    /// </summary>
    [Parameter]
    public InputMaskKind InputMaskKind { get; set; }

    /// <summary>
    /// Gets or sets the input mask
    /// </summary>
    [Parameter]
    public string InputMaskPlaceholder { get; set; }

    /// <summary>
    /// Get or set the flag indicating if value should be returned without the mask.
    /// </summary>
    /// <example> phone number with mask '06-05-06-07-08' will ne returned as '0605060708' if <c>AutoUnmask</c> is set to <c>true</c></example>
    /// <value><c>false</c> by default</value>
    [Parameter]
    public bool AutoUnmask { get; set; } = false;

    /// <summary>
    /// Get or set the flag indicating if value should be clear if mask is incomplete.
    /// </summary>
    /// <example> phone number with mask '06-05-06-07-' will ne returned as '' if <c>ClearIncomplete</c> is set to <c>true</c></example>
    /// <value><c>false</c> by default</value>
    [Parameter]
    public bool ClearIncomplete { get; set; } = false;

    /// <summary>
    /// Gets or sets input text mode.
    /// </summary>
    [Parameter]
    public TextBoxMode? TextMode { get; set; }

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
    /// Gets or sets the maximum number of characters entered
    /// </summary>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets the Datalist id.
    /// </summary>
    protected string DatalistId { get; } = GetNewElementId();

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

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (InputMaskKind != InputMaskKind.None)
        {
            _dotNetObjRef = DotNetObjectReference.Create(this);
        }
        if (MaxLength is null && ValueExpression is not null)
        {
            // Get the max length from the MaxLength attribute of the property
            MaxLength = GetValueMemberCustomAttribute<MaxLengthAttribute>()?.Length;
        }
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _showResetButton = TextMode != TextBoxMode.Password && ShowResetButton(GetValueAttribute(CurrentValue));
    }

    /// <summary> Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync  </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && InputMaskKind != InputMaskKind.None)
        {
            await JS.InvokeVoidAsync("Lagoon.LgTextBox.InitInputMask", ElementRef, _dotNetObjRef, AutoUnmask, ClearIncomplete, InputMaskKind == InputMaskKind.Email);
        }
        if (_initPrefixSuffixPadding)
        {
            JS.InvokeVoid("Lagoon.LgTextBox.InitPrefixSuffixPadding", ElementRef);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Component rendering
    /// </summary>
    /// <param name="builder">Component builder</param>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        bool hasPrefix = !string.IsNullOrEmpty(Prefix);
        bool hasSuffix = !string.IsNullOrEmpty(Suffix);
        string value = TextMode != TextBoxMode.Password ? GetValueAttribute(CurrentValue) : null;
        if (!ReadOnly)
        {
            bool _renderHasPrefix = hasPrefix;
            bool _renderHasSuffix = hasSuffix || _showResetButton;
            // The main DIV css class
            LgCssClassBuilder cssClass = new("form-input");
            cssClass.AddIf(TextMode == TextBoxMode.Multiline, "form-input-textarea");
            // The INPUT css class
            LgCssClassBuilder inputCssClass = new("form-control txtb");
            // Build the component
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", cssClass);
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
                builder.CloseElement(); // 100
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
                // suffix part
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
                builder.CloseElement(); // 300
            }

            // input / textarea
            if (TextMode == TextBoxMode.Multiline)
            {
                builder.OpenElement(500, "textarea");
            }
            else
            {
                builder.OpenElement(500, "input");
            }
            // password / text
            if (TextMode == TextBoxMode.Password)
            {
                builder.AddAttribute(501, "type", "password");
            }
            else
            {
                builder.AddAttribute(501, "type", "text");
            }
            builder.AddMultipleAttributes(502, AdditionalAttributes);
            builder.AddAttribute(503, "id", ElementId);
            builder.AddAttribute(504, "placeholder", Placeholder.CheckTranslate());
            builder.AddAttribute(505, "class", inputCssClass.ToString());
            builder.AddAttribute(506, "title", Tooltip.CheckTranslate());
            if (DataList != null)
            {
                builder.AddAttribute(507, "list", DatalistId);
            }
            // Do not fill password value
            if (value is not null)
            {
                builder.AddAttribute(508, "value", value);
            }

            if (Disabled)
            {
                builder.AddAttribute(509, "disabled", Disabled.ToString());
            }

            if (InputMaskKind != InputMaskKind.None)
            {
                string maskKind = "data-inputmask";
                string maskValue = InputMask;

                switch (InputMaskKind)
                {
                    case InputMaskKind.Mask:
                        maskValue = "'mask':'" + InputMask + "'";
                        break;
                    case InputMaskKind.Email:
                        maskValue = "'alias':'email'";
                        break;
                    case InputMaskKind.Regex:
                        maskKind = "data-inputmask-regex";
                        break;
                    default:
                        maskValue = "'mask':'" + InputMask + "'";
                        break;
                }

                if (!string.IsNullOrEmpty(InputMaskPlaceholder) && InputMaskKind != InputMaskKind.Regex)
                {
                    maskValue += ",'placeholder':'" + InputMaskPlaceholder + "'";
                }

                builder.AddAttribute(510, maskKind, maskValue);
            }
            if (!OnChangeDisabled)
            {
                builder.AddAttribute(520, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnChangeAsync));
            }
            builder.AddAttribute(521, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, OnInputAsync));
            builder.AddAttribute(522, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnFocusAsync));
            builder.AddAttribute(523, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyUpAsync));
            // Add characters limit
            if (MaxLength.HasValue)
            {
                builder.AddAttribute(524, "maxlength", MaxLength.Value);
            }
            RenderAccessibilityAttribute(builder, 600);
            builder.AddElementReferenceCapture(700, capturedRef => ElementRef = capturedRef​​​​​);
            builder.CloseElement(); // 700

            //Datalist-Autocomplete
            if (DataList != null)
            {
                builder.OpenElement(800, "datalist");
                builder.AddAttribute(801, "id", DatalistId);
                builder.OpenRegion(802);
                int index = 0;
                foreach (string item in DataList)
                {
                    builder.OpenElement(index++, "option");
                    builder.AddAttribute(index++, "value", item);
                    builder.CloseElement();
                }
                builder.CloseRegion();
                builder.CloseElement();
            }

            builder.CloseElement(); // 0
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
            builder.AddAttribute(1, "class", $"txtb-ro {InputCssClass}");
            // Readonly render
            builder.OpenElement(6, "span");
            builder.AddMultipleAttributes(7, AdditionalAttributes);
            builder.AddAttribute(8, "title", Tooltip.CheckTranslate());
            RenderAccessibilityAttribute(builder, 10);
            builder.AddElementReferenceCapture(20, capturedRef => ElementRef = capturedRef​​​​​);
            // Do not fill password value
            if (TextMode != TextBoxMode.Password)
            {
                builder.AddContent(21, !string.IsNullOrEmpty(value) ? value : "emptyReadonlyValue".Translate());
            }
            builder.CloseElement(); // span - 6
            builder.CloseElement(); // div - 0
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(!HasLabel(), "input-without-lbl");
        builder.AddIf(TextMode == TextBoxMode.Multiline, "textarea");
    }

    #endregion

    #region Inputs events

    /// <summary>
    /// Invoked when input content change
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnInputAsync(ChangeEventArgs args)
    {
        _showResetButton = TextMode != TextBoxMode.Password && ShowResetButton((string)args.Value);
        if (OnInput.HasDelegate)
        {
            await OnInput.TryInvokeAsync(App, args);
        }
        if (OnChangeDisabled || InputMaskKind != InputMaskKind.None)
        {
            await BaseChangeValueAsync(args);
        }
    }

    /// <summary>
    /// Invoked by js when inputmask is in use
    /// For input event
    /// </summary>
    /// <param name="value">New value</param>
    [JSInvokable("OnJsOnCompleteInputMaskAsync")]
    public Task OnJsOnCompleteInputMaskAsync(string value)
    {
        return OnInputAsync(new ChangeEventArgs() { Value = value });
    }

    /// <summary>
    /// Invoked by js when inputmask is in use
    /// </summary>
    /// <param name="value">New value</param>
    [JSInvokable("OnJsChangeAsync")]
    public async Task OnJsChangeAsync(string value)
    {
        // Need this condition for inputmask and avoid onchange callback
        if ((value ?? "") != (CurrentValueAsString ?? ""))
        {
            await OnChangeAsync(new ChangeEventArgs() { Value = value });
        }
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value?.ToString();
        return Task.CompletedTask;
    }

    /// <summary>
    /// invoked when content change (lost focus)
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>

    private async Task OnChangeAsync(ChangeEventArgs args)
    {
        await BaseChangeValueAsync(args);
    }

    /// <summary>
    /// Invoked when user focus the input
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnFocusAsync(FocusEventArgs args)
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
    private async Task OnKeyUpAsync(KeyboardEventArgs args)
    {
        if (OnKeyUp.HasDelegate)
        {
            await OnKeyUp.TryInvokeAsync(App, args);
        }
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return !string.IsNullOrEmpty(Placeholder) || (TextMode!=TextBoxMode.Password && !string.IsNullOrEmpty(CurrentValue));
    }

    /// <inheritdoc/>
    internal override Task OnResetValueAsync()
    {
        _showResetButton = false;
        return base.OnResetValueAsync();
    }

    #endregion

    #region Format

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out string result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Free resources
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        _dotNetObjRef?.Dispose();
        base.Dispose(disposing);
    }

    #endregion

}