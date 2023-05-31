using Lagoon.UI.Components.Forms;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections;
using System.Diagnostics;

namespace Lagoon.UI.Components;

/// <summary>
/// Based on branch release/5.0 --> https://github.com/dotnet/aspnetcore/blob/release/5.0/src/Components/Web/src/Forms/EditForm.cs
/// Renders a form element that cascades an <see cref="EditContext"/> to descendants.
/// 
/// Lagoon modifications:
///     - Add DataAnnotationValidator and LgValidatorSummary in the BuildRenderTree
///     - DRAFT Add OnEditStateChanged event 
/// </summary>
public partial class LgEditForm : LgComponentBase, IFormTrackerComponent
{

    #region Private properties

    private readonly Func<Task> _handleSubmitDelegate; // Cache to avoid per-render allocations

    private EditContext _editContext;

    private bool _hasSetEditContextExplicitly;

    /// <summary>
    /// Validator to use for displaying errors message associated to 
    /// </summary>
    private LgValidator _validator;

    /// <summary>
    /// Reference to the HtmlForm element
    /// </summary>
    private ElementReference _this;

    /// <summary>
    /// Flag to handle the subscibtion to the OnValidationStateChanged event
    /// </summary>
    private bool _hasSetOnValidationStateChanged;

    #endregion

    #region Cascading Parameters

    /// <summary>
    /// Potential form tracker defined by an ancestor 
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    public LgFormTracker FormTracker { get; set; }

    #endregion

    #region Parameters

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created <c>form</c> element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Supplies the edit context explicitly. If using this parameter, do not
    /// also supply <see cref="Model"/>, since the model value will be taken
    /// from the <see cref="EditContext.Model"/> property.
    /// </summary>
    [Parameter]
    public EditContext EditContext
    {
        get => _editContext;
        set
        {
            _editContext = value;
            _hasSetEditContextExplicitly = value != null;
        }
    }

    /// <summary>
    /// Specifies the top-level model object for the form. An edit context will
    /// be constructed for this model. If using this parameter, do not also supply
    /// a value for <see cref="EditContext"/>.
    /// </summary>
    [Parameter]
    public object Model { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="EditForm"/>.
    /// </summary>
    [Parameter]
    public RenderFragment<EditContext> ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the confirmation message to show before submit.
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    /// <summary>
    /// A callback that will be invoked when the form is submitted.
    ///
    /// If using this parameter, you are responsible for triggering any validation
    /// manually, e.g., by calling <see cref="EditContext.Validate"/>.
    /// </summary>
    [Parameter]
    public EventCallback<EditContext> OnSubmit { get; set; }

    /// <summary>
    /// A callback that will be invoked when the form is submitted and the
    /// <see cref="EditContext"/> is determined to be valid.
    /// </summary>
    [Parameter]
    public EventCallback<EditContext> OnValidSubmit { get; set; }

    /// <summary>
    /// A callback that will be invoked when the form is submitted and the
    /// <see cref="EditContext"/> is determined to be invalid.
    /// </summary>
    [Parameter]
    public EventCallback<EditContext> OnInvalidSubmit { get; set; }

    /// <summary>
    /// If set replace the default error message (#lgEditFormValidation)
    /// </summary>
    [Parameter]
    public string ValidationErrorMessage { get; set; }

    /// <summary>
    /// Get or set optionnal error display option (if not set, application configuration will be used)
    /// </summary>    
    [Parameter]
    public EditFormErrorsDisplayOptions? ErrorsDisplayOptions { get; set; }

    /// <summary>
    /// Get or set the display mode of mandory and optional fields
    /// </summary>
    [Parameter]
    public RequiredInputDisplayMode? RequiredInputDisplayMode { get; set; }

    /// <summary>
    /// Active a ObjectGraphDataAnnotationsValidator instead of a simple DataAnnotationsValidator (required when you have nested properties with DataAnnotation)
    /// </summary>
    [Parameter]
    public bool? UseValidationComplexModel { get; set; }

    /// <summary>
    /// Ignore form from tracking modifications.
    /// </summary>
    [Parameter]
    public bool IgnoreFormTracking { get; set; } = false;

    /// <summary>
    /// Get or set the falg indicating if the form should be submitted when enter key is pressed. (<c>true</c> by default)
    /// </summary>
    [Parameter]
    public bool SubmitOnEnter { get; set; } = true;

    #endregion

    #region Public properties

    /// <summary>
    /// Get the validator instance to show errors manually 
    /// </summary>
    public LgValidator Validator => _validator;

    /// <summary>
    /// Event fired when the form is submitting (or has complete submission)
    /// </summary>
    public event Action<bool> OnSubmitting;

    #endregion 

    #region Initialization

