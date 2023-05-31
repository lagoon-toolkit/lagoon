namespace Lagoon.UI.Components.Input.Internal;

/// <summary>
/// Add label management to inputbase component.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract class LgInputRenderBase<TValue> : LgInputBase<TValue>, IInputCommonProperties, IInputBusy
{

    #region fields

    private bool _isBusy;

    #endregion

    #region cascading parameter

    /// <summary>
    /// Gets or sets input reference to manage cancellation/confirmation
    /// </summary>
    [CascadingParameter]
    private InputBusyReference InputBusyReference { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    protected string ElementId { get; } = GetNewElementId();

    /// <summary>
    /// Gets the element reference for the focus.
    /// </summary>
    protected abstract ElementReference FocusElementRef { get; }

    /// <summary>
    /// Gets if the component is busy.
    /// </summary>
    bool IInputBusy.IsBusy => _isBusy;

    /// <summary>
    /// Gets or sets the label
    /// </summary>
    [Parameter]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the render fragment to customize the label part
    /// </summary>
    [Parameter]
    public RenderFragment LabelContent { get; set; }

    /// <summary>
    /// Gets or sets if the component received the focus on first render
    /// </summary>
    [Parameter]
    public bool AutoFocus { get; set; }

    /// <summary>
    /// Gets or sets if the component display confirmation modal with the message
    /// </summary>
    [Parameter]
    public string ConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// Gets or sets a callback call before OnChange
    /// </summary>
    [Parameter]
    public Func<TValue, TValue, Task<bool>> ValueChangeCallback { get; set; }

    /// <summary>
    /// Gets or sets a callback when value changed is cancelled.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnCancel { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Keep the reference to manage confirmation and cancellation in the GridView
        if (InputBusyReference is not null && InputBusyReference.ExpectedInputType == GetType())
        {
            InputBusyReference.Reference = this;
        }
        //Element en cours de modification
    }

    /// <summary>
    /// Children render
    /// </summary>
    /// <param name="builder">RenderTreeBuilder to use to create children component</param>
    protected abstract void OnRenderComponent(RenderTreeBuilder builder);

    ///<inheritdoc/>
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return firstRender && AutoFocus ? OnAutoFocusAsync() : Task.CompletedTask;
    }

    /// <summary>
    /// Method called when the automatic focus happend.
    /// </summary>
    protected virtual async Task OnAutoFocusAsync()
    {
        await JS.FocusAsync(FocusElementRef);
    }

    /// <summary>
    /// Build a basic structure for children component with :
    /// - Title
    /// - ChildrenContent
    /// - Validator
    /// </summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Don't render any content if not visible
        if (!IsVisible)
        {
            return;
        }
        // Div containing the label, the derived component and the validator
        builder.OpenElement(1, "div");
        builder.AddAttribute(2, "class", GetClassAttribute());
        builder.AddMultipleAttributes(3, GetAdditionalAttributes());
        // Delegate inherited component rendering
        builder.OpenRegion(100);
        OnRenderComponent(builder);
        builder.CloseRegion();
        // Add title
        if (LabelContent != null || !string.IsNullOrEmpty(Label))
        {
            builder.OpenComponent<LgLabel>(201);
            builder.AddAttribute(202, nameof(LgLabel.Text), Label);
            builder.AddAttribute(203, nameof(LgLabel.For), ElementId);
            builder.AddAttribute(204, nameof(LgLabel.CssClass), GetLabelClassAttribute());
            RenderFragment labelContent = GetLabelContent(LabelContent);
            if (labelContent is not null)
            {
                builder.AddAttribute(205, nameof(LgLabel.ChildContent), labelContent);
            }
            builder.CloseComponent();
        }
        // Add validator
        if (HasValidator)
        {
            builder.OpenComponent<LgValidationMessage<TValue>>(301);
            builder.AddAttribute(302, nameof(LgValidationMessage<TValue>.For), ValueExpression);
            builder.CloseComponent();
        }
        // Close the main div
        builder.CloseElement();
    }

    /// <summary>
    /// Check if label must have lblActive class
    /// </summary>
    /// <returns></returns>
    protected virtual bool HasActiveLabel()
    {
        return !string.IsNullOrEmpty(CurrentValueAsString);
    }

    /// <summary>
    /// Check if label must have lbl value
    /// </summary>
    /// <returns></returns>
    protected virtual bool HasLabel()
    {
        return !string.IsNullOrEmpty(Label);
    }

    /// <summary>
    /// Get label css class
    /// </summary>
    /// <returns></returns>
    private string GetLabelClassAttribute()
    {
        LgCssClassBuilder builder = new();
        builder.AddIf(HasActiveLabel(), "lblActive");
        return builder.ToString();
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(AdditionalAttributes, GetTooltipAttributes(Tooltip, TooltipIsHtml));
    }

    internal virtual async Task OnResetAsync()
    {
        await OnResetValueAsync();

        if (OnReset.HasDelegate)
        {
            await OnReset.TryInvokeAsync(App, new ChangeEventArgs() { Value = Value });
        }
    }

    /// <summary>
    /// Call during onclick reset button
    /// </summary>
    /// <returns></returns>
    internal virtual Task OnResetValueAsync()
    {
        CurrentValue = default;
        return BaseChangeValueAsync(new ChangeEventArgs { Value = default });
    }

    /// <summary>
    /// Change value method
    /// </summary>
    /// <param name="value">updated value</param>
    protected abstract Task ChangeValueAsync(object value);

    /// <summary>
    /// invoked when content change (lost focus)
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async Task BaseChangeValueAsync(ChangeEventArgs args)
    {
        // Post change callback
        if (ValueChangeCallback is not null)
        {
            bool isValid = TryParseValueFromString(args.Value?.ToString(), out TValue newValue);
            if (isValid && !await ValueChangeCallback.Invoke(newValue, Value))
            {
                RestoreOriginalValue();
                return;
            }
        }
        if (string.IsNullOrEmpty(ConfirmationMessage))
        {
            await ChangeValueAsync(args.Value);
            if (OnChange.HasDelegate)
            {
                await OnChange.TryInvokeAsync(App, args);
            }
        }
        else
        {
            _isBusy = true;
            await App.ShowConfirmAsync(ConfirmationMessage.Translate(args.Value?.ToString()),
                async () =>
                {
                    await ChangeValueAsync(args.Value);
                    if (OnChange.HasDelegate)
                    {
                        await OnChange.TryInvokeAsync(App, args);
                    }
                    await JS.FocusAsync(FocusElementRef, 200);
                    _isBusy = false;
                },
                async () =>
                {
                    RestoreOriginalValue();
                    await JS.FocusAsync(FocusElementRef, 200);
                    _isBusy = false;
                });
        }
    }

    /// <summary>
    /// Indicate if the component is in an cancel action mode
    /// </summary>
    private bool _isCancelAction;

    /// <summary>
    /// Restore DOM original value
    /// </summary>
    protected virtual void RestoreOriginalValue()
    {
        _isCancelAction = true;
        StateHasChanged();
        _isCancelAction = false;
        StateHasChanged();
    }

    /// <summary>
    /// Get value for the html value attribute. 
    /// Used in the cancel action.
    /// </summary>
    /// <param name="returnValue"></param>
    /// <returns></returns>
    protected string GetValueAttribute(string returnValue)
    {
        // Always return null if the action is cancelling
        return _isCancelAction ? null : returnValue;
    }

    /// <summary>
    /// Call cancellation
    /// </summary>
    /// <returns></returns>
    public async Task CancelValueAsync()
    {       
        await CancelValueActionAsync();
        if (OnCancel.HasDelegate)
        {
            await OnCancel.InvokeAsync(new ChangeEventArgs { Value = CurrentValue });
        }
    }

    /// <summary>
    /// Set input to the previous value
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task CancelValueActionAsync()
    {
        return ChangeValueAsync(PreviousValue);
    }

    #endregion

}
