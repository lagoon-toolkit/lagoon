using Lagoon.Core.Models;
using Lagoon.UI.Application;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop.WebAssembly;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lagoon.UI.Components;

/// <summary>
/// Extensions methods
/// </summary>
public static class Extensions
{

    #region IJSRuntime extensions

    /// <summary>
    /// Remove a database from IndexedDB
    /// </summary>
    /// <param name="js">IJSRuntime extension</param>
    /// <param name="dbName">Database name to remove</param>
    internal static async Task RemoveIndexedDbAsync(this IJSRuntime js, string dbName)
    {
        await js.InvokeVoidAsync("Lagoon.JsUtils.DeleteDatabase", dbName);
    }

    /// <summary>
    /// Return the loading animation defined in index.html (cf app-init.js)
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <returns></returns>
    internal static async Task<string> GetLoadingAnimationAsync(this IJSRuntime js)
    {
        return await js.InvokeAsync<string>("Lagoon.LoadingParams.GetOriginalLoadingAnimation");
    }

    /// <summary>
    /// Return true if localstorage contain a key with 'LgEula' + application base path. False otherwise
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <param name="keyState">LocalStorage key for eula validation state</param>
    /// <param name="keyDico">LocalStorage key for eula definition</param>
    /// <returns>Return true if localstorage contain a key with 'LgEula' + application base path. False otherwise</returns>
    internal static bool HasAcceptedCgu(this IJSInProcessRuntime js, string keyState, string keyDico)
    {
        return js.Invoke<bool>("Lagoon.JsUtils.HasAcceptedCgu", keyState, keyDico, "EulaVersion".Translate());
    }

    /// <summary>
    /// Return true if localstorage contain a key with 'LgEula' + application base path. False otherwise
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <param name="key">LocalStorage key</param>
    /// <returns>Return true if localstorage contain a key with 'LgEula' + application base path. False otherwise</returns>
    internal static string[] GetCguSupportedLanguage(this IJSInProcessRuntime js, string key)
    {
        return js.Invoke<string[]>("Lagoon.JsUtils.GetCguSupportedLanguage", key);
    }

    /// <summary>
    /// Accept the last version of EULA
    /// </summary>
    /// <param name="js">IJSRuntime extension</param>
    /// <param name="storageKey">LocalStorage key</param>
    internal static bool AcceptCgu(this IJSInProcessRuntime js, string storageKey)
    {
        return js.Invoke<bool>("Lagoon.JsUtils.AcceptCgu", storageKey, "EulaVersion".Translate());
    }

    /// <summary>
    /// Lauch Lagoon service worker initialisation process.
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <param name="app">Reference to LgApplication to allow communication between sw and the app</param>
    /// <param name="applicationRootName">ApplicationName. Used a IndexedDb prefix</param>
    internal static async Task InitLagoonServiceWorkerAsync(this IJSRuntime js, DotNetObjectReference<LgApplication> app, string applicationRootName)
    {
        await js.InvokeVoidAsync("Lagoon.ServiceWorker.Init", app, applicationRootName);
    }

    /// <summary>
    /// Try to unregister all services workers
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    public static async Task UnregisterServiceWorkerAsync(this IJSRuntime js)
    {
        await js.InvokeVoidAsync("Lagoon.ServiceWorker.Unregister");
    }

    /// <summary>
    /// Lauch Lagoon service worker initialisation process.
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <param name="app">Reference to LgApplication to allow communication between sw and the app</param>
    /// <param name="applicationRootName">ApplicationName. Used a IndexedDb prefix</param>
    internal static async Task RegisterUiUpdateServiceWorkerAsync(this IJSRuntime js, DotNetObjectReference<LgApplication> app, string applicationRootName)
    {
        await js.InvokeVoidAsync("Lagoon.ServiceWorker.SubscribeUiUpdate", app, applicationRootName);
    }