    /// <summary>
    /// Constructs an instance of <see cref="EditForm"/>.
    /// </summary>
    public LgEditForm()
    {
        _handleSubmitDelegate = HandleSubmitAsync;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Add the current EditForm to the FormTracker
        FormTracker?.RegisterComponent(this);
        // If not explicitly set, initialize error display config with app configuration
        if ((ErrorsDisplayOptions is null) || (RequiredInputDisplayMode is null))
        {
            LgEditFormConfiguration config = App.BehaviorConfiguration.EditForm;
            ErrorsDisplayOptions ??= config.ErrorsDisplayOptions;

            RequiredInputDisplayMode ??= config.RequiredInputDisplayMode;
        }
        UseValidationComplexModel ??= false;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        // Remove the current EditForm from the FormTracker
        FormTracker?.UnregisterComponent(this);
        if (_editContext != null)
        {
            _editContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (_hasSetEditContextExplicitly && Model != null)
        {
            throw new InvalidOperationException($"{nameof(EditForm)} requires a {nameof(Model)} " +
                $"parameter, or an {nameof(EditContext)} parameter, but not both.");
        }
        else if (!_hasSetEditContextExplicitly && Model == null)
        {
            throw new InvalidOperationException($"{nameof(EditForm)} requires either a {nameof(Model)} " +
                $"parameter, or an {nameof(EditContext)} parameter, please provide one of these.");
        }

        // If you're using OnSubmit, it becomes your responsibility to trigger validation manually
        // (e.g., so you can display a "pending" state in the UI). In that case you don't want the
        // system to trigger a second validation implicitly, so don't combine it with the simplified
        // OnValidSubmit/OnInvalidSubmit handlers.
        if (OnSubmit.HasDelegate && (OnValidSubmit.HasDelegate || OnInvalidSubmit.HasDelegate))
        {
            throw new InvalidOperationException($"When supplying an {nameof(OnSubmit)} parameter to " +
                $"{nameof(EditForm)}, do not also supply {nameof(OnValidSubmit)} or {nameof(OnInvalidSubmit)}.");
        }

        // Update _editContext if we don't have one yet, or if they are supplying a
        // potentially new EditContext, or if they are supplying a different Model
        if (Model != null && Model != _editContext?.Model)
        {
            bool raiseFieldChange = false;
            if (_editContext != null)
            {
                _editContext.OnValidationStateChanged -= OnValidationStateChanged;
                raiseFieldChange = true;
            }

            _editContext = new EditContext(Model);
            _editContext.OnValidationStateChanged += OnValidationStateChanged;
            _hasSetOnValidationStateChanged = true;

            if (raiseFieldChange)
            {
                FormTracker?.SetExplicitModificationStateAsync();
            }
        }
        else if (!_hasSetOnValidationStateChanged)
        {
            _hasSetOnValidationStateChanged = true;
            _editContext.OnValidationStateChanged += OnValidationStateChanged;
        }
    }

    /// <summary>
    /// The model linked to the form has changed.
    /// </summary>
    private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
    {
        FormTracker?.SetExplicitModificationStateAsync(IsModified());
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!SubmitOnEnter)
        {
            // Disable form submit on enter key
            await JS.InvokeVoidAsync("Lagoon.JsUtils.DisableEnterKey", _this);
        }
    }

    #endregion

    #region IFormTrackerComponent

    /// <inheritdoc />
    public bool IsModified()
    {
        return !IgnoreFormTracking && EditContext.IsModified();
    }

    /// <inheritdoc />
    public void SetModifiedState(bool state)
    {
        if (!state)
        {
            EditContext.ResetFormTracker();
        }
    } 

    #endregion

    #region Private methods

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Debug.Assert(_editContext != null);
        LgEditFormConfiguration config = GetCascadingConfiguration();

        // If _editContext changes, tear down and recreate all descendants.
        // This is so we can safely use the IsFixed optimization on CascadingValue,
        // optimizing for the common case where _editContext never changes.
        builder.OpenRegion(_editContext.GetHashCode());

