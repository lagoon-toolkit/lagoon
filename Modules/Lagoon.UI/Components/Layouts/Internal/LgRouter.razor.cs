using Lagoon.UI.Application;
using Lagoon.UI.Pages;
using Microsoft.AspNetCore.Components.Routing;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Lagoon router
/// </summary>
public class LgRouter : IComponent, IHandleAfterRender, IDisposable
{
    #region private fields

    private RenderHandle _renderHandle;
    private bool _navigationInterceptionEnabled;
    private string _location;
    private bool _initialized;

    #endregion

    #region Dependencies injection

    /// <summary>
    /// The main application.
    /// </summary>
    [Inject]
    private LgApplication App { get; set; }

    /// <summary>
    /// Property to perform logging.
    /// </summary>
    [Inject]
    private ILoggerFactory LoggerFactory { get; set; }

    /// <summary>
    /// Gets or sets the navigation manager service.
    /// </summary>
    [Inject]
    private NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Gets or sets navigation interception dependency
    /// </summary>
    [Inject]
    private INavigationInterception NavigationInterception { get; set; }

    #endregion

    #region cascading parameter

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    public LgApp AppComponent { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets found content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Attaches the router to a Microsoft.AspNetCore.Components.RenderHandle.
    /// </summary>
    /// <param name="renderHandle">A Microsoft.AspNetCore.Components.RenderHandle that allows the component to be rendered.</param>
    void IComponent.Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
        _location = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        NavigationManager.LocationChanged += HandleLocationChangedAsync;
        App.RefreshRouteTable();
    }

    /// <summary>
    /// Remove router events
    /// </summary>
    public virtual void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChangedAsync;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    async Task IHandleAfterRender.OnAfterRenderAsync()
    {
        if (!_navigationInterceptionEnabled)
        {
            _navigationInterceptionEnabled = true;
            await NavigationInterception.EnableNavigationInterceptionAsync();
        }
    }

    /// <summary>
    /// Sets parameters supplied by the component's parent in the render tree.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A System.Threading.Tasks.Task that completes when the component has finished updating and rendering itself.</returns>
    public async Task SetParametersAsync(ParameterView parameters)
    {
        try
        {
            parameters.SetParameterProperties(this);
            // Initialise the navigation
            if (!_initialized)
            {
                _initialized = true;
                await NavigateToAsync(_location);
            }
            // Do the children component render
            _renderHandle.Render(ChildContent);
        }
        catch (Exception ex)
        {
            App.TraceCriticalException(ex);
        }
    }

    /// <summary>
    /// Call when location has changed
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Event arguments</param>
    private async void HandleLocationChangedAsync(object sender, LocationChangedEventArgs args)
    {
        try
        {
            if (_renderHandle.IsInitialized)
            {
                string uri = NavigationManager.ToBaseRelativePath(args.Location);
                string redirectTo = await AppComponent.IsRedirectionNeededAsync(uri);
                if (redirectTo is null)
                {
                    await NavigateToAsync(uri);
                }
                else
                {
                    NavigationManager.NavigateTo(redirectTo);
                }
            }
        }
        catch (Exception ex)
        {
            AppComponent.ShowException(ex);
        }
    }

    /// <summary>
    /// Define route and tab parameters
    /// </summary>
    private Task NavigateToAsync(string uri)
    {
#if DEBUG
        ILogger<LgRouter> logger = LoggerFactory.CreateLogger<LgRouter>();
        logger.LogInformation("NavigateTo : {Uri}", uri);
#endif
        // Get the route handler
        RouteData routeData = App.GetRouteData(uri) ?? App.GetRouteData(LgPageNotFound.ROUTE);
        // Navigate to the handler
        return AppComponent.NavigateToAsync(uri, routeData);
    }

    /// <summary>
    /// Build tab key from the route uri.
    /// </summary>
    /// <param name="uri">URI to open in the tab.</param>
    /// <param name="prefix">Prefix to add.</param>
    /// <returns></returns>
    internal static string GenerateTabKey(string uri, string prefix = "tab")
    {
        string[] uriSegments = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (uriSegments.Length == 0)
        {
            return LgAppTabContainerLayout.TABHOMEKEY;
        }
        for (int i = 0; i < uriSegments.Length; i++)
        {
            uriSegments[i] = char.ToUpperInvariant(uriSegments[i][0]) + uriSegments[i][1..];
        }
        string key = string.Join(string.Empty, uriSegments);
        return prefix + key;
    }
    #endregion

}