    /// <summary>
    /// Try to subscribe to WebPush notification. Should not be called directly, <see cref="LgApplication.SubscribeToPushNotificationAsync" />
    /// </summary>
    /// <param name="js">IJSRuntime extension</param>
    /// <param name="publicKey">VAPID public key</param>
    /// <returns></returns>
    internal static async Task<WebPushSubscription> SubcribeToPushNotificationAsync(this IJSRuntime js, string publicKey)
    {
        try
        {
            return await js.InvokeAsync<WebPushSubscription>("Lagoon.ServiceWorker.SubscribePushNotification", publicKey);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Unregister the subscription to WebPush notification. Should not be called directly, <see cref="LgApplication.UnsubscribeToPushNotificationAsync" />
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    internal static async Task<string> UnsubscribeToPushNotificationAsync(this IJSRuntime js)
    {
        return await js.InvokeAsync<string>("Lagoon.ServiceWorker.UnsubscribePushNotification");
    }

    /// <summary>
    /// Check if the user has accepted WebPush notification
    /// </summary>
    /// <param name="js">IJSRuntime extension</param>
    /// <returns><c>true</c> if the user has already accepted notification for this domain, <c>false</c> otherwise</returns>
    public static bool IsPushNotificationGranted(this IJSInProcessRuntime js)
    {
        return js.Invoke<bool>("Lagoon.ServiceWorker.PushNotificationAllowed");
    }

    internal static async Task<bool> HasSubscribedToWebPushNotificationAsync(this IJSInProcessRuntime js)
    {
        return await js.InvokeAsync<bool>("Lagoon.ServiceWorker.HasSubscribedToWebPushNotification");
    }

    /// <summary>
    /// Check if there is an active service worker
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    public static bool IsServiceWorkerActive(this IJSInProcessRuntime js)
    {
        return js.Invoke<bool>("Lagoon.ServiceWorker.IsServiceWorkerActive");
    }

    /// <summary>
    /// Include a script tag in the DOM (if not already exist)
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <param name="scriptPath">Script src to include</param>
    /// <param name="addAppVersion">Append the appVersion in the script path</param>
    public static ValueTask<string> ScriptIncludeAsync(this IJSRuntime js, string scriptPath, bool addAppVersion = true)
    {
        return js.InvokeAsync<string>("Lagoon.ScriptsManager.Add", scriptPath, addAppVersion);
    }

    /// <summary>
    /// Call a function which return a JS object (JsObjectRef)
    /// </summary>
    /// <param name="js">Extension for IJSRuntime</param>
    /// <param name="fn">Function to call (which return an JS Object)</param>
    /// <param name="fnArgs">Optional args for fn</param>
    /// <returns>A JsObjectRef which should be passed as an argument to js function</returns>
    public static async Task<JsObjectRef> ScriptGetNewRefAsync(this IJSRuntime js, string fn, params object[] fnArgs)
    {
        // Wrap the call inside JsObjectManager.Add method and retrieve the JsObjectRef created by fn
        JsObjectRef jsObjRef = await js.InvokeAsync<JsObjectRef>("Lagoon.JsObjectManager.ScriptGetNewRef", fn, fnArgs);
        // Inject the JSRuntime (to be able to call JS for JsObjectRef's Dispose call)
        jsObjRef.JS = js;
        return jsObjRef;
    }

    /// <summary>
    /// Download a file from url.
    /// </summary>
    /// <param name="js">IJSRuntime extension</param>
    /// <param name="url">Download url</param>        
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Replace \"JS.DownloadFromUrl\" by \"App.DownloadAsync\" method.")]
    public static void DownloadFromUrl(this IJSRuntime js, string url)
    {
        if (js is WebAssemblyJSRuntime webAssemblyJSRuntime)
        {
            webAssemblyJSRuntime.Invoke<object>("Lagoon.JsFileUtils.OpenURL", url);
        }
    }

