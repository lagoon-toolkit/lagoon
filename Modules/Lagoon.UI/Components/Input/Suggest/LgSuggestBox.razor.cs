using Lagoon.Helpers.Data;
using Lagoon.UI.Components.Input.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;

/// <summary>
/// A suggest box component.
/// </summary>
public partial class LgSuggestBox<TItem> : LgInputRenderBase<string>
{
    #region properties

    /// <summary>
    /// Allow to see all data
    /// </summary>
    private bool _canSeeAllData;

    /// <summary>
    /// Data to diplaying
    /// </summary>
    private IEnumerable<TItem> _data;

    /// <summary>
    /// Display or not the items list
    /// </summary>
    private bool _dropdownVisible;

    /// <summary>
    /// Allow c# invokation by JS for the current instance
    /// </summary>
    private DotNetObjectReference<LgSuggestBox<TItem>> _dotNetObjRef = null;

    /// <summary>
    /// Current value of search
    /// </summary>
    private string _searchText = "";

    /// <summary>
    /// Gets or sets select render
    /// </summary>
    protected RenderFragment Suggest { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Get or set the flag indicating if value should be returned without the mask.
    /// </summary>
    /// <example> phone number with mask '06-05-06-07-08' will ne returned as '0605060708' if <c>AutoUnmask</c> is set to <c>true</c></example>
    /// <value><c>false</c> by default</value>
    [Parameter]
    public bool AutoUnmask { get; set; } = false;

    /// <summary>
    /// Gets or sets binded property
    /// </summary>
    [Parameter]
    public Func<TItem, string> BindedProperty { get; set; }

    /// <summary>
    /// Get or set the flag indicating if value should be clear if mask is incomplete.
    /// </summary>
    /// <example> phone number with mask '06-05-06-07-' will ne returned as '' if <c>ClearIncomplete</c> is set to <c>true</c></example>
    /// <value><c>false</c> by default</value>
    [Parameter]
    public bool ClearIncomplete { get; set; } = false;

    /// <summary>
    /// Gets or sets remote data url
    /// </summary>
    [Parameter]
    public string ControllerUri { get; set; }

    /// <summary>
    /// Filter search list
    /// </summary>
    [Parameter]
    public Expression<Func<TItem, string>> FilterProperty { get; set; }

    /// <summary>
    /// Hilghlight founded text into result list
    /// </summary>
    [Parameter]
    public bool HighlightText { get; set; }

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
    /// Gets or sets the search text mode (containe by default)
    /// </summary>
    [Parameter]
    public FilterTextSearchMode FilterTextSearchMode { get; set; } = FilterTextSearchMode.Contains;

    /// <summary>
    /// Gets or sets child item result content 
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> ItemContent { get; set; }

    /// <summary>
    /// Gets or sets the items source
    /// </summary>
    [Parameter]
    public ICollection<TItem> Items { get; set; }

    /// <summary>
    /// Define the number of items displayed from the search result.
    /// Default : 5
    /// </summary>
    [Parameter]
    public int MaxItemsInView { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum number of characters entered
    /// </summary>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Minimal input length before the search is fired
    /// </summary>
    [Parameter]
    public int MinSearchCharacter { get; set; }

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
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnInput { get; set; }

    /// <summary>
    /// Gets or sets a callback called on the onkey event.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Display a "see all" value to get all data
    /// </summary>
    [Parameter]
    public bool SeeAllEnabled { get; set; }

    /// <summary>
    /// Gets or sets the type of case and accent sensitivity.
    /// </summary>
    /// <remarks>NOTICE: For remote data, the result may depend on the collation applied on the database.</remarks>
    [Parameter]
    public CollationType? TextFilterCollation { get; set; }

    #endregion

    #region properties 

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    internal ElementReference ElementRef;

    /// <summary>
    ///  Gets or sets the DOM focus element reference.
    /// </summary>
    protected override ElementReference FocusElementRef => ElementRef;

    /// <summary>
    /// Determine if data must be loaded from a controller.
    /// </summary>
    protected bool HasController => !string.IsNullOrEmpty(ControllerUri);

    /// <summary>
    /// Determine if all data are displayed
    /// </summary>
    protected bool SeeAllData => !SeeAllEnabled && _canSeeAllData;

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _dotNetObjRef = DotNetObjectReference.Create(this);
        TextFilterCollation ??= App.BehaviorConfiguration.TextFilterCollation.Value;
    }

    /// <summary>
    /// Initialize the context
    /// </summary>
    /// <param name="allItems">Get all items if true</param>
    /// <returns></returns>
    private async Task InitializeDataAsync(bool allItems)
    {
        // Load data
        using (WaitingContext wc = GetNewWaitingContext())
        {
            DataPage<TItem> dataPage = await GetDataAsync(allItems, wc.CancellationToken);

            if (dataPage is not null)
            {
                _data = dataPage.Data;
                _canSeeAllData = !allItems && dataPage.PageCount > 1;
            }
        }
        // Highlight text
        if (HighlightText)
        {
            await JsHighlightTextAsync();
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Keyboard controller for Rgaa
            await JS.InvokeVoidAsync("Lagoon.LgGlobalSearch.keyBoardController", ElementRef, _dotNetObjRef);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Get the data
    /// </summary>
    /// <param name="allItems">Take all data?</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    private Task<DataPage<TItem>> GetDataAsync(bool allItems, CancellationToken cancellationToken = default)
    {
        CustomDataPageLoader<TItem> data = GetDataLoader(allItems);
        return data is null ? Task.FromResult<DataPage<TItem>>(null) : data.GetDataPageAsync(cancellationToken);
    }

    /// <summary>
    /// Get data loader
    /// </summary>
    /// <param name="allItems">Take all data?</param>
    /// <returns></returns>
    private CustomDataPageLoader<TItem> GetDataLoader(bool allItems)
    {
        CustomDataPageLoader<TItem> dataLoader;
        if (HasController)
        {
            dataLoader = new RemoteDataPageLoader<TItem>()
            {
                ControllerUri = ControllerUri,
                ControllerQueryArg = _searchText,
                HttpClient = Http
            };

        }
        else if (Items is null)
        {
            return null;
        }
        else
        {
            dataLoader = new LocalDataPageLoader<TItem>()
            {
                Items = Items.AsQueryable()
            };
        }

        dataLoader.PageSize = SeeAllEnabled || allItems ? 0 : MaxItemsInView;
        dataLoader.CurrentPage = 1;
        dataLoader.AllowCount = true;
        dataLoader.ModelFilter = GetModelFilter();

        return dataLoader;
    }

    /// <summary>
    /// Search filter 
    /// </summary>
    /// <returns></returns>
    private ModelFilter<TItem> GetModelFilter()
    {
        ModelFilter<TItem> modelFilter = new(default);
        modelFilter.AddTextSearch(FilterProperty, FilterTextSearchMode, _searchText, TextFilterCollation.Value);
        return modelFilter;
    }

    /// <summary>
    /// On focus input
    /// </summary>
    /// <param name="args">FocusEventArgs</param>
    /// <returns></returns>
    private async Task FocusAsync(FocusEventArgs args)
    {
        if (MinSearchCharacter == 0)
        {
            await SearchDataAsync();
        }
        if (OnFocus.HasDelegate)
        {
            await OnFocus.TryInvokeAsync(App, args);
        }
    }

    /// <summary>
    /// Launches the search on Enter
    /// </summary>
    /// <param name="args">ChangeEventArgs</param>
    /// <returns></returns>
    private async Task SearchAsync(ChangeEventArgs args)
    {
        _searchText = args.Value.ToString();
        await SearchDataAsync();
        if (OnInput.HasDelegate)
        {
            await OnInput.TryInvokeAsync(App, args);
        }
    }

    /// <summary>
    /// Search data
    /// </summary>
    /// <returns></returns>
    private async Task SearchDataAsync()
    {
        if (_searchText.Length >= MinSearchCharacter)
        {
            _data = null;
            _dropdownVisible = true;
            await InitializeDataAsync(false);
        }
        else
        {
            _dropdownVisible = false;
        }
    }

    /// <summary>
    /// Display current dropdown with current data 
    /// </summary>
    /// <param name="args"></param>
    private async Task ShowDropdownAsync(KeyboardEventArgs args)
    {
        if (args.Code is "ArrowDown" or "ArrowUp" or "Enter" or "NumpadEnter")
        {
            if (_data is not null)
            {
                _dropdownVisible = true;
            }
        }
        else if (args.Code == "Escape")
        {
            _dropdownVisible = false;
            await JS.InvokeVoidAsync("Lagoon.LgGlobalSearch.resetFocus", ElementRef);
        }
        if (OnKeyUp.HasDelegate)
        {
            await OnKeyUp.TryInvokeAsync(App, args);
        }
    }

    /// <summary>
    ///  user click to get all items
    /// </summary>
    /// <returns></returns>
    private Task GetAllItemsAsync()
    {
        return InitializeDataAsync(true);
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("suggest-box", "form-group", CssClass);
        builder.AddIf(ReadOnly, "form-group-ro");
    }

    /// <summary>
    /// Close dropdown from javascript
    /// </summary> 
    [JSInvokable]
    public Task CloseDropdown()
    {
        _dropdownVisible = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Call JS to highligth search text into result
    /// </summary>
    /// <returns></returns>
    public async Task JsHighlightTextAsync()
    {
        await Task.Delay(200);
        await JS.InvokeVoidAsync("Lagoon.LgGlobalSearch.highlightFoundedText", ElementRef, _searchText, FilterTextSearchMode);
    }

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        builder.AddContent(0, Suggest);
    }

    /// <summary>
    /// Invoked when the value is changed.
    /// </summary>
    internal async Task SelectItemAsync(TItem item)
    {
        if (BindedProperty is null)
        {
            await BaseChangeValueAsync(new ChangeEventArgs { Value = item?.ToString() });
        }
        else
        {
            await BaseChangeValueAsync(new ChangeEventArgs { Value = BindedProperty(item) });
        }
        _dropdownVisible = false;
    }

    /// <inheritdoc />
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValue = value?.ToString();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }

    #endregion
}
