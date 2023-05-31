using Lagoon.UI.Components.Input.Internal;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;


/// <summary>
/// Component used to display / modify time type
/// </summary>
/// <typeparam name="TValue">Must be a DateTime(?) or a TimeSpan(?)</typeparam>
public partial class LgTimeBox<TValue> : LgInputRenderBase<TValue>
{

    #region Public properties

    /// <summary>
    /// Gets or sets a callback called on the onfocus event.
    /// </summary>
    [Parameter]
    public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>
    /// Gets or sets a callback called when the time is valid and completed (the control is still focus).
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnComplete { get; set; }

    /// <summary>
    /// Gets or sets ignore seconds input format        
    /// </summary>
    [Parameter]
    public bool IgnoreSeconds { get; set; } = true;

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets if the clock picker is displayed
    /// </summary>
    [Parameter]
    public bool ShowClockPicker { get; set; } = true;

    #endregion

    #region Private properties

    /// <summary>
    /// Timme format attempt
    /// </summary>
    private static string _timeFormat;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    /// <summary>
    /// Allow c# invokation by JS for the current instance (onChange event is handled by DatePicker)
    /// </summary>
    private DotNetObjectReference<LgTimeBox<TValue>> _dotNetObjRef;

    /// <summary>
    /// Flag th handle
    /// </summary>
    private bool _shouldTriggerOnChange = false;

    #endregion