    /// <summary>
    /// Send a text file to the browser (text/csv)
    /// </summary>
    /// <param name="js">JSRuntime extension</param>
    /// <param name="filename">Filename</param>
    /// <param name="contents">File content</param>
    /// <param name="inline">True if navigator should open the file, otherwise false</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the \"App.SaveAsFile\" method with \".csv\" for the file name parameter.")]
    public static void DownloadCsv(this IJSRuntime js, string filename, string contents, bool inline = false)
    {
        js.DownloadText(filename, contents, "text/csv", new UTF8Encoding(false), inline);
    }

    /// <summary>
    /// Send a text file to the browser (text/plain)
    /// </summary>
    /// <param name="js">The JavaScript runtime.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="inline">True if navigator should open the file, otherwise false</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the \"App.SaveAsFile\" method.")] //13/07/2022
    public static void DownloadText(this IJSRuntime js, string filename, string contents, bool inline = false)
    {
        js.DownloadText(filename, contents, Tools.ExtrapolateContentType(filename), inline);
    }

    /// <summary>
    /// Send a text file to the browser (text/plain)
    /// </summary>
    /// <param name="js">The JavaScript runtime.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="inline">True if navigator should open the file, otherwise false</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the \"App.SaveAsFile\" method.")] //13/07/2022
    public static void DownloadText(this IJSRuntime js, string filename, string contents, Encoding encoding, bool inline = false)
    {
        js.DownloadText(filename, contents, Tools.ExtrapolateContentType(filename), encoding, inline);
    }

    /// <summary>
    /// Send a text file to the browser (text/plain)
    /// </summary>
    /// <param name="js">The JavaScript runtime.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <param name="inline">True if navigator should open the file, otherwise false</param>
    /// <remarks>This method uses UTF-8 encoding without a Byte-Order Mark (BOM).
    /// If it is necessary to include a UTF-8 identifier, such as a byte order mark, at the beginning of a file,
    /// use the <see cref="DownloadText(IJSRuntime, string, string, string, Encoding, bool)"/> method overload with <see cref="Encoding.UTF8"/> encoding.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the \"App.SaveAsFile\" method.")] //13/07/2022
    public static void DownloadText(this IJSRuntime js, string filename, string contents, string contentType, bool inline = false)
    {
        js.DownloadText(filename, contents, contentType, new UTF8Encoding(false), inline);
    }

    /// <summary>
    /// Send a text based file to the browser.
    /// </summary>
    /// <param name="js">The JavaScript runtime.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="contents">The string to write to the file.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <param name="encoding">The encoding to apply to the string.</param>
    /// <param name="inline">True if navigator should open the file, otherwise false</param>
    [Obsolete("Use the \"App.SaveAsFile\" method.")] //13/07/2022
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DownloadText(this IJSRuntime js, string filename, string contents, string contentType, Encoding encoding, bool inline = false)
    {
        byte[] file;
        byte[] bom = encoding.GetPreamble();
        byte[] text = encoding.GetBytes(contents);
        if (bom.Length > 0)
        {
            // Concat the BOM and the text content
            file = new byte[bom.Length + text.Length];
            bom.CopyTo(file, 0);
            text.CopyTo(file, bom.Length);
        }
        else
        {
            // There's no BOM
            file = text;
        }
        js.DownloadRawContent(filename, contentType, file, inline);
    }

    /// <summary>
    /// Send a raw file to the browser.
    /// </summary>
    /// <param name="js">JSRuntime extension</param>
    /// <param name="filename">Browser download file name</param>
    /// <param name="contentType">Content Type</param>
    /// <param name="content">File content</param>
    /// <param name="inline">True if navigator should open the file, otherwise false</param>
    [Obsolete("Use the \"App.SaveAsFile\" method.")] //13/07/2022
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DownloadRawContent(this IJSRuntime js, string filename, string contentType, byte[] content, bool inline = false)
    {
        if (js is WebAssemblyJSRuntime webAssemblyJSRuntime)
        {
            string target = inline ? "_blank" : "";
            webAssemblyJSRuntime.InvokeUnmarshalled<string, byte[], byte[], bool>("Lagoon.JsFileUtils.OpenBlobUnmarshalled",
                $"{filename}|{target}|{contentType}", content, null);
        }
        else
        {
            throw new Exception("The WebAssemblyJSRuntime can't be found !");
        }
    }

