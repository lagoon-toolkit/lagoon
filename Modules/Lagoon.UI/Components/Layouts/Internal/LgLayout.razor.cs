namespace Lagoon.UI.Components;


/// <summary>
/// Application base layout
/// </summary>
public partial class LgLayout : LgComponentBase
{

    #region constants

    /// <summary>
    /// Define maximal height to determine current media is vertical orientation
    /// </summary>
    public const int MAX_HEIGHT_VERTICAL_SCREEN = 500;

    #endregion

    #region fields

    /// <summary>
    /// Last window information used for the display.
    /// </summary>
    private WindowInformation _lastWindowInformation;

    /// <summary>
    /// Indicator for screen reader information after login
    /// </summary>
    private bool _srInfoAppLoaded;

    /// <summary>
    /// Menu is collapsed ? (mobile) 
    /// </summary>
    private bool _collapseSidebar = false;

    private bool _isSideBarVisible = false;

    /// <summary>
    /// Css Class to determine sidebar style and grid template
    /// for the layout depending on the sidebar
    /// </summary>
    private string _cssClassSidebar = "sidebarExpanded";

    /// <summary>
    /// Css Class to determine style and grid template
    /// for the layout
    /// </summary>
    public string _cssClassWrapper;

    #endregion

    #region dependencies injection

    /// <summary>
    /// Service used to communicate with the LgMenuConfiguration
    /// </summary>
    /// <value></value>
    [Inject]
    private MenuService MenuService { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Root application component.
    /// </summary>
    [CascadingParameter]
    private LgApp AppComponent { get; set; }

    /// <summary>
    /// Footer render
    /// </summary>
    [CascadingParameter]
    private RenderFragment FooterContent { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Entry point for layout configuration
    /// </summary>
    /// <value></value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the application use tabbed navigation else the simple page navigation is used.
    /// </summary>
    [Parameter]
    public bool IsTabbed { get; set; }

    /// <summary>
    /// Display header into app layout ?
    /// </summary>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>
    /// Display footer into app layout ?
    /// </summary>
    [Parameter]
    public bool ShowFooter { get; set; }

    /// <summary>
    /// Display sidebar into app layout ?
    /// </summary>
    [Parameter]
    public bool ShowSidebar { get; set; }

    /// <summary>
    /// Gets or sets layout top content
    /// </summary>
    [Parameter]
    public RenderFragment TopContent { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// LgLayout initialization
    /// Init wrapper css class depending on header, siderbar usage
    /// used to define css grid template
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ConfigureForCurrentMedia();
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("content-page");
        builder.AddIf(IsTabbed, "content-page-tabbed");
    }

    /// <summary>
    /// After first render, inject blazorsize javascript librairie
    /// </summary>
    /// <param name="firstRender">Is the first render of LgLayout component ?</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Toastr initialization
            await JS.InvokeVoidAsync("Lagoon.JsUtils.initToastrOption", App.BehaviorConfiguration.Toastr);
            // Check screen resolution changes
            App.OnWindowResize += OnWindowResizeAsync;
        }
        else if (!_srInfoAppLoaded)
        {
            // Application loaded indicator after login
            ShowScreenReaderInformation("#RgaaAppLoaded");
            _srInfoAppLoaded = true;
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Dispose resource
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        App.OnWindowResize -= OnWindowResizeAsync;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Called when Sidebar is collapsed or not
    /// </summary>
    public void ToggleSidebarHandler(ToggleSidebarEventArgs e)
    {
        _collapseSidebar = e.Collapsed;
        //Define the css class for the main div next to the side bar
        // this class define the padding of the main div
        // deping on the collapsed or expanded sidebar
        if (_collapseSidebar)
        {
            _cssClassSidebar = "sidebarCollapsed";
        }
        else
        {
            _cssClassSidebar = "sidebarExpanded";
        }
        StateHasChanged();
    }

    /// <summary>
    /// On window resized, change variable
    /// which define layout state as mobile, tablet or large size
    /// </summary>
    /// <param name="newWI">Window information</param>
    public Task OnWindowResizeAsync(WindowInformation newWI)
    {
        WindowInformation oldWI = _lastWindowInformation;

        // Call refresh menu only when screen size change the media orientation or the media type
        if (oldWI.MediaOrientation != newWI.MediaOrientation || oldWI.MediaType != newWI.MediaType)
        {
            MenuService.RefreshAll();
            ConfigureForCurrentMedia();
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Set wrapper Css classes
    /// Set sidebar Css classe
    /// depending on media type and orientation
    /// </summary>
    private void ConfigureForCurrentMedia()
    {
        // We keep the current window information for resize handling
        _lastWindowInformation = App.WindowInformation;
        // Set sidebar visibility
        _isSideBarVisible = ShowSidebar && (App.WindowInformation.MediaType != MediaType.Mobile
            || App.WindowInformation.MediaOrientation == MediaOrientation.Landscape);
        // Init classes on wrapper div => changing grid-template
        if (ShowHeader && ShowSidebar && (App.WindowInformation.MediaType == MediaType.Desktop
            || App.WindowInformation.MediaOrientation == MediaOrientation.Landscape))
        {
            _cssClassWrapper = "wrapperGridHeadSidbar";
            _cssClassSidebar = "sidebarExpanded";
        }
        else if (ShowSidebar && (App.WindowInformation.MediaType == MediaType.Desktop ||
            App.WindowInformation.MediaOrientation == MediaOrientation.Landscape))
        {
            _cssClassWrapper = "wrapperGridSidebar";
        }
        else if (ShowHeader)
        {
            _cssClassWrapper = "wrapperGridHead";
            _cssClassSidebar = "";
        }
        else
        {
            _cssClassWrapper = "";
            _cssClassSidebar = "";
        }
    }

    #endregion

}
