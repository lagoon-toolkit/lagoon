// Last InputBase code from :
// https://github.com/dotnet/aspnetcore/blob/88c720f125a92df225dccc4a6308a865e179e37d/src/Components/Web/src/Forms/InputBase.cs

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components.Input.Internal;

/// <summary>
/// A base class for form input components. This base class automatically
/// integrates with an <see cref="Microsoft.AspNetCore.Components.Forms.EditContext"/>, which must be supplied
/// as a cascading parameter.
/// </summary>
public abstract class LgInputBase<TValue> : LgComponentBase, ILgComponentPolicies
{
    #region fields

    private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
    private bool _hasInitializedParameters;
    private bool _previousParsingAttemptFailed;
    private ValidationMessageStore _parsingValidationMessages;
    private Type _nullableUnderlyingType;
    private string _ariaLabel;
    private bool? _isReadOnly;
    private bool? _isModelRequired;
    private bool _valueMemberInfoLoaded;
    private MemberInfo _valueMemberInfo;

    // Indicate the state of component after policies applied
    private PolicyState _policyState;

    // Indicate if the user policies must be checked
    private bool _updatePolicyState;

    /// <summary>
    /// Reset button arialabel
    /// </summary>
    private string _resetButtonAriaLabel;

    /// <summary>
    /// Reset button Text
    /// </summary>
    private string _resetButtonText;

    /// <summary>
    /// Indicates whether the reset button can be displayed.
    /// </summary>
    private bool _resetButtonEnabled;

    #endregion

    #region cascading parameters

    [CascadingParameter]
    private EditContext CascadedEditContext { get; set; }

    /// <summary>
    /// Optionnal cascading parameter provided by <see cref="LgEditForm" />
    /// </summary>
    [CascadingParameter]
    private LgEditFormConfiguration EditFormConfiguration { get; set; }

