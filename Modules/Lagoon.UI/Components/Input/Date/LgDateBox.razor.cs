using Lagoon.UI.Components.Input.Internal;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Lagoon.UI.Components;


/// <summary>
/// Component used to display / modify date type
/// </summary>
/// <typeparam name="TValue">Must be a DateTime (or DateTime?)</typeparam>
public partial class LgDateBox<TValue> : LgInputRenderBase<TValue>
{
    #region Constants

    /// <summary>
    /// Date format CurrentValue
    /// </summary>
    private const string C_S_DATE_FORMAT = "yyyy-MM-dd";

    /// <summary>
    /// Dateformat  for datepicker librairy
    /// </summary>
    private const string C_S_DATE_FORMAT_JS = "yyyy-mm-dd";

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets complete input format
    /// </summary>
    [Parameter]
    public string DisplayFormat { get; set; } = null;

    /// <summary>
    /// Gets or sets a callback called on the onfocus event.
    /// </summary>
    [Parameter]
    public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>
    /// Gets or sets if the OnChange event is disabled.
    /// </summary>
    /// <remarks>
    /// Used to prevent error when the control is removed.
    /// </remarks>
    [Parameter]
    public bool OnChangeDisabled { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the latest date that may be selected; all later dates will be disabled.
    /// </summary>
    [Parameter]
    public DateTime? Max { get; set; }

    /// <summary>
    /// Gets or sets the earliest date that may be selected; all earlier dates will be disabled.
    /// </summary>
    [Parameter]
    public DateTime? Min { get; set; }

    /// <summary>
    /// Get or sets the default display of the datebox
    /// </summary>
    [Parameter]
    public DateBoxKind DateBoxKind { get; set; } = DateBoxKind.Day;

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    #endregion

    #region fields

    private bool languageChanged = false;

    /// <summary>
    /// Date format for display
    /// </summary>
    private string DateDisplayFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToString();

    /// <summary>
    /// Datepickerformat attempt
    /// </summary>
    private string DatePickerFormat;

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    /// <summary>
    /// Display value into input.
    /// </summary>
    private string DisplayValueAsString => FormatValueAsDisplayString(CurrentValue);

    /// <summary>
    /// Allow c# invokation by JS for the current instance (onChange event is handled by DatePicker)
    /// </summary>
    private DotNetObjectReference<LgDateBox<TValue>> _dotNetObjRef;

    /// <summary>
    /// Used to detect if the date has been changed directly by the developper without using the datebox ui
    /// (bind value updated directly)
    /// </summary>
    private string _currentValueAsString;

    #endregion

    #region Initialization

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        _dotNetObjRef = DotNetObjectReference.Create(this);

        if (DisplayFormat != null)
        {
            DateDisplayFormat = DisplayFormat;
        }

        //  Date formmat for datepickerformat (not the same)
        DatePickerFormat = DateDisplayFormat.ToLower();
        if (DateDisplayFormat.Contains("MMM"))
        {
            DatePickerFormat = DateDisplayFormat.Replace("MMM", "M");
        }

        _currentValueAsString = CurrentValueAsString;

        if (DateBoxKind == DateBoxKind.Month)
        {
            ClearFormatMonthPicker();
        }
    }


    #endregion

