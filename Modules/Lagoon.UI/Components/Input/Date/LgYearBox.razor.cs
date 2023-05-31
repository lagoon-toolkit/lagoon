using Lagoon.UI.Components.Input.Internal;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;

/// <summary>
/// Component used to display / modify date input
/// Only used for year picker
/// </summary>
/// <typeparam name="TValue">Must be an int</typeparam>
public partial class LgYearBox<TValue> : LgInputRenderBase<TValue>
{
    #region Constants
    /// <summary>
    /// Dateformat  for datepicker librairy
    /// </summary>
    private const string DATE_FORMAT = "yyyy";

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets a callback called on the onfocus event.
    /// </summary>
    [Parameter]
    public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    #endregion

    #region fields

    private bool languageChanged = false;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    /// <summary>
    /// Allow c# invokation by JS for the current instance (onChange event is handled by DatePicker)
    /// </summary>
    private DotNetObjectReference<LgYearBox<TValue>> _dotNetObjRef;

    #endregion

    #region Initialization

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _dotNetObjRef = DotNetObjectReference.Create(this);
    }

    #endregion

    #region LgYearBox render

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "form-input");
            builder.OpenElement(2, "span");
            builder.AddAttribute(3, "class", "form-input-suffix");
            if (ShowResetButton())
            {
                builder.OpenComponent<LgResetButtonComponent>(10);
                builder.AddAttribute(11, nameof(LgResetButtonComponent.ResetButtonText), ResetButtonText);
                builder.AddAttribute(12, nameof(LgResetButtonComponent.ResetButtonAriaLabel), ResetButtonAriaLabel);
                builder.AddAttribute(13, nameof(LgResetButtonComponent.CssClass), "reset-btn");
                builder.AddAttribute(14, nameof(LgResetButtonComponent.OnClickReset), EventCallback.Factory.Create(this, OnResetAsync));
                builder.CloseComponent(); // 10
            }
            builder.OpenComponent<LgIcon>(15);
            builder.AddAttribute(16, nameof(LgIcon.IconName), IconNames.All.Calendar);
            builder.CloseComponent(); //span
            builder.CloseElement(); //div

            // Visible field to get display value
            builder.OpenElement(20, "input");
            builder.AddMultipleAttributes(21, AdditionalAttributes);
            builder.AddAttribute(22, "type", "text");
            builder.AddAttribute(23, "class", $"form-control dtb");
            builder.AddAttribute(24, "title", Tooltip.CheckTranslate());
            builder.AddAttribute(25, "placeholder", Placeholder.CheckTranslate());
            builder.AddAttribute(26, "value", Value.ToString());
            if (Disabled)
            {
                builder.AddAttribute(27, "disabled", Disabled.ToString());
            }
            builder.AddAttribute(28, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnChangeAsync));
            builder.AddAttribute(29, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnFocusAsync));
            builder.AddAttribute(30, "id", ElementId);
            RenderAccessibilityAttribute(builder, 100);
            builder.AddElementReferenceCapture(31, capturedRef => ElementRef = capturedRef​​​​​);
            builder.CloseElement(); //20
            builder.CloseElement(); // 0
        }
        else
        {
            // Readonly render
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", $"dtb-ro");

            builder.OpenElement(2, "span");
            builder.AddMultipleAttributes(3, AdditionalAttributes);
            builder.AddAttribute(4, "title", Tooltip.CheckTranslate());
            RenderAccessibilityAttribute(builder, 100);
            builder.AddContent(5, !String.IsNullOrEmpty(Value.ToString()) ? Value.ToString() : "emptyReadonlyValue".Translate());
            builder.CloseElement(); // span

            builder.CloseElement(); //div
        }
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        if (!string.IsNullOrEmpty(Placeholder))
        {
            return true;
        }
        return base.HasActiveLabel();
    }

    #endregion

    #region Parsing method

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        // Unwrap nullable types. We don't have to deal with receiving empty values for nullable
        // types here, because the underlying InputBase already covers that.
        Type targetType = Nullable.GetUnderlyingType(typeof(int)) ?? typeof(int);

        bool success;
        if (targetType == typeof(int))
        {
            success = TryParseYear(value, out result);
        }
        else
        {
            throw new InvalidOperationException($"The type '{targetType}' is not a supported int type.");
        }

        if (success)
        {
            Debug.Assert(result != null);
            validationErrorMessage = null;
            return true;
        }
        else
        {
            result = default;
            validationErrorMessage = "#lgNumBoxErrFormat";// string.Format("lgDateBoxErrType".Translate(), FieldIdentifier.FieldName);
            return false;
        }
    }

    private static bool TryParseYear(string value, [MaybeNullWhen(false)] out TValue result)
    {
        bool success = BindConverter.TryConvertToInt(value, CultureInfo.InvariantCulture, out int parsedValue);
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

    #endregion

    #region Events methods

    /// <summary>
    /// Invoked when user fill value without using datepicker
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
        string newValue = string.Empty;
        if (int.TryParse(value?.ToString(), out int parsedValue))
        {
            newValue = parsedValue.ToString();
        }
        CurrentValueAsString = newValue;
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    public override Task CancelValueActionAsync()
    {
        CurrentValue = PreviousValue;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked when user focus the input
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnFocusAsync(FocusEventArgs args)
    {
        await JS.InvokeVoidAsync("Lagoon.LgDateBox.InitDatepicker", _dotNetObjRef, ElementRef, DATE_FORMAT, DATE_FORMAT, languageChanged, CurrentValueAsString, "", "", 2);
        languageChanged = false;
        if (OnFocus.HasDelegate)
        {
            await OnFocus.TryInvokeAsync(App, args);
        }
    }

    /// <summary>
    /// Call from LgDateBox.js : get onchange value
    /// </summary>
    /// <param name="value">filled value</param>
    [JSInvokable]
    public async Task SetDateBoxValueAsync(string value)
    {
        if (value != CurrentValueAsString && int.TryParse(value, out int parsedValue))
        {
            await BaseChangeValueAsync(new ChangeEventArgs() { Value = parsedValue });
        }
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Free event handler(s)
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        // Unregister event
        _dotNetObjRef.Dispose();
        base.Dispose(disposing);
    }

    #endregion
}
