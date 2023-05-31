using Lagoon.Helpers.Data;

namespace Lagoon.UI.Components;

/// <summary>
/// A Global search component.
/// </summary>
public partial class LgSearchNavigation<TItem> : LgAriaComponentBase
    where TItem : IPageLink
{
    #region privates properties

    /// <summary>
    /// Allow to see all data
    /// </summary>
    private bool _canSeeAllData;

    /// <summary>
    /// Data to display
    /// </summary>
    private IEnumerable<TItem> _data;

    /// <summary>
    /// Display or not the items list
    /// </summary>
    private bool _dropdownVisible;

    /// <summary>
    /// Allow c# invokation by JS for the current instance
    /// </summary>
    private DotNetObjectReference<LgSearchNavigation<TItem>> _dotNetObjRef;

    /// <summary>
    /// Current value of search
    /// </summary>
    private string _searchText = "";

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets render of child content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets remote data url
    /// </summary>
    [Parameter]
    public string ControllerUri { get; set; }

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the search text mode (containe by default)
    /// </summary>
    [Parameter]
    public FilterTextSearchMode FilterTextSearchMode { get; set; } = FilterTextSearchMode.Contains;

    /// <summary>
    /// Hilghlight founded text into result list
    /// </summary>
    [Parameter]
    public bool HighlightText { get; set; }

    /// <summary>
    /// Gets or sets the items source
    /// </summary>
    [Parameter]
    public ICollection<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets child item result content 
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> ItemContent { get; set; }

    /// <summary>
    /// Define the number of items displayed from the search result.
    /// Default : 5
    /// </summary>
    [Parameter]
    public int MaxItemsInView { get; set; } = 5;

    /// <summary>
    /// Minimal input length before the search is fired
    /// </summary>
    [Parameter]
    public int MinSearchCharacter { get; set; }

    /// <summary>
    /// Gets or sets key enter event in search input
    /// </summary>
    [Parameter]
    public EventCallback<string> OnSearch { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets if the menu have a vertical scroll bar when the list of sub-items is too large.
    /// </summary>
    [Parameter]
    public bool Scrollable { get; set; }

    /// <summary>
    /// Display a "see all" value to get all data
    /// </summary>
    [Parameter]
    public bool SeeAllEnabled { get; set; }

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
    internal ElementReference ElementRef { get; set; }

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
        _dotNetObjRef = DotNetObjectReference.Create(this);
        base.OnInitialized();
        TextFilterCollation ??= App.BehaviorConfiguration.TextFilterCollation.Value;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dotNetObjRef.Dispose();
        }
        base.Dispose(disposing);
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
    private ModelFilter<TItem> GetModelFilter()
    {
        ModelFilter<TItem> modelFilter = new(default);
        modelFilter.AddTextSearch(page => page.Title, FilterTextSearchMode, _searchText, TextFilterCollation.Value);
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
        bool isKeyEnter = args.Code is "Enter" or "NumpadEnter";
        if (args.Code == "ArrowDown" || args.Code == "ArrowUp" || isKeyEnter)
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
        builder.Add("global-search", CssClass);
        // Add the scrollable class
        builder.AddIf(Scrollable, "scrollable");
    }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes()
    {
        return GetAdditionalAttributes(AdditionalAttributes, GetTooltipAttributes(Tooltip, TooltipIsHtml));
    }

    /// <summary>
    /// Close the dropdown after navigate to the search
    /// </summary>
    private void Navigate(string uri)
    {
        _dropdownVisible = false;
        App.NavigationManager.NavigateTo(uri);
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
    /// OnSearch from javascript
    /// </summary> 
    [JSInvokable("OnSearchAsync")]
    public async Task OnSearchAsync()
    {
        if (OnSearch.HasDelegate && _searchText.Length > 0)
        {
            await OnSearch.TryInvokeAsync(App, _searchText);
        }
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

    #endregion
}