        builder.OpenElement(0, "form");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "onsubmit", _handleSubmitDelegate);
        builder.AddAttribute(3, "class", GetClassAttribute());
        builder.AddElementReferenceCapture(4, (e) => _this = e);
        builder.AddCascadingValueComponent(5, this, (RenderFragment)((mainBuilder) =>
        {
            mainBuilder.AddCascadingValueComponent(6, _editContext, (RenderFragment)((subBuilder) =>
            {
                // Add a DataAnnotationValidator
                if (UseValidationComplexModel.Value)
                {
                    subBuilder.OpenComponent<ObjectGraphDataAnnotationsValidator>(7);
                }
                else
                {
                    subBuilder.OpenComponent<DataAnnotationsValidator>(7);
                }
                subBuilder.CloseComponent();
                // Inject a cascading value with the ErrorsDisplayOptions configuration (used by LgValidationSummary & LgValidationMessage)
                subBuilder.AddCascadingValueComponent(8, config, (RenderFragment)((subSubBuilder) =>
                {
                    // To display validation message (full or partial) in a Toastr
                    subSubBuilder.OpenComponent<LgValidator>(9);
                    subSubBuilder.AddAttribute(10, nameof(LgValidator.Title), ValidationErrorMessage);
                    subSubBuilder.AddComponentReferenceCapture(11, capturedRef => _validator = (LgValidator)capturedRef​​​​​);
                    subSubBuilder.CloseComponent();
                    // Add a validation summary to display validation error message in a summary if explicitly configured (not display by default)
                    if (config.ErrorsDisplayOptions.HasFlag(EditFormErrorsDisplayOptions.Summary))
                    {
                        subSubBuilder.OpenComponent<LgValidationSummary>(12);
                        subSubBuilder.AddAttribute(13, nameof(LgValidationSummary.Title), ValidationErrorMessage);
                        subSubBuilder.CloseComponent();
                    }
                    // Add EditForm ChildContent
                    subSubBuilder.AddContent(14, ChildContent?.Invoke(_editContext));
                }));
            }));
        }));
        builder.CloseElement(); // form element
        builder.CloseRegion(); // main region
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(CssClass);
    }

    /// <summary>
    /// Get error display options => local => cascading => application configuration
    /// </summary>
    private LgEditFormConfiguration GetCascadingConfiguration()
    {
        LgEditFormConfiguration config = App.BehaviorConfiguration.EditForm;
        return new LgEditFormConfiguration()
        {
            ErrorsDisplayOptions = ErrorsDisplayOptions ?? config.ErrorsDisplayOptions,
            RequiredInputDisplayMode = RequiredInputDisplayMode ?? config.RequiredInputDisplayMode
        };
    }

    /// <summary>
    /// Check confirmation mesage before handled form submission
    /// </summary>
    private async Task HandleSubmitAsync()
    {
        if (!string.IsNullOrEmpty(ConfirmationMessage))
        {
            await App.ShowConfirmAsync(ConfirmationMessage, HandleSubmitProcessAsync);
        }
        else
        {
            await HandleSubmitProcessAsync();
        }
    }

    /// <summary>
    /// Handled form submission according to context (OnSubmit || (OnValidSubmit and OnInvalidSubmit))
    /// </summary>
    private async Task HandleSubmitProcessAsync()
    {
        Debug.Assert(_editContext != null);

        try
        {
            using (var _ = GetNewWaitingContext())
            {
                // Let children know that the form is currently submitting
                OnSubmitting?.Invoke(true);
                if (OnSubmit.HasDelegate)
                {
                    // When using OnSubmit, the developer takes control of the validation lifecycle
                    await OnSubmit.TryInvokeAsync(App, _editContext);
                }
                else
                {
                    // Otherwise, the system implicitly runs validation on form submission
                    bool isValid = _editContext.Validate(); // This will likely become ValidateAsync later
                    if (isValid && OnValidSubmit.HasDelegate)
                    {
                        await OnValidSubmit.TryInvokeAsync(App, _editContext);
                    }
                    if (!isValid && OnInvalidSubmit.HasDelegate)
                    {
                        await OnInvalidSubmit.TryInvokeAsync(App, _editContext);
                    }
                }
            }
        }
        finally
        {
            OnSubmitting?.Invoke(false);
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Clear all validation message
    /// </summary>
    /// <param name="revalidate">if true context will be validated</param>
    /// <param name="markAsUnmodified">if true all field will be marked as unmodified</param>
    public void ClearValidationMessages(bool revalidate = false, bool markAsUnmodified = false)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        object GetInstanceField(Type type, object instance, string fieldName)
        {
            var fieldInfo = type.GetField(fieldName, bindingFlags);
            return fieldInfo.GetValue(instance);
        }

        var fieldStates = GetInstanceField(typeof(EditContext), EditContext, "_fieldStates");
        var clearMethodInfo = typeof(HashSet<ValidationMessageStore>).GetMethod("Clear", bindingFlags);

        foreach (DictionaryEntry kv in (IDictionary)fieldStates)
        {
            var messageStores = GetInstanceField(kv.Value.GetType(), kv.Value, "_validationMessageStores");
            clearMethodInfo.Invoke(messageStores, null);
        }

        if (markAsUnmodified)
            EditContext.MarkAsUnmodified();

        if (revalidate)
            EditContext.Validate();
    }

    #endregion

}
