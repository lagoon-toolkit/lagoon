using Lagoon.UI.Application;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Lagoon.UI.Components;

/// <summary>
/// Base class for Lagoon components.
/// </summary>
public abstract class LgComponentBase : ComponentBase, IDisposable
{

    #region fields

    /// <summary>
    /// The anonymous http client.
    /// </summary>
    private HttpClient _anonymousHttpClient;

    /// <summary>
    /// The authenticated http client.
    /// </summary>
    private HttpClient _authenticatedHttpClient;    

    #endregion

    #region dependencies injection

    /// <summary>
    /// Give access to LgApplication context
    /// </summary>
    [Inject]
    public LgApplication App { get; set; }

    /// <summary>
    /// Used to retrieve current logged user
    /// </summary>
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    /// <summary>
    /// Used to check policy for the connected user
    /// </summary>
    [Inject]
    public PolicyService PolicyService { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Waiting context provider.
    /// </summary>
    [CascadingParameter]
    internal IWaitingContextProvider WaitingContextProvider { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The JS Runtime instance.
    /// </summary>
    public IJSInProcessRuntime JS => App.JS;

    /// <summary>
    /// The navigation manager.
    /// </summary>
    public NavigationManager NavigationManager => App.NavigationManager;

    /// <summary>
    /// Shortcut to AuthenticatedHttpClient.
    /// </summary>
    public HttpClient Http => AuthenticatedHttpClient;

    /// <summary>
    /// Authenticated HttpClient instance.
    /// </summary>
    public HttpClient AuthenticatedHttpClient
    {
        get
        {
            _authenticatedHttpClient ??= App.HttpClientFactory.CreateAuthenticatedClient();
            return _authenticatedHttpClient;
        }
    }

    /// <summary>
    /// Anonymous HttpClient instance.
    /// </summary>
    public HttpClient AnonymousHttpClient
    {
        get
        {
            _anonymousHttpClient ??= App.HttpClientFactory.CreateAnonymousClient();
            return _anonymousHttpClient;
        }
    }

    #endregion

    #region initialization

    /// <summary>
    /// Generates a new DOM element identifier in order to associate it with a label.
    /// </summary>
    /// <returns>The new id.</returns>
    protected internal static string GetNewElementId(char prefix = 'i')
    {
        return prefix + Guid.NewGuid().ToString("N");
    }

    #endregion

    #region show messages

    /// <summary>
    /// Show and trace an exception message
    /// </summary>
    /// <param name="ex">Exception to trace and display</param>
    public void ShowException(Exception ex)
    {
        App.ShowException(ex);
    }

    /// <summary>
    /// Display a warning message
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowWarning(string message, string title = null)
    {
        App.ShowWarning(message, title);
    }

    /// <summary>
    /// Display a warning HTML message.
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowWarning(MarkupString message, MarkupString? title = null)
    {
        App.ShowWarning(message, title);
    }

    /// <summary>
    /// Display a success message
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowSuccess(string message, string title = null)
    {
        App.ShowSuccess(message, title);
    }

    /// <summary>
    /// Display a success HTML message.
    /// </summary>
    /// <param name="message">toastr message</param>
    /// <param name="title">toastr title</param>
    public void ShowSuccess(MarkupString message, MarkupString? title = null)
    {
        App.ShowSuccess(message, title);
    }

    /// <summary>
    /// Display an error message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowError(string message, string title = null)
    {
        App.ShowError(message, title);
    }

    /// <summary>
    /// Display a HTML error message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowError(MarkupString message, MarkupString? title = null)
    {
        App.ShowError(message, title);
    }

    /// <summary>
    /// Display an information message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowInformation(string message, string title = null)
    {
        App.ShowInformation(message, title);
    }

    /// <summary>
    /// Display a HTML information message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="title">The title of the message.</param>
    public void ShowInformation(MarkupString message, MarkupString? title = null)
    {
        App.ShowInformation(message, title);
    }

    #endregion

    #region screen reader information

    /// <summary>
    /// Display information for the screen reader only
    /// </summary>
    /// <param name="message"></param>

    public void ShowScreenReaderInformation(string message)
    {
        App.ShowScreenReaderInformation(message);
    }

    #endregion

    #region confirmation message & Modal

    /// <summary>
    /// Show the confirmation modal
    /// </summary>
    /// <param name="confirmationMessage">Confirmation message</param>
    /// <param name="callback">Confirmation callback</param>
    /// <param name="title">Confirmation title</param>
    public void ShowConfirm(string confirmationMessage, Func<Task> callback, string title = null)
    {
        TryShowConfirmAsync(confirmationMessage, callback, title);
    }

    /// <summary>
    /// Execute async method to show a modal window.
    /// </summary>
    /// <param name="confirmationMessage">Confirmation message</param>
    /// <param name="callback">Confirmation callback</param>
    /// <param name="title">Confirmation title</param>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void TryShowConfirmAsync(string confirmationMessage, Func<Task> callback, string title = null)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await App.ShowConfirmAsync(confirmationMessage.CheckTranslate(), callback, title);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

    #region authentication management

    /// <summary>
    /// Return user authentication state
    /// </summary>
    /// <returns>True if authenticated, false otherwise</returns>
    public async Task<bool> IsUserAuthenticatedAsync()
    {
        AuthenticationState user = await AuthenticationStateProvider?.GetAuthenticationStateAsync();
        return user != null && user.User != null && user.User.Identity.IsAuthenticated;
    }

    /// <summary>
    /// Check if current user is in a specified policy
    /// </summary>
    /// <param name="user">Informations about the user.</param>
    /// <param name="policyName">Policy name to check</param>
    /// <returns><c>true</c> is user is in policy or if the <paramref name="policyName"/> is null or empty; <c>false</c> otherwise</returns>
    public Task<bool> IsInPolicyAsync(ClaimsPrincipal user, string policyName)
    {
        return PolicyService.IsInPolicyAsync(user, policyName);
    }

    /// <summary>
    /// Check if current user is in a specified policy
    /// </summary>
    /// <param name="policyName">Policy name to check</param>
    /// <returns><c>true</c> is user is in policy or if the <paramref name="policyName"/> is null or empty; <c>false</c> otherwise</returns>
    public async Task<bool> IsInPolicyAsync(string policyName)
    {
        return await IsInPolicyAsync(await GetUserAsync(), policyName);
    }

    /// <summary>
    /// Return user ClaimsPrincipal
    /// </summary>
    /// <returns>True if authenticated, false otherwise</returns>
    public virtual async Task<ClaimsPrincipal> GetUserAsync()
    {
        AuthenticationState user = await AuthenticationStateProvider?.GetAuthenticationStateAsync();
        return user != null && user.User != null && user.User.Identity.IsAuthenticated
            ? user.User
            : null;
    }

    /// <summary>
    /// Retrieve information added by AddInfo() during OnSignIn
    /// </summary>
    /// <param name="key">Key info</param>
    /// <returns>Value if found, null otherwise</returns>
    public async Task<string> UserInfoAsync(string key)
    {
        ClaimsPrincipal claimsPrincipal = await GetUserAsync();
        return claimsPrincipal?.Claims.Where(c => c.Type == key).FirstOrDefault()?.Value;
    }

    #region obsolete

    /// <summary>
    /// Return user authentication state
    /// </summary>
    /// <returns>True if authenticated, false otherwise</returns>
    [Obsolete("Use the \"IsUserAuthenticateAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<bool> IsUserAuthenticate()
    {
        return IsUserAuthenticatedAsync();
    }

    /// <summary>
    /// Return user ClaimsPrincipal
    /// </summary>
    /// <returns>True if authenticated, false otherwise</returns>
    [Obsolete("Use the \"IsUserAuthenticateAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<ClaimsPrincipal> GetUser()
    {
        return GetUserAsync();
    }

    /// <summary>
    /// Retrieve information added by AddInfo() during OnSignIn
    /// </summary>
    /// <param name="key">Key info</param>
    /// <returns>Value if found, null otherwise</returns>
    [Obsolete("Use the \"IsUserAuthenticateAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<string> UserInfo(string key)
    {
        return UserInfoAsync(key);
    }

    /// <summary>
    /// Check if current user is in <paramref name="policyName"/>
    /// </summary>
    /// <param name="policyName">Policy to check</param>
    /// <returns>True is user is in policy, false otherwise</returns>
    [Obsolete("Use the \"IsUserAuthenticateAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<bool> IsInPolicy(string policyName)
    {
        return IsInPolicyAsync(policyName);
    }

    #endregion

    #endregion

    #region dispose resources

    /// <summary>
    /// Don't modify the content of this method. If you want to free some resources, use <c>Dispose(bool disposing)</c>
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Free used resources
    /// </summary>
    /// <param name="disposing">Will be always true, we don't use finalize in this component</param>
    protected virtual void Dispose(bool disposing) { }

    #endregion

    #region render

    /// <summary>
    /// Return the content to set in "class" attribute.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetClassAttribute()
    {
        LgCssClassBuilder builder = new();
        OnBuildClassAttribute(builder);
        return builder.ToString();
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected virtual void OnBuildClassAttribute(LgCssClassBuilder builder) { }

    /// <summary>
    /// Get the list of additional attributes to add to component.
    /// </summary>
    /// <param name="lists">Attribute lists.</param>
    /// <returns>The list of additional attributes to add to component.</returns>
    public static IEnumerable<KeyValuePair<string, object>> GetAdditionalAttributes(params IEnumerable<KeyValuePair<string, object>>[] lists)
    {
        return lists.Where(list => list != null).SelectMany(list => list);
    }

    /// <summary>
    /// Get the list of attributes to add to render a tooltip.
    /// </summary>
    /// <param name="tooltip">Tooltip content.</param>
    /// <param name="tooltipIsHtml"><c>true</c> to authorize html content. <c>false</c> otherwise.</param>
    /// <param name="position">Tooltip position.</param>
    /// <param name="ignoreTooltip">Indicate if tooltip must be ignore.</param>
    /// <returns>The list of attributes to add to render a tooltip.</returns>
    public static IEnumerable<KeyValuePair<string, object>> GetTooltipAttributes(string tooltip, bool tooltipIsHtml, TooltipPosition position = TooltipPosition.None, bool ignoreTooltip = false)
    {
        if (!ignoreTooltip)
        {
            string text = tooltip.CheckTranslate();
            if (!string.IsNullOrEmpty(text))
            {
                yield return new KeyValuePair<string, object>("data-tooltip-html", tooltipIsHtml.JsonEncode());
                yield return new KeyValuePair<string, object>("data-original-title", text);
                yield return new KeyValuePair<string, object>("title", text);
                if (position != TooltipPosition.None)
                {
                    yield return new KeyValuePair<string, object>("data-tooltip-pos", position.ToString().ToLowerInvariant());
                }
            }
        }
    }

    #endregion

    #region methods - Waiting Context

    /// <summary>
    /// Return a new waiting context with a cancellation token source.
    /// </summary>
    /// <returns>A new waiting context with a cancellation token source.</returns>
    public WaitingContext GetNewWaitingContext()
    {
        if (WaitingContextProvider is null)
        {
#if DEBUG
            throw new InvalidOperationException("The component must be placed under a IWaitingContextProvider component.");
#else
            return null;
#endif
        }
        return WaitingContextProvider.GetNewWaitingContext();
    }

    #endregion

    #region methods - Executing actions

    /// <summary>
    /// The methode raise the CommandName if it's defined.
    /// Else if a ConfirmationMessage is set, the confirmation message is shown;
    /// If an URI is set, the navigation is used else the OnClick event handler is raised.
    /// </summary>
    /// <param name="component">The source component.</param>
    /// <returns>The executing task.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task ExecuteActionAsync(IActionComponent component)
    {

        if (string.IsNullOrEmpty(component.CommandName))
        {
            await ExecuteActionAsync(component.OnClick, component.Uri ?? component.Link?.Uri, component.ConfirmationMessage, component.Target);
        }
        else
        {
            if (!string.IsNullOrEmpty(component.ConfirmationMessage) && App.IsDebugEnabled)
            {
                throw new InvalidOperationException($"The {nameof(component.ConfirmationMessage)} parameter cannot be used at the same time as {nameof(component.CommandName)} parameter.");
            }
            await ExecuteCommandAsync(component.ParentCommandListener, component.CommandName, component.CommandArgument);
        }
    }

    /// <summary>
    /// Execute an event action after showing confirmation.
    /// </summary>
    /// <param name="onAction">Action to execute.</param>
    /// <param name="navigateToUri">URI to navigate to is the action is not defined.</param>
    /// <param name="confirmation">Confirmation to show before action execution.</param>
    /// <param name="target">The name of the browser window in which the URI should be opened.</param>
    /// <returns>The executing task.</returns>
    public Task ExecuteActionAsync(EventCallback<ActionEventArgs> onAction, string navigateToUri = null, string confirmation = null, string target = null)
    {
        return ExecuteActionAsync(onAction, (wc) => new ActionEventArgs(wc), navigateToUri, confirmation, target);
    }

    /// <summary>
    /// Execute an event action after showing confirmation.
    /// </summary>
    /// <param name="onAction">Action to execute.</param>
    /// <param name="newArgCallback">Function returning the arg to pass to the event.</param>
    /// <param name="navigateToUri">URI to navigate to if the action is not defined.</param>
    /// <param name="confirmation">Confirmation to show before action execution.</param>
    /// <param name="target">The name of the browser window in which the URI should be opened.</param>
    /// <returns>The executing task.</returns>
    public async Task ExecuteActionAsync<TActionEventArgs>(EventCallback<TActionEventArgs> onAction,
        Func<WaitingContext, TActionEventArgs> newArgCallback, string navigateToUri = null,
        string confirmation = null, string target = null) where TActionEventArgs : ActionEventArgs
    {
        // Closing previous error messages
        await JS.InvokeVoidAsync("Lagoon.JsUtils.hideAllToastr");
        // Run the action
        await new ActionContext<TActionEventArgs>(this, onAction, newArgCallback, confirmation, navigateToUri, target).ExecuteAsync();
    }

    /// <summary>
    /// Bubble a command to the parent's components.
    /// </summary>
    /// <param name="commandListener">The parent command listener.</param>
    /// <param name="commandName">The command name.</param>
    /// <param name="commandArgument">The command argument.</param>
    /// <returns>The executing task.</returns>
    internal async Task ExecuteCommandAsync(ICommandListener commandListener, string commandName, object commandArgument = null)
    {
        try
        {
            // Closing previous error messages
            await JS.InvokeVoidAsync("Lagoon.JsUtils.hideAllToastr");
            // Bubble the command
            if (commandListener is not null)
            {
                using (WaitingContext wc = GetNewWaitingContext())
                {
                    await commandListener.RaiseBubbleCommandAsync(new CommandEventArgs(wc, commandName, commandArgument));
                }
            }
#if DEBUG
            else
            {
                Lagoon.Helpers.Trace.ToConsole(this, $"No more CommandListener for {commandName}");

            }
#endif
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

    #region methods - policy

    /// <summary>
    /// Initialise the visibility and editable state of a component with policies.
    /// </summary>
    /// <param name="policyState">The component policy state.</param>
    /// <param name="parentPolicy">The parent policy manager.</param>
    /// <param name="hasPolicyVisible">Indicate if the current component has visibility policy sets.</param>
    /// <param name="hasPolicyEditable">Indicate if the current component has editable policy sets.</param>
    /// <returns>True if the policies parameters must be evaluated.</returns>
    internal static bool InitPolicyState(ref PolicyState policyState, LgAuthorizeView parentPolicy, bool hasPolicyVisible, bool hasPolicyEditable)
    {
        bool result = hasPolicyVisible || hasPolicyEditable || parentPolicy is not null;
        if (policyState is null)
        {
            policyState = new(!hasPolicyVisible, !hasPolicyEditable && parentPolicy is null);
        }
        else if (!result)
        {
            policyState.Visible = true;
            policyState.Editable = true;
        }
        return result;
    }

    #endregion

    #region methods - static

    /// <summary>
    /// Create lambda expression from function
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static LambdaExpression GetLambdaExpression<TItem, TResult>(Expression<Func<TItem, TResult>> expression)
    {
        return Expression.Lambda(expression.Body, expression.Parameters);
    }

    #endregion

}