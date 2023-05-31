using Microsoft.JSInterop.WebAssembly;

namespace Lagoon.UI.Helpers;

/// <summary>
/// Handler for Javascript references.
/// </summary>
public class JsObjectRef : IAsyncDisposable
{

    /// <summary>
    /// Need a reference to JSRuntime for disposing a JsObjectRef on JS side
    /// </summary>
    public IJSRuntime JS { get; set; }

    /// <summary>
    /// This key is used to map this object with an JS object
    /// </summary>
    [JsonPropertyName("__jsObjectRefId")]
    public int JsObjectRefId { get; set; }

    /// <summary>
    /// Remove the reference on JS Side
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
        return JS.InvokeVoidAsync("Lagoon.JsObjectManager.Free", JsObjectRefId);
    }

    /// <summary>
    /// Dispose reference
    /// </summary>
    public void Dispose()
    {
        if (JS is WebAssemblyJSRuntime webAssemblyJSRuntime)
        {
            webAssemblyJSRuntime.InvokeVoid("Lagoon.JsObjectManager.Free", JsObjectRefId);
        }
        else throw new InvalidCastException("You must use the dispose async");
    }

}
