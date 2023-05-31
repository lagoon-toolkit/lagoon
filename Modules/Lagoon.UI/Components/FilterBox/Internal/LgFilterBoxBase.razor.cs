namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Filter box 
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
/// <typeparam name="TFilter">The Filter type.</typeparam>
public abstract partial class LgFilterBoxBase<TValue, TFilter> : LgComponentBase, ILgFilterBox
    where TFilter : Filter<TValue>
{

    #region fields

    /// <summary>
    /// Keep filter before change
    /// </summary>
    private Filter _filterValue;

    /// <summary>
    /// Gets or sets display of the current filter
    /// </summary>
    protected string _summary;

    /// <summary>
    /// Gets or sets if there is no filter.
    /// </summary>
    private bool _isEmpty;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets actived tabs in filter
    /// </summary>
    [Parameter]
    public FilterTab ActiveTabs { get; set; }

    /// <summary>
    /// Gets or sets style class of the control part
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Default tab displayed
    /// </summary>
    [Parameter]
    public FilterTab DefaultTab { get; set; }

    /// <summary>
    /// Gets or sets filter expression
    /// </summary>
    [Parameter]
    public TFilter Filter { get; set; }

    /// <summary>
    /// Gets or sets selection event
    /// </summary>
    [Parameter]
    public EventCallback<TFilter> FilterChanged { get; set; }

    /// <summary>
    /// Gets or sets the method to use to format value.
    /// </summary>
    [Parameter]
    public Func<TValue, string> FormatValue { get; set; }

    /// <summary>
    /// Gets or sets the list of choices.
    /// </summary>
    [Parameter]
    public virtual Func<CancellationToken, Task<IEnumerable<TValue>>> GetItems { get; set; }

    /// <summary>
    /// Gets or sets list of choices
    /// </summary>
    [Parameter]
    [Obsolete("This property is only usable as parameter, use the WorkingItems property")]
    public IEnumerable<TValue> Items { get; set; }

    /// <summary>
    /// Gets or sets filter expression
    /// </summary>
    [Parameter]
    public Filter RawFilter { get => Filter; set => Filter = value as TFilter; }

    /// <summary>
    /// Gets or sets selection event
    /// </summary>
    [Parameter]
    public EventCallback<Filter> RawFilterChanged { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the currently loaded editor for the filter.
    /// </summary>
    internal LgFilterEditorBase<TValue, TFilter> FilterEditor { get; set; }

    /// <summary>
    /// Gets or sets open state of the list
    /// </summary>
    protected bool IsOpen { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets filter content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region ILgFilterBox interface

    /// <summary>
    /// Get or sets the selected tab.
    /// </summary>
    ILgFilterEditor ILgFilterBox.FilterEditor => FilterEditor;

    #endregion

    #region methods               

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        FormatValue ??= DefaultFormatValue;
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Filter != _filterValue || _filterValue is null)
        {
            _filterValue = Filter;
            UpdateSummary();
        }
    }

    /// <summary>
    /// Return the default methods to use to format values.
    /// </summary>
    /// <returns>The default methods to use to format values.</returns>
    protected virtual string DefaultFormatValue(TValue value)
    {
        return value?.ToString();
    }

    /// <summary>
    /// Get the editor render fragment.
    /// </summary>
    /// <returns>The editor render fragment.</returns>
    private RenderFragment GetFilterEditorContent()
    {
        return builder =>
        {
            builder.OpenComponent(1, GetFilterEditorComponentType());
            builder.CloseComponent();
        };
    }

    /// <summary>
    /// Return the type of the component to use to edit filter.
    /// </summary>
    /// <returns>The type of the component to use to edit filter.</returns>
    protected abstract Type GetFilterEditorComponentType();

    /// <summary>
    /// Update the filter summary text.
    /// </summary>
    private void UpdateSummary()
    {
        _summary = Filter?.ToString(FormatFilterValue);
        _isEmpty = string.IsNullOrEmpty(_summary);
        if (_isEmpty)
        {
            _summary = "FilterBoxNoSelection".Translate();
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("filterbox", CssClass);
        builder.AddIf(!_isEmpty, "filterbox-filled");
    }

    /// <summary>
    /// Rise cancel event
    /// </summary>
    /// <returns></returns>
    protected void Cancel()
    {
        IsOpen = false;
    }

    /// <summary>
    /// Method called when the clear filter is called.
    /// </summary>
    /// <returns></returns>
    protected async Task ResetAsync()
    {
        await ChangeFilterAsync(null);
        IsOpen = false;
    }

    /// <summary>
    /// Method called when the validate button is clicked.
    /// </summary>
    /// <returns></returns>
    protected async Task ValidateAsync()
    {
        if (FilterEditor is null)
        {
            throw new InvalidOperationException("No filter editor loaded.");
        }
        else
        {
            await ChangeFilterAsync(FilterEditor.GetFilter());
            IsOpen = false;
        }
    }

    /// <summary>
    /// Change the current filter.
    /// </summary>
    /// <param name="filter"></param>
    protected Task ChangeFilterAsync(TFilter filter)
    {
        if (filter is not null && filter.IsEmpty)
        {
            filter = null;
        }
        if (Filter != filter)
        {
            Filter = filter;
            // Callback use by the binding
            if (FilterChanged.HasDelegate)
            {
                return FilterChanged.TryInvokeAsync(App, Filter);
            }
            // Call back use by the GridView 
            if (RawFilterChanged.HasDelegate)
            {
                return RawFilterChanged.TryInvokeAsync(App, Filter);
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Dropdown key up event management
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnKeyUpAsync(KeyboardEventArgs args)
    {
        if (args.Code == "Enter")
        {
            await ValidateAsync();
        }
    }

    /// <summary>
    /// Format the value to be displayed.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The formated value.</returns>
    internal string FormatFilterValue(TValue value)
    {
        if (value is null)
        {
            return "FilterEmptyValue".Translate();
        }
        else
        {
            return FormatValue(value);
        }
    }

    #endregion

}