    /// <summary>
    /// Focus the element by its reference
    /// </summary>
    /// <param name="js">JSRuntime extension</param>
    /// <param name="elementReference">Element reference</param>
    /// <param name="delay">Executing focus delay in ms</param>
    public static ValueTask FocusAsync(this IJSRuntime js, ElementReference elementReference, int delay = 100)
    {
        return js.InvokeVoidAsync("Lagoon.JsUtils.focusElementByReference", elementReference, null, delay);
    }

    /// <summary>
    /// Focus the element by selector inside parent
    /// </summary>
    /// <param name="js">JSRuntime extension</param>
    /// <param name="parentElementReference">Parent element reference</param>        
    /// <param name="selector">CSS selector</param>
    /// <param name="delay">Executing focus delay in ms</param>
    /// <returns></returns>
    public static ValueTask FocusAsync(this IJSRuntime js, ElementReference parentElementReference, string selector, int delay = 100)
    {
        return js.InvokeVoidAsync("Lagoon.JsUtils.focusElementByReference", parentElementReference, selector, delay);
    }

    /// <summary>
    /// Focus the element by selector
    /// </summary>
    /// <param name="js">JSRuntime extension</param>        
    /// <param name="selector">CSS selector</param>        
    /// <returns></returns>
    public static ValueTask FocusAsync(this IJSRuntime js, string selector)
    {
        return js.InvokeVoidAsync("Lagoon.JsUtils.focusElement", selector);
    }

    #endregion

    #region EventCallBackExtension

    /// <summary>
    /// Excute the EventCallback handler and show the error in the toastr if there's an error.
    /// </summary>
    /// <param name="eventCallback">The evntCallback.</param>
    /// <param name="app">The application instance.</param>
    public static async Task TryInvokeAsync(this EventCallback eventCallback, LgApplication app)
    {
        try
        {
            await eventCallback.InvokeAsync();
        }
        catch (Exception ex)
        {
            app.ShowException(ex);
        }
    }

    /// <summary>
    /// Excute the EventCallback handler and show the error in the toastr if there's an error.
    /// </summary>
    /// <param name="eventCallback">The evntCallback.</param>
    /// <param name="app">The application instance.</param>
    /// <param name="args">The event callback argument.</param>
    public static async Task TryInvokeAsync<TValue>(this EventCallback<TValue> eventCallback, LgApplication app, TValue args)
    {
        try
        {
            await eventCallback.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            app.ShowException(ex);
        }
    }

    #endregion

    #region AuthenticationState extensions

    /// <summary>
    /// Gets the first value of the specified claim type if exists; <c>null</c> ortherwise.
    /// </summary>
    /// <param name="authenticationState">The authentication state.</param>
    /// <param name="key">The claim type to get the value from.</param>
    /// <returns>The first value of the specified claim type if exists; <c>null</c> ortherwise.</returns>
    public static string UserClaimValue(this AuthenticationState authenticationState, string key)
    {
        return authenticationState?.User?.Claims.Where(c => c.Type == key).FirstOrDefault()?.Value;
    }

    /// <summary>
    /// Gets all the values of the specified claim type if exists; <c>null</c> ortherwise.
    /// </summary>
    /// <param name="authenticationState">The authentication state.</param>
    /// <param name="key">The claim type to get the values from.</param>
    public static IEnumerable<string> UserClaimValues(this AuthenticationState authenticationState, string key)
    {
        return authenticationState?.User?.Claims.Where(x => x.Type == key).Select(p => p.Value);
    }

#if DEBUG

    /// <summary>
    /// Get all the user claim types.
    /// </summary>
    /// <param name="authenticationState">The authentication state.</param>
    /// <returns>All the user claim types.</returns>
    public static IEnumerable<string> UserClaimTypes(this AuthenticationState authenticationState)
    {
        return authenticationState?.User?.Claims.Select(p => p.Type).Distinct();
    }

#endif

    #endregion

}