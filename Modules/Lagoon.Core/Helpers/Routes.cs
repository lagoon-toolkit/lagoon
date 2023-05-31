#pragma warning disable CS1591 // Disable XML comments

namespace Lagoon.Internal;

/// <summary>
/// Routes des controlleurs internes à Lagoon.
/// </summary>
public static class Routes
{

    /// <summary>
    /// LgConfigurationController.
    /// </summary>
    public const string CONFIGURATION_ROUTE = "api/-/configuration";

    // Warning : urls used without constants call in service-worker-handler.js

    public const string CONFIGURATION_GET_URI = CONFIGURATION_ROUTE;

    /// <summary>
    /// LgDicoController.
    /// </summary>
    public const string DICO_ROUTE = "api/-/dico";

    public const string DICO_CSV = "dico-csv";
    public const string DICO_CSV_URI = DICO_ROUTE + "/" + DICO_CSV;

    public const string DICO_KEYS = "dico-keys";
    public const string DICO_KEYS_URI = DICO_ROUTE + "/" + DICO_KEYS;

    public const string DICO_UPDATE = "dico-update";
    public const string DICO_UPDATE_URI = DICO_ROUTE + "/" + DICO_UPDATE;

    /// <summary>
    /// LgEulaController.
    /// </summary>
    public const string EULA_ROUTE = "api/-/eula";

    public const string EULA_GET_URI = EULA_ROUTE;

    public const string EULA_VERSION = "eula-version";
    public const string EULA_VERSION_URI = EULA_ROUTE + "/" + EULA_VERSION;

    public const string EULA_UPDATE = "eula-update/{updateVersion}";
    public const string EULA_UPDATE_URI = EULA_ROUTE + "/eula-update/";

    public const string EULA_VERSION_UPDATE = "eula-version-update";
    public const string EULA_VERSION_UPDATE_URI = EULA_ROUTE + "/" + EULA_VERSION_UPDATE;

    /// <summary>
    /// The virtual path of the application launcher.
    /// </summary>
    public const string INDEX_PATH = "/index.html";

    /// <summary>
    /// LgLogController.
    /// </summary>
    public const string LOG_ROUTE = "api/-/log";

    public const string LOG_ADD = "log-add";
    public const string LOG_ADD_URI = LOG_ROUTE + "/" + LOG_ADD;

    public const string LOG_ADD_LIST = "log-add-list";
    public const string LOG_ADD_LIST_URI = LOG_ROUTE + "/" + LOG_ADD_LIST;

    public const string LOG_DOWNLOAD = "log-download";
    public const string LOG_DOWNLOAD_URI = LOG_ROUTE + "/" + LOG_DOWNLOAD;

    public const string LOG_ENTRIES = "log-entries";
    public const string LOG_ENTRIES_URI = LOG_ROUTE + "/" + LOG_ENTRIES;

    /// <summary>
    /// LgNotificationBaseController.
    /// </summary>
    public const string NOTIFICATIONS_ROUTE = "api/-/notification";

    /// <summary>
    /// LgServiceWorkerAssetsController.
    /// </summary>
    public const string SW_ASSETS_ROUTE = "api/-/sw-assets";

    /// <summary>
    /// LgTabsController.
    /// </summary>
    public const string TABS_ROUTE = "api/-/tabs";

    /// <summary>
    /// The virtual root path where resource files are generated.
    /// </summary>
    public const string VIRTUAL_ROOT_PATH = "/_vroot/";

}
