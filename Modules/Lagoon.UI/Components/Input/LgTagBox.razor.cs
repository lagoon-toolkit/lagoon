using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Component to input a list of text values.
/// </summary>
public partial class LgTagBox : LgInputRenderBase<List<string>>
{

    #region fields

    private bool? _oldReadOnly = null;

    /// <summary>
    /// Reload Tagbox if true
    /// </summary>
    private bool _reload;

    /// <summary>
    /// Dispose JS indicator
    /// </summary>
    private bool _disposeJS;

    /// <summary>
    /// Current value list in string format
    /// </summary>
    private string _currentValue;

    /// <summary>
    /// Old value
    /// </summary>
    private string _oldValue;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the select placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the tag CssClass (default : "label label-info")
    /// </summary>
    [Parameter]
    public string TagCssClass { get; set; } = "label label-info";

    /// <summary>
    /// When set, no more than the given number of tags are allowed to add (default: undefined). When maxTags is reached, a class 'bootstrap-tagsinput-max' is placed on the tagsinput element.
    /// </summary>
    [Parameter]
    public int? MaxTags { get; set; }

    /// <summary>
    /// Defines the maximum length of a single tag. (default: undefined)
    /// </summary>
    [Parameter]
    public int? MaxChars { get; set; }

    /// <summary>
    /// Array of keycodes which will add a tag when typing in the input. (default: [13, 59, 44], which are ENTER, semicolon and comma).
    /// </summary>
    [Parameter]
    public List<int> ConfirmKeys { get; set; } = new List<int> { 13, 59, 44 };

    /// <summary>
    /// delimeter used to join values. (default: comma)
    /// </summary>
    [Parameter]
    public string Delimiter { get; set; } = ";";

    /// <summary>
    /// When true, the same tag can be added multiple times. (default: false)
    /// </summary>
    [Parameter]
    public bool AllowDuplicates { get; set; }

    /// <summary>
    /// When true, the input height is fixed. If there is more value tag than possible to display, an horizontal scroll bar is displayed in the tagbox.
    /// </summary>
    [Parameter]
    public bool NoWrap { get; set; }

    /// <summary>
    /// DotNet Object reference
    /// </summary>
    private DotNetObjectReference<LgTagBox> _dotNetObjRef;

    /// <summary>
    /// Gets or sets the input mask kind
    /// </summary>
    [Parameter]
    public InputMaskKind InputMaskKind { get; set; }

    /// <summary>
    /// Gets or sets the input mask
    /// </summary>
    [Parameter]
    public string InputMask { get; set; }

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
    public bool AutoUnmask { get; set; }

    /// <summary>
    /// Get or set the flag indicating if value should be clear if mask is incomplete.
    /// </summary>
    /// <example> phone number with mask '06-05-06-07-' will ne returned as '' if <c>ClearIncomplete</c> is set to <c>true</c></example>
    /// <value><c>false</c> by default</value>
    [Parameter]
    public bool ClearIncomplete { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef { get; set; }

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _dotNetObjRef = DotNetObjectReference.Create(this);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _dotNetObjRef?.Dispose();
        _disposeJS = true;
        base.Dispose(disposing);

    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _currentValue = CurrentValue is null ? null : string.Join(Delimiter, CurrentValue);
        if (_currentValue != _oldValue)
        {
            _oldValue = _currentValue;
            _reload = true;
        }
        if (_oldReadOnly.HasValue && _oldReadOnly.Value != ReadOnly)
        {
            _oldReadOnly = ReadOnly;
            _reload = true;
        }
    }

    /// <summary> 
    /// Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync  
    /// </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!ReadOnly && !_disposeJS && (firstRender || _reload))
        {
            _oldReadOnly = ReadOnly;
            _reload = false;
            await JS.InvokeVoidAsync("Lagoon.LgTagBox.Load", ElementId, _dotNetObjRef, TagCssClass, MaxTags, MaxChars, ConfirmKeys,
                Delimiter, AllowDuplicates, InputMaskKind != InputMaskKind.None, InputMaskKind, InputMaskPlaceholder, AutoUnmask, ClearIncomplete, InputMask);
        }

        if (_disposeJS)
        {
            _disposeJS = false;
            await JS.InvokeVoidAsync("Lagoon.LgTagBox.Destroy", ElementId);
        }
    }

    /// <inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.OpenElement(0, "div");
            if (NoWrap)
            {
                builder.AddAttribute(1, "class", "form-input fixed-height");
            }
            else
            {
                builder.AddAttribute(1, "class", "form-input");
            }
            builder.OpenElement(10, "input");
            builder.AddAttribute(11, "type", "text");
            builder.AddMultipleAttributes(12, AdditionalAttributes);
            builder.AddAttribute(13, "placeholder", Placeholder.CheckTranslate());
            builder.AddAttribute(15, "class", $"form-control ntb {InputCssClass}");
            builder.AddAttribute(16, "title", Tooltip.CheckTranslate());
            builder.AddAttribute(17, "value", _currentValue);
            if (Disabled)
            {
                builder.AddAttribute(18, "disabled", Disabled.ToString());
            }
            builder.AddAttribute(19, "id", ElementId);
            RenderAccessibilityAttribute(builder, 21);
            builder.AddElementReferenceCapture(100, capturedRef => ElementRef = capturedRef);
            builder.CloseElement();

            if (ShowResetButton())
            {
                builder.OpenElement(50, "span");
                builder.AddAttribute(51, "class", "form-input-suffix");
                builder.OpenComponent<LgResetButtonComponent>(52);
                builder.AddAttribute(53, nameof(LgResetButtonComponent.ResetButtonText), ResetButtonText);
                builder.AddAttribute(54, nameof(LgResetButtonComponent.ResetButtonAriaLabel), ResetButtonAriaLabel);
                builder.AddAttribute(55, nameof(LgResetButtonComponent.CssClass), "reset-btn");
                builder.AddAttribute(56, nameof(LgResetButtonComponent.OnClickReset), EventCallback.Factory.Create(this, OnResetAsync));
                builder.CloseComponent();
                builder.CloseElement();// 50
            }
            builder.CloseElement();
        }
        else
        {
            // Readonly render
            builder.OpenElement(50, "span");
            builder.AddMultipleAttributes(51, AdditionalAttributes);
            builder.AddAttribute(52, "class", "ntb-ro");
            RenderAccessibilityAttribute(builder, 53);
            builder.AddContent(54, string.IsNullOrEmpty(_currentValue) ? "emptyReadonlyValue".Translate() : _currentValue);
            builder.CloseElement();
            _disposeJS = true;
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(!HasLabel(), "input-without-lbl");
    }

    /// <inheritdoc/>
    protected override bool TryParseValueFromString(string value, out List<string> result, out string validationErrorMessage)
    {
        throw new System.InvalidOperationException();
    }

    /// <inheritdoc/>
    protected override bool ShowResetButton()
    {
        return ResetButtonEnabled && !string.IsNullOrEmpty(_currentValue);
    }

    /// <summary>
    /// Invoked from JS when the value is changed.
    /// </summary>
    /// <param name="value">The tag box value</param>
    /// <returns></returns>
    [JSInvokable]
    public Task OnValueChangeAsync(List<string> value)
    {
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = value });
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValue = (List<string>)value;
        _oldValue = string.Join(Delimiter, CurrentValue);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    internal override Task OnResetAsync()
    {
        CurrentValue = null;
        return OnResetValueAsync();
    }

    #endregion

}