    #region LgDateTextBox Render

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        string value = DateBoxKind == DateBoxKind.Week ? GetWeekNumber(_currentValueAsString) : DisplayValueAsString;

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
            builder.AddAttribute(26, "value", value);
            if (Disabled)
            {
                builder.AddAttribute(27, "disabled", Disabled.ToString());
            }
            if (OnChangeDisabled)
            {
                builder.AddAttribute(28, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, OnInputAsync));
            }
            else
            {
                builder.AddAttribute(29, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnChangeAsync));
            }
            builder.AddAttribute(30, "onfocus", EventCallback.Factory.Create<FocusEventArgs>(this, OnFocusAsync));
            builder.AddAttribute(31, "id", ElementId);
            RenderAccessibilityAttribute(builder, 100);
            builder.AddElementReferenceCapture(32, capturedRef => ElementRef = capturedRef​​​​​);
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
            builder.AddContent(5, !String.IsNullOrEmpty(DisplayValueAsString) ? value : "emptyReadonlyValue".Translate());
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

    #region Parsing methods

    /// <inheritdoc />
    protected override string FormatValueAsString(TValue value)
    {
        return value switch
        {
            DateTime dateTimeValue => BindConverter.FormatValue(dateTimeValue, C_S_DATE_FORMAT, CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffsetValue => BindConverter.FormatValue(dateTimeOffsetValue, C_S_DATE_FORMAT, CultureInfo.InvariantCulture),
            _ => string.Empty,// Handles null for Nullable<DateTime>, etc.
        };
    }

    private string FormatValueAsDisplayString(TValue value)
    {
        return value switch
        {
            DateTime dateTimeValue => BindConverter.FormatValue(dateTimeValue, DateDisplayFormat, CultureInfo.CurrentCulture),
            DateTimeOffset dateTimeOffsetValue => BindConverter.FormatValue(dateTimeOffsetValue, DateDisplayFormat, CultureInfo.CurrentCulture),
            _ => string.Empty,
        };
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        // Unwrap nullable types. We don't have to deal with receiving empty values for nullable
        // types here, because the underlying InputBase already covers that.
        Type targetType = Nullable.GetUnderlyingType(typeof(DateTime)) ?? typeof(DateTime);

        bool success;
        if (targetType == typeof(DateTime))
        {
            success = TryParseDateTime(value, out result);
        }
        else if (targetType == typeof(DateTimeOffset))
        {
            success = TryParseDateTimeOffset(value, out result);
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
            result = default;
            validationErrorMessage = "#lgDateBoxErrType";// string.Format("lgDateBoxErrType".Translate(), FieldIdentifier.FieldName);
            return false;
        }
    }

    /// <summary>
    /// String to DateTime
    /// </summary>
    /// <param name="value">String value</param>
    /// <param name="result">Parsed DateTime value</param>
    /// <returns></returns>
    private static bool TryParseDateTime(string value, [MaybeNullWhen(false)] out TValue result)
    {
        bool success = BindConverter.TryConvertToDateTime(value, CultureInfo.InvariantCulture, C_S_DATE_FORMAT, out DateTime parsedValue);
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
    /// String to DateTimeOffset
    /// </summary>
    /// <param name="value">String value</param>
    /// <param name="result">Parsed DateTimeOffset value</param>
    /// <returns></returns>
    private static bool TryParseDateTimeOffset(string value, [MaybeNullWhen(false)] out TValue result)
    {
        bool success = BindConverter.TryConvertToDateTimeOffset(value, CultureInfo.InvariantCulture, C_S_DATE_FORMAT, out DateTimeOffset parsedValue);
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
    /// Clear Diplay format and Datepicker format for month picker
    /// </summary>
    private void ClearFormatMonthPicker()
    {
        DateDisplayFormat = Regex.Replace(DateDisplayFormat, "([. /-]+d+)|(d+[. /-]+)", "", RegexOptions.IgnoreCase);
        DatePickerFormat = Regex.Replace(DatePickerFormat, "([. /-]+d+)|(d+[. /-]+)", "", RegexOptions.IgnoreCase);
    }


    /// <summary>
    /// Get the first day according to the selected value
    /// </summary>
    /// <param name="value">The value selected</param>
    /// <returns>The first day of the week</returns>
    private static DateTime? TryGetFirstDayOfWeek(string value)
    {
        bool success = BindConverter.TryConvertToDateTime(value, CultureInfo.InvariantCulture, C_S_DATE_FORMAT, out DateTime parsedValue);
        if (success)
        {
            int diff = (7 + (parsedValue.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime firstDayOfWeek = parsedValue.AddDays(-1 * diff).Date;
            return firstDayOfWeek;
        }

        return null;
    }

    /// <summary>
    /// Number of the week in the year having 4 days and starting on a Monday.
    /// https://stackoverflow.com/questions/12196714/bug-in-weeknumber-calculation-net
    /// </summary>
    /// <param name="value">Date for which to find the week.</param>
    /// <returns>The number of the week in the year.</returns>
    private static string GetWeekNumber(string value)
    {
        DateTime? date = TryGetFirstDayOfWeek(value);
        if (date is not null)
        {
            int dayOfWeek = (int)((DateTime)date).DayOfWeek; // normally Monday
            var l_o_th = ((DateTime)date).AddDays((7 + 4 - dayOfWeek) % 7);
            DateTime l_o_yth = new(l_o_th.Year, 1, 1);
            l_o_yth = l_o_yth.AddDays((7 + 4 - (int)l_o_yth.DayOfWeek) % 7);

            return $"{(l_o_th - l_o_yth).TotalDays / 7 + 1}/{l_o_yth.Year}";
        }

        return "";
    }

    #endregion

    #region Event methods

    /// <summary>
    /// Call from LgDateBox.js : get onchange value
    /// </summary>
    /// <param name="value">filled value</param>
    [JSInvokable]
    public async Task SetDateBoxValueAsync(string value)
    {
        if (DateBoxKind == DateBoxKind.Week)
        {
            var date = TryGetFirstDayOfWeek(value);
            if (date is not null)
            {
                value = ((DateTime)date).ToString(C_S_DATE_FORMAT);
            }
        }
        _currentValueAsString = value;
        if (!_currentValueAsString.Equals(CurrentValueAsString))
        {
            await BaseChangeValueAsync(new ChangeEventArgs() { Value = value });
        }
    }

    /// <summary>
    /// Gets the week number based on the date sent
    /// </summary>
    /// <param name="date">Selected date (yyyy-MM-dd)</param>
    /// <returns></returns>
    [JSInvokable]
    public Task<string> GetWeekNumberAsync(string date)
    {
        return Task.FromResult(GetWeekNumber(date));
    }

    /// <summary>
    /// Invoked when user fill value without using datepicker
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private Task OnChangeAsync(ChangeEventArgs args)
    {
        return SetCurrentValueAsync(args);
    }

    /// <summary>
    /// Invoked when input content change
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private Task OnInputAsync(ChangeEventArgs args)
    {
        return SetCurrentValueAsync(args);
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value?.ToString();
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    public override Task CancelValueActionAsync()
    {            
        CurrentValue = PreviousValue;            
        return Task.CompletedTask;
    }

    /// <summary>
    /// Change the current value.
    /// </summary>
    /// <param name="args">The new value.</param>
    private async Task SetCurrentValueAsync(ChangeEventArgs args)
    {
        // TODO the user enters only the month or the week (display) => CurrentValueAsString
        var value = args.Value;
        string newValue = null;
        RawDate magicRawcDate = Date.MagicStrToDate(value?.ToString(), false);
        if (magicRawcDate.IsValid() && !magicRawcDate.IsEmpty)
        {
            DateTime newDate = new(magicRawcDate.Year, magicRawcDate.Month, magicRawcDate.Day);
            newValue = BindConverter.FormatValue(newDate, C_S_DATE_FORMAT, CultureInfo.InvariantCulture);
        }
        else
        {
            newValue = "";
        }
        await BaseChangeValueAsync(new ChangeEventArgs { Value = newValue });
    }

    /// <summary>
    /// Invoked when user focus the input
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnFocusAsync(FocusEventArgs args)
    {
        // Date picker limits
        string minDate = "", maxDate = "";
        if (Min.HasValue)
        {
            minDate = BindConverter.FormatValue(Min.Value, C_S_DATE_FORMAT, CultureInfo.InvariantCulture);
        }
        if (Max.HasValue)
        {
            maxDate = BindConverter.FormatValue(Max.Value, C_S_DATE_FORMAT, CultureInfo.InvariantCulture);
        }
        // Init & Display datepicker on the hidden input with attempt format test script
        await JS.InvokeVoidAsync("Lagoon.LgDateBox.InitDatepicker", _dotNetObjRef, ElementRef, C_S_DATE_FORMAT_JS, DatePickerFormat, languageChanged, CurrentValueAsString ?? "", minDate, maxDate, DateBoxKind);
        languageChanged = false;
        if (OnFocus.HasDelegate)
        {
            await OnFocus.TryInvokeAsync(App, args);
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