    /// <summary>
    /// Provides information about the currently authenticated user.
    /// </summary>
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; }

    /// <summary>
    /// Potential policies defined by an ancestor
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgAuthorizeView ParentPolicy { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the value of the input. This should be used with two-way binding.
    /// </summary>
    /// <example>
    /// @bind-Value="model.PropertyName"
    /// </example>
    [Parameter]
    public TValue Value { get; set; }

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets the display name for this field.
    /// <para>This value is used when generating error messages when the input value fails to parse correctly.</para>
    /// </summary>
    [Parameter] public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
    [Parameter]
    public string AriaLabel { get => _ariaLabel.CheckTranslate(); set => _ariaLabel = value; }

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
    [Parameter]
    public string AriaLabelledBy { get; set; }

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the input disabled attribute
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Get or set validator state
    /// </summary>
    /// <value>True by default</value>
    [Parameter]
    public bool IsValidatorEnabled { get; set; } = true;

    /// <summary>
    /// Policy required to view the TextBox
    /// </summary>
    [Parameter]
    public string PolicyVisible { get; set; }

    /// <summary>
    /// Policy required to edit the TextBox
    /// </summary>
    [Parameter]
    public string PolicyEdit { get; set; }

    /// <summary>
    /// Gets or sets the input readonly attribute
    /// </summary>
    [Parameter]
    public bool ReadOnly
    {
        get => _isReadOnly.HasValue && _isReadOnly.Value || !_policyState.Editable;
        set => _isReadOnly = value;
    }

    /// <summary>
    /// Get or sets if this field is mandatory. If this parameter is <c>null</c> or not defined, the
    /// value is extracted from the [Required] attribute on the binding property.
    /// </summary>
    [Parameter]
    public bool? Required { get; set; }

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

    /// <summary>
    /// Display reset button if true
    /// </summary>
    [Parameter]
    public bool? ResetButton { get; set; }

    /// <summary>
    /// Reset button text
    /// </summary>
    [Parameter]
    public string ResetText { get; set; }

    /// <summary>
    /// Reset button ariaLabel text
    /// </summary>
    [Parameter]
    public string ResetTextAriaLabel { get; set; }

    /// <summary>
    /// Reset button ariaLabel text
    /// </summary>
    [Parameter]
    public bool UseResetGridViewConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the call when the input is reset.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnReset { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets the associated <see cref="Microsoft.AspNetCore.Components.Forms.EditContext"/>.
    /// This property is uninitialized if the input does not have a parent <see cref="EditForm"/>.
    /// </summary>
    protected EditContext EditContext { get; set; }

    /// <summary>
    /// Get if the component has a validator.
    /// </summary>
    protected bool HasValidator => EditContext is not null && IsValidatorEnabled;

    /// <summary>
    /// Gets the <see cref="FieldIdentifier"/> for the bound value.
    /// </summary>
    protected internal FieldIdentifier? FieldIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the current value of the input.
    /// </summary>
    protected TValue CurrentValue
    {
        get => Value;
        set
        {
            bool hasChanged = !EqualityComparer<TValue>.Default.Equals(value, Value);
            if (hasChanged)
            {
                Value = value;
                _ = ValueChanged.TryInvokeAsync(App, Value);
                EditContext?.NotifyFieldChanged(FieldIdentifier.Value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the current value of the input, represented as a string.
    /// </summary>
    protected string CurrentValueAsString
    {
        get => FormatValueAsString(CurrentValue);
        set
        {
            _parsingValidationMessages?.Clear();

            bool parsingFailed;

            if (_nullableUnderlyingType != null && string.IsNullOrEmpty(value))
            {
                // Assume if it's a nullable type, null/empty inputs should correspond to default(T)
                // Then all subclasses get nullable support almost automatically (they just have to
                // not reject Nullable<T> based on the type itself).
                parsingFailed = false;
                CurrentValue = default!;
            }
            else if (TryParseValueFromString(value, out TValue parsedValue, out string validationErrorMessage))
            {
                parsingFailed = false;
                CurrentValue = parsedValue!;
            }
            else
            {
                parsingFailed = true;

                // EditContext may be null if the input is not a child component of EditForm.
                if (EditContext is not null)
                {
                    _parsingValidationMessages ??= new ValidationMessageStore(EditContext);
                    _parsingValidationMessages.Add(FieldIdentifier.Value, validationErrorMessage);

                    // Since we're not writing to CurrentValue, we'll need to notify about modification from here
                    EditContext.NotifyFieldChanged(FieldIdentifier.Value);
                }
            }

            // We can skip the validation notification if we were previously valid and still are
            if (parsingFailed || _previousParsingAttemptFailed)
            {
                EditContext?.NotifyValidationStateChanged();
                _previousParsingAttemptFailed = parsingFailed;
            }
        }
    }

    /// <summary>
    /// Try to parse value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parsedValue"></param>
    /// <returns></returns>
    internal virtual bool TryParseValueFromString(string value, out TValue parsedValue)
    {
        if (_nullableUnderlyingType != null && string.IsNullOrEmpty(value))
        {                
            parsedValue = default;
            return true;
        }            
        return TryParseValueFromString(value, out parsedValue, out string _);            
    }

    /// <summary>
    /// Gets or sets Previous value
    /// </summary>
    protected virtual TValue PreviousValue { get; set; }

    /// <summary>
    /// Gets a CSS class string that combines the <c>class</c> attribute and and a string indicating
    /// the status of the field being edited (a combination of "modified", "valid", and "invalid").
    /// Derived components should typically use this value for the primary HTML element's 'class' attribute.
    /// </summary>
    protected string InputCssClass => EditContext?.FieldCssClass(FieldIdentifier.Value) ?? string.Empty;

    /// <summary>
    /// Check if the user can edit the control according to the applied policy <see cref="PolicyVisible"/>
    /// </summary>
    /// <value>True if the control should be editable, false otherwise</value>
    protected bool IsVisible => _policyState.Visible;

    /// <summary>
    /// True if the field binded to the control have an [Require] attribute
    /// </summary>
    protected bool IsRequired { get; set; }

    /// <summary>
    /// Reset button arialabel
    /// </summary>
    protected string ResetButtonAriaLabel => _resetButtonAriaLabel;

    /// <summary>
    /// Indicates whether the reset button can be displayed.
    /// </summary>
    protected bool ResetButtonEnabled => _resetButtonEnabled;

    /// <summary>
    /// Reset button text
    /// </summary>
    protected string ResetButtonText => _resetButtonText;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="InputBase{TValue}"/>.
    /// </summary>
    protected LgInputBase()
    {
        _validationStateChangedHandler = OnValidateStateChanged;
    }

    #endregion

    #region methods

    /// <summary>
    /// Define if the reset button is displayed
    /// </summary>
    protected virtual bool ShowResetButton()
    {
        return _resetButtonEnabled && Value is not null ;
    }

    /// <summary>
    /// Indicate if the reset button must be shown, depending if the value is null or empty.
    /// </summary>
    /// <param name="attributeValue">The attribute value.</param>
    /// <returns><c>true</c> if the reset button must be shown, depending if the value is null or empty.</returns>
    protected bool ShowResetButton(string attributeValue)
    {
        return _resetButtonEnabled && !string.IsNullOrEmpty(attributeValue);
    }

    /// <summary>
    /// Formats the value as a string. Derived classes can override this to determine the formating used for <see cref="CurrentValueAsString"/>.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A string representation of the value.</returns>
    protected virtual string FormatValueAsString(TValue value)
    {
        return value?.ToString();
    }

    /// <summary>
    /// Parses a string to create an instance of <typeparamref name="TValue"/>. Derived classes can override this to change how
    /// <see cref="CurrentValueAsString"/> interprets incoming values.
    /// </summary>
    /// <param name="value">The string value to be parsed.</param>
    /// <param name="result">An instance of <typeparamref name="TValue"/>.</param>
    /// <param name="validationErrorMessage">If the value could not be parsed, provides a validation error message.</param>
    /// <returns>True if the value could be parsed; otherwise false.</returns>
    protected abstract bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage);

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Keep original value to cancellation
        SavePreviousValue(Value);
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Initialize the component visibility if it's depend of policies
        _updatePolicyState = InitPolicyState(ref _policyState, ParentPolicy, !string.IsNullOrEmpty(PolicyVisible), !string.IsNullOrEmpty(PolicyEdit));
    }

    ///<inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        if (_updatePolicyState)
        {
            return ((ILgComponentPolicies)this).UpdatePolicyStateAsync(AuthenticationState, _policyState);
        }
        else
        {
            return base.OnParametersSetAsync();
        }

    }

    /// <inheritdoc />
    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        if (!_hasInitializedParameters)
        {
            // This is the first run
            // Could put this logic in OnInit, but its nice to avoid forcing people who override OnInit to call base.OnInit()
            if (ValueExpression is not null)
            {
                FieldIdentifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(ValueExpression);
                if (CascadedEditContext != null)
                {
                    EditContext = CascadedEditContext;
                    EditContext.OnValidationStateChanged += _validationStateChangedHandler;
                }
            }
            _nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TValue));

            // Reset part
            ResetButton ??= !App.BehaviorConfiguration.Input.HideAlwaysResetButton;
            _resetButtonText = UseResetGridViewConfiguration ? ResetText : App.BehaviorConfiguration.Input.ResetText;
            _resetButtonAriaLabel = UseResetGridViewConfiguration ? ResetText : App.BehaviorConfiguration.Input.ResetTextAriaLabel;
            _hasInitializedParameters = true;
        }
        else if (CascadedEditContext != EditContext && ValueExpression is not null)
        {
            // Not the first run

            // We don't support changing EditContext because it's messy to be clearing up state and event
            // handlers for the previous one, and there's no strong use case. If a strong use case
            // emerges, we can consider changing this.
            throw new InvalidOperationException($"{GetType()} does not support changing the " +
                $"{nameof(Microsoft.AspNetCore.Components.Forms.EditContext)} dynamically.");
        }
        // Check if a reset button can be shown
        _resetButtonEnabled = ResetButton.Value && default(TValue) is null && !Disabled;
        // Initialise the mandatory
        if (Required.HasValue)
        {
            IsRequired = Required.Value;
        }
        else
        {
            if (!_isModelRequired.HasValue)
            {
                _isModelRequired = ValueMemberHasCustomAttribute<RequiredAttribute>();
            }
            IsRequired = _isModelRequired.Value;
        }
        UpdateAdditionalValidationAttributes();
        // For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
        return base.SetParametersAsync(ParameterView.Empty);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        // When initialization in the SetParametersAsync method fails, the EditContext property can remain equal to null
        if (EditContext is not null)
        {
            EditContext.OnValidationStateChanged -= _validationStateChangedHandler;
        }
        base.Dispose(disposing);
    }

    private void OnValidateStateChanged(object sender, ValidationStateChangedEventArgs eventArgs)
    {
        UpdateAdditionalValidationAttributes();

        StateHasChanged();
    }

    private void UpdateAdditionalValidationAttributes()
    {
        if (EditContext is null)
        {
            return;
        }

        bool hasAriaInvalidAttribute = AdditionalAttributes != null && AdditionalAttributes.ContainsKey("aria-invalid");
        if (EditContext.GetValidationMessages(FieldIdentifier.Value).Any())
        {
            if (hasAriaInvalidAttribute)
            {
                // Do not overwrite the attribute value
                return;
            }

            if (ConvertToDictionary(AdditionalAttributes, out Dictionary<string, object> additionalAttributes))
            {
                AdditionalAttributes = additionalAttributes;
            }

            // To make the `Input` components accessible by default
            // we will automatically render the `aria-invalid` attribute when the validation fails
            // value must be "true" see https://www.w3.org/TR/wai-aria-1.1/#aria-invalid
            additionalAttributes["aria-invalid"] = "true";
            additionalAttributes["aria-describedby"] = FieldIdentifier.Value.FieldName + "_error";
        }
        else if (hasAriaInvalidAttribute)
        {
            // No validation errors. Need to remove `aria-invalid` if it was rendered already

            if (AdditionalAttributes!.Count == 1)
            {
                // Only aria-invalid argument is present which we don't need any more
                AdditionalAttributes = null;
            }
            else
            {
                if (ConvertToDictionary(AdditionalAttributes, out Dictionary<string, object> additionalAttributes))
                {
                    AdditionalAttributes = additionalAttributes;
                }

                additionalAttributes.Remove("aria-invalid");
            }
        }
    }

    /// <summary>
    /// Returns a dictionary with the same values as the specified <paramref name="source"/>.
    /// </summary>
    /// <returns>true, if a new dictrionary with copied values was created. false - otherwise.</returns>
    private static bool ConvertToDictionary(IReadOnlyDictionary<string, object> source, out Dictionary<string, object> result)
    {
        bool newDictionaryCreated = true;
        if (source == null)
        {
            result = new Dictionary<string, object>();
        }
        else if (source is Dictionary<string, object> currentDictionary)
        {
            result = currentDictionary;
            newDictionaryCreated = false;
        }
        else
        {
            result = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in source)
            {
                result.Add(item.Key, item.Value);
            }
        }

        return newDictionaryCreated;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("form-group", InputCssClass, CssClass);
        builder.AddIf(Disabled, "disabled");
        builder.AddIf(ReadOnly, "form-group-ro");
    }

    /// <summary>
    /// Get the content of label's custom content.
    /// </summary>
    /// <param name="labelContent">The label custom content.</param>
    /// <returns>The content of label's custom content.</returns>
    protected RenderFragment GetLabelContent(RenderFragment labelContent)
    {
        string mandatoryLabel = GetMandatoryLabel();
        return builder =>
        {
            if (labelContent is not null)
            {
                builder.AddContent(0, labelContent);
            }
            if (!ReadOnly && !string.IsNullOrEmpty(mandatoryLabel))
            {
                builder.OpenElement(1, "span");
                builder.AddContent(2, mandatoryLabel);
                builder.CloseElement();
            }
        };
    }

    /// <summary>
    /// Return the label corresponding to the configuration
    /// </summary>
    /// <returns></returns>
    protected string GetMandatoryLabel()
    {
        return (EditFormConfiguration?.RequiredInputDisplayMode switch
        {
            RequiredInputDisplayMode.MandatoryOnly => IsRequired ? "MandatoryLabel" : null,
            RequiredInputDisplayMode.OptionalOnly => IsRequired ? null : "OptionalLabel",
            RequiredInputDisplayMode.Both => IsRequired ? "MandatoryLabel" : "OptionalLabel",
            _ => null,
        }).Translate();
    }

    /// <summary>
    /// Add aria attribute for Rgaa support
    /// </summary>
    /// <param name="builder">RenderTreeBuilder</param>
    /// <param name="firstIndex">Index of the first attribute</param>
    /// <returns>last index value</returns>
    internal virtual int RenderAccessibilityAttribute(RenderTreeBuilder builder, int firstIndex)
    {
        builder.AddAttribute(firstIndex + 1, "aria-label", AriaLabel);
        builder.AddAttribute(firstIndex + 2, "aria-labelledby", AriaLabelledBy);
        if (IsRequired)
        {
            builder.AddAttribute(firstIndex + 3, "aria-required", "true");
        }

        if (ReadOnly)
        {
            builder.AddAttribute(firstIndex + 4, "aria-readonly", "true");
        }
        return firstIndex + 5;
    }

    /// <summary>
    /// Gets if the bound property have the specified attribute.
    /// </summary>
    /// <returns><c>true</c> if the bound property have the specidfied attribute; <c>false</c> otherwise.</returns>
    /// <typeparam name="T">The type of the attribute to find.</typeparam>
    protected bool ValueMemberHasCustomAttribute<T>() where T : Attribute
    {
        return GetValueMemberCustomAttribute<T>() is not null;
    }

    /// <summary>
    /// Gets the attribute if it's bound to the property.
    /// </summary>
    /// <returns>The attribute if it's bound to the property.; <c>null</c> otherwise.</returns>
    /// <typeparam name="T">The type of the attribute to find.</typeparam>
    protected T GetValueMemberCustomAttribute<T>() where T : Attribute
    {
        if (!_valueMemberInfoLoaded)
        {
            _valueMemberInfo = GetValueMemberInfo();
            _valueMemberInfoLoaded = true;
        }
        return (T)(_valueMemberInfo?.GetCustomAttribute(typeof(T), true));
    }

    /// <summary>
    /// Get the MemberInfo of the property bound to the value of the input.
    /// </summary>
    /// <returns>The MethodInfo instance if found; otherwise return <c>null</c>.</returns>
    private MemberInfo GetValueMemberInfo()
    {
        // Inspired from https://stackoverflow.com/a/1560950/3568845
        MemberExpression body;
        if (ValueExpression?.Body is MemberExpression memberExpression)
        {
            body = memberExpression;
        }
        else
        {
            if (ValueExpression?.Body is UnaryExpression unaryExpression)
            {
                body = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                body = null;
            }
        }
        return body?.Member;
    }

    /// <summary>
    /// Keep previous value.
    /// </summary>
    /// <param name="value">The current value.</param>
    protected virtual void SavePreviousValue(TValue value)
    {
        PreviousValue = value;
    }

    #endregion

}
