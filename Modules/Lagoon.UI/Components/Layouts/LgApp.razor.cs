namespace Lagoon.UI.Components;

/// <summary>
/// The App component must be used as the root component of the app. (App.razor)
/// </summary>
public partial class LgApp : LgComponentBase, IPageTitleHandler, IWaitingContextProvider, IPageDefaultLayoutProvider
{

    #region fields

    /// <summary>
    /// Last value set to the document title
    /// </summary>
    private string _currentTitle;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the type of a layout to be used if the page does not declare any layout.
    /// If specified, the type must implement IComponent and accept a parameter named Body.
    /// </summary>
    [Parameter]
    public Type DefaultLayout { get; set; }

    /// <summary>
    /// Gets or sets if the application use tabbed navigation else the simple page navigation is used.
    /// </summary>
    [Parameter]
    public bool IsTabbed { get; set; } = true;

    /// <summary>
    /// Display header into app layout ?
    /// </summary>
    [Parameter]
    public bool ShowHeader { get; set; }

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

    #endregion

    #region render fragments

    /// <summary>
    /// The content that will be displayed while asynchronous authorization is in progress.
    /// </summary>
    [Parameter]
    public RenderFragment Authorizing { get; set; }

    /// <summary>
    /// The child content.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The content to display when an unhandled error occurs.
    /// </summary>
    [Parameter]
    public RenderFragment<Exception> ErrorContent { get; set; }

    /// <summary>
    /// The layout footer content.
    /// </summary>
    [Parameter]
    public RenderFragment FooterContent { get; set; }

    /// <summary>
    /// The content that will be displayed if the user is not authorized.
    /// </summary>
    [Parameter]
    public RenderFragment<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> NotAuthorized { get; set; }

    /// <summary>
    /// Gets or sets the content to display when no match is found for the requested route.
    /// </summary>
    [Parameter]
    public RenderFragment NotFound { get; set; }

    /// <summary>
    /// The list of <see cref="LgStartupTab"/> to define the pages to open when application startup.
    /// (Only used when the parameter "IsTabbed" is <c>true</c>.
    /// </summary>
    [Parameter]
    public RenderFragment StartupTabs { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets the last route data where the user navigate to.
    /// </summary>
    internal RouteData CurrentRouteData;

    /// <summary>
    /// Gets the last URI where the user navigate to.
    /// </summary>
    internal string CurrentUri;

    #endregion

    #region events

    /// <summary>
    /// Event called when router navigate to a page.
    /// </summary>
    internal event Func<NavigatingToEventArgs, Task> OnNavigatingTo;

    /// <summary>
    /// Event called when router navigate to a page.
    /// </summary>
    internal event Func<NavigateToEventArgs, Task> OnNavigateTo;

    #endregion

    #region methods

        ///<inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            App.OnLanguageChanged += OnLanguageChanged;
            App.OnNavigateFromServiceWorker += OnNavigateFromServiceWorker;
            // Check configuration for RGAA support
            if (IsTabbed && App.BehaviorConfiguration.RgaaSupport)
            {
                throw new Exception("RgaaTabbedError".Translate());
            }
        }

        /// <summary>
        /// A navigation event has been received from a service-worker.
        /// According to app configuration, open a new applicative or browser tab
        /// </summary>
        /// <param name="action">Url or partial url (which will be completed with the actual document.uri)</param>
        private void OnNavigateFromServiceWorker(string action)
        {
            if (IsTabbed)
            {
                // open a new applicative tab
                App.NavigateTo(action);
            }
            else
            {
                // open a new browser tab
                JS.Invoke<object>("Lagoon.JsFileUtils.OpenURL", action, null, "_blank", null);
            }
        }

        ///<inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.OnLanguageChanged -= OnLanguageChanged;
                App.OnNavigateFromServiceWorker -= OnNavigateFromServiceWorker;
            }
            base.Dispose(disposing);
        }

    /// <summary>
    /// Check if the current URI can be reached, else return the fallback URI.
    /// </summary>
    /// <param name="uri">URI to navigate to.</param>
    /// <returns>The fallback URI if redirection needed, else null.</returns>
    internal async Task<string> IsRedirectionNeededAsync(string uri)
    {
        if (uri != CurrentUri && OnNavigatingTo is not null)
        {
            // Check if navigation must be cancelled
            NavigatingToEventArgs args = new(uri);
            await OnNavigatingTo?.Invoke(args);
            if (args.Cancel)
            {
                // go back to the current URI
                return CurrentUri;
            }
        }
        // The navigation don't need redirection
        return null;
    }

    /// <summary>
    /// Show page from route in interface.
    /// </summary>
    /// <param name="uri">Target URI.</param>
    /// <param name="routeData">Page render informations.</param>
    internal async Task NavigateToAsync(string uri, RouteData routeData)
    {
        CurrentUri = uri;
        CurrentRouteData = routeData;
        if (OnNavigateTo is not null)
        {
            await OnNavigateTo.Invoke(new NavigateToEventArgs(uri, routeData));
        }
    }

    ///<inheritdoc/>
    async Task IPageTitleHandler.SetPageErrorTitleAsync(ITab tab, string title, string iconName)
    {
        await JS.InvokeVoidAsync("Lagoon.JsUtils.ChangePageTitle", $"{App.ApplicationInformation.Name}  - {title}");
    }

    /// <summary>
    /// Method called when page title is defined.
    /// </summary>
    /// <param name="page">Page instance.</param>
    async Task IPageTitleHandler.SetPageTitleAsync(LgPage page)
    {
        string title = page.Title;
        if (_currentTitle is null || title != _currentTitle)
        {
            _currentTitle = title;
            title = title.CheckTranslate();
            title = App.ApplicationInformation.Name + (string.IsNullOrEmpty(title) ? "" : " - " + title);
            await JS.InvokeVoidAsync("Lagoon.JsUtils.ChangePageTitle", title);
        }
    }

    /// <summary>
    /// Return a new waiting context with a cancellation token source.
    /// </summary>
    /// <returns>A new waiting context with a cancellation token source.</returns>
    WaitingContext IWaitingContextProvider.GetNewWaitingContext()
    {
        return new WaitingContext();
    }

        /// <summary>
        /// Method call when user change Langue
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void OnLanguageChanged(LanguageChangedEventArgs e)
        {
            string xLangCookieName = $".{App.ApplicationInformation.RootName}.Lang";
            // Get the current language from the cookie value
            string res = JS.Invoke<string>("Lagoon.JsUtils.GetCookie", xLangCookieName);
            if (res != e.CultureName)
            {
                // Change the HTML document attribute "lang" and update http cookie (server should answer in the same language)
                JS.InvokeVoid("Lagoon.JsUtils.ChangeLang", xLangCookieName, e.TwoLetterISOLanguageName, e.CultureName);
                ShowScreenReaderInformation("#RgaaLangChanged");
                // Reload the application with the new language
                App.NavigationManager.NavigateTo(CurrentUri, true);
            }
        }

    #endregion

    #region page template

    /// <summary>
    /// Return the default layout to use.
    /// </summary>
    /// <returns></returns>
    Type IPageDefaultLayoutProvider.GetDefaultLayout()
    {
        return DefaultLayout;
    }

    #endregion

}
