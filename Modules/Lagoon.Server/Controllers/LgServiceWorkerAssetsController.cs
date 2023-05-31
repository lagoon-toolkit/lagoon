using Lagoon.Internal;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Ping controller
/// </summary>
[Route(Routes.SW_ASSETS_ROUTE)]
public class LgServiceWorkerAssetsController : Controller
{

    #region constants

    /// <summary>
    /// The path of build service-worker-assets.js.
    /// </summary>
    public const string SW_ASSETS_FILE = "/service-worker-assets.js";

    #endregion

    #region fields

    /// <summary>
    /// The main application
    /// </summary>
    private readonly ILgApplication _app;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application.</param>
    public LgServiceWorkerAssetsController(ILgApplication app)
    {
        _app = app;
    }

    #endregion

    #region methods

    /// <summary>
    /// Return active application indicator
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 2592000)] // (1 month)
    public IActionResult Get()
    {
        // Try to load the assets file
        Microsoft.Extensions.FileProviders.IFileInfo sourceFile = _app.WebRootFileProvider.GetFileInfo(SW_ASSETS_FILE);
        if (!sourceFile.Exists)
        {
            return NotFound();
        }
        // Extract the assets definitions
        AssetsManifest manifest;
        string js;
        int start;
        int end;
        using (Stream stream = sourceFile.CreateReadStream())
        {
            js = new StreamReader(stream).ReadToEnd();
            start = js.IndexOf('{');
            end = js.LastIndexOf('}') + 1;
            manifest = JsonSerializer.Deserialize<AssetsManifest>(js[start..end]);
        }
        // Update the offline asset list
        OfflineAssetManager manager = new(manifest.Assets);
        _app.LoadOfflineAssets(manager);
        // Remove the excluded assets
        manifest.Assets = manifest.Assets.Where(a => !a.Exclude).ToList();
        // Send the modified file
        StringBuilder sb = new(js[0..start], js.Length);
        sb.Append(JsonSerializer.Serialize(manifest));
        sb.Append(js[end..]);
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/javascript; charset=UTF-8");
    }

    #endregion

    #region private classes

    private class AssetsManifest
    {
        [JsonPropertyName("assets")]
        public List<OfflineAsset> Assets { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

    }

   #endregion

}