    #region Initialization

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _dotNetObjRef = DotNetObjectReference.Create(this);
        _timeFormat = "HH:mm:ss";
        if (IgnoreSeconds)
        {
            _timeFormat = "HH:mm";
        }        
        if(string.IsNullOrEmpty(Placeholder) && ShowClockPicker && IgnoreSeconds)
        {
            Placeholder = "HH:MM";
        }
    }

    #endregion

    #region LgTimeBox Render

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "form-input");

            if (ShowResetButton())
            {
                builder.OpenElement(10, "span");
                builder.AddAttribute(11, "class", "form-input-suffix");
                builder.OpenComponent<LgResetButtonComponent>(12);
                builder.AddAttribute(13, nameof(LgResetButtonComponent.ResetButtonText), ResetButtonText);
                builder.AddAttribute(14, nameof(LgResetButtonComponent.ResetButtonAriaLabel), ResetButtonAriaLabel);
                builder.AddAttribute(15, nameof(LgResetButtonComponent.CssClass), $"reset-btn");
                builder.AddAttribute(16, nameof(LgResetButtonComponent.OnClickReset), EventCallback.Factory.Create(this, OnResetAsync));
                builder.CloseComponent();
                builder.CloseElement(); // 10
            }

            builder.OpenElement(20, "input");
            builder.AddMultipleAttributes(21, AdditionalAttributes);
            builder.AddAttribute(22, "class", $"form-control ttb {CssClass}");
            builder.AddAttribute(23, "title", Tooltip.CheckTranslate());
            builder.AddAttribute(24, "placeholder", Placeholder.CheckTranslate());
            builder.AddAttribute(25, "value", GetValueAttribute(CurrentValueAsString));
            if (Disabled)
                builder.AddAttribute(26, "disabled", Disabled.ToString());
            builder.AddAttribute(27, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnFocusAsync));
            builder.AddAttribute(28, "onblur", EventCallback.Factory.Create<EventArgs>(this, OnBlurAsync));
            builder.AddAttribute(29, "id", ElementId);
            RenderAccessibilityAttribute(builder, 100);
            builder.AddElementReferenceCapture(30, capturedRef => ElementRef = capturedRef​​​​​);
            builder.CloseElement();
            builder.CloseElement(); // 0
        }
        else
        {
            // Readonly render
            builder.OpenElement(20, "span");
            builder.AddMultipleAttributes(21, AdditionalAttributes);
            builder.AddAttribute(22, "class", $"dtb-ro ");
            builder.AddAttribute(23, "title", Tooltip.CheckTranslate());
            RenderAccessibilityAttribute(builder, 100);
            builder.AddContent(24, !String.IsNullOrEmpty(FormatValueAsString(CurrentValue)) ? FormatValueAsString(CurrentValue) : "emptyReadonlyValue".Translate());
            builder.CloseElement();
        }
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        if (!String.IsNullOrEmpty(Placeholder))
        {
            return true;
        }
        return base.HasActiveLabel();
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(!HasLabel(), "input-without-lbl");
    }
    #endregion

    #region Parsing methods

    /// <inheritdoc />
    protected override string FormatValueAsString(TValue value)
    {
        return value switch
        {
            DateTime dateTimeValue => BindConverter.FormatValue(dateTimeValue, _timeFormat, CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffsetValue => BindConverter.FormatValue(dateTimeOffsetValue, "", CultureInfo.InvariantCulture),
            TimeSpan dateTimeSpanValue => BindConverter.FormatValue(new DateTime(dateTimeSpanValue.Ticks), _timeFormat, CultureInfo.InvariantCulture),
            _ => string.Empty,// Handles null for Nullable<DateTime>, etc.
        };
    }

    /// <summary>
    /// Try to convert input in DateTime / TimeSpan
    /// </summary>
    /// <param name="value">Input value</param>
    /// <param name="result">Result typed</param>
    /// <param name="validationErrorMessage">Parsing error message</param>
    /// <returns>True if succed, false otherwise</returns>
    protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
    {
        // Unwrap nullable types. We don't have to deal with receiving empty values for nullable
        // types here, because the underlying InputBase already covers that.
        var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        bool success;
        if (targetType == typeof(DateTime))
        {
            success = TryParseDateTime(CurrentValue != null ? (DateTime)(object)CurrentValue : DateTime.Now, value, out result);
        }
        else if (targetType == typeof(TimeSpan))
        {
            success = TryParseTimeSpan(value, out result);
        }
        else
        {
            throw new InvalidOperationException($"The type '{targetType}' is not a supported date type.");
        }

        if (success)
        {
            Debug.Assert(result != null);
            validationErrorMessage = null;
            return true;
        }
        else
        {
            validationErrorMessage = string.Format("lgTimeBoxErrType".Translate(), FieldIdentifier?.FieldName);
            return false;
        }
    }

    /// <summary>
    /// Try to convert string value into DateTime
    /// </summary>
    private static bool TryParseDateTime(DateTime baseDate, string value, [MaybeNullWhen(false)] out TValue result)
    {
        string strBaseDate = BindConverter.FormatValue(baseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var success = BindConverter.TryConvertToDateTime(strBaseDate + " " + value, CultureInfo.InvariantCulture, "yyyy-MM-dd" + " " + _timeFormat, out var parsedValue);
        if (success)
        {
            result = (TValue)(object)parsedValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Try Yo convert string value into TimeSpan
    /// </summary>
    private static bool TryParseTimeSpan(string value, [MaybeNullWhen(false)] out TValue result)
    {
        var success = BindConverter.TryConvertTo<TimeSpan>(value, CultureInfo.InvariantCulture, out var parsedResult);
        if (success)
        {
            result = (TValue)(object)parsedResult;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    #endregion

    #region Event methods

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if(!ReadOnly && !Disabled)
        {
            // Clock picker library doesn't support the seconds displaying and InputMask
            if (ShowClockPicker && IgnoreSeconds)
            {
                await JS.InvokeVoidAsync("Lagoon.LgTimeBox.InitTimePicker",
                    _dotNetObjRef, ElementRef, "lgTimeBoxOk".Translate(), "lgTimeBoxCancel".Translate());
            }
            else
            {
                await JS.InvokeVoidAsync("Lagoon.LgTimeBox.InitTimeMask", _dotNetObjRef, ElementRef, IgnoreSeconds);
            }
        }        
    }

    /// <summary>
    /// Call from LgTimeBox.js : get ono change value
    /// </summary>
    /// <param name="value">filled value</param>
    [JSInvokable]
    public Task SetTimeBoxValueAsync(string value)
    {
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = value });
    }

    ///<inheritdoc/>
    protected override async Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value?.ToString();
        _shouldTriggerOnChange = true;
        if (OnComplete.HasDelegate)
        {
            await OnComplete.TryInvokeAsync(App, new ChangeEventArgs() { Value = value });
        }
    }

    ///<inheritdoc/>
    public override Task CancelValueActionAsync()
    {
        return ChangeValueAsync(FormatValueAsString(PreviousValue));
    }

    /// <summary>
    /// Invoked when user focus the input
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    internal async Task OnFocusAsync(FocusEventArgs args)
    {
        //await JS.InvokeVoidAsync("Lagoon.LgTimeBox.InitTimeMask", _dotNetObjRef, ElementRef, IgnoreSeconds);
        if (OnFocus.HasDelegate)
        {
            await OnFocus.TryInvokeAsync(App, args);
        }
    }

    /// <summary>
    /// When the control loose focus, trigger on change event if the value has change
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    internal async Task OnBlurAsync(EventArgs args)
    {
        if (_shouldTriggerOnChange && OnChange.HasDelegate)
        {
            await OnChange.TryInvokeAsync(App, new ChangeEventArgs() { Value = CurrentValueAsString });
        }
        _shouldTriggerOnChange = false;
    }

    #endregion

    #region Dispose

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        _dotNetObjRef.Dispose();
        base.Dispose(disposing);
    }

    #endregion

}
