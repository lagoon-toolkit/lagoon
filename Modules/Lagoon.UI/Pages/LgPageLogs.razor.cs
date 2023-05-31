using Lagoon.Core.Application.Logging;
using Lagoon.Internal;
using Lagoon.UI.Application.Logging;

namespace Lagoon.UI.Pages;

/// <summary>
/// Page to download application logs.
/// </summary>
[Authorize()]
[Route(ROUTE)]
public partial class LgPageLogs : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "lg/trace";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgLogsTitle", IconNames.All.Bug);
    }

    /// <summary>
    /// Local errors
    /// </summary>
    public List<ClientLogData> LogData { get; set; }

    #endregion

    #region Private properties

    /// <summary>
    /// Local errors
    /// </summary>
    private List<ClientLogData> _localErrors = null;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            await SetTitleAsync(Link());
            _localErrors = LgClientLoggerManager.GetLocalErrors(App);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

    #region Download / Clear client side error

    /// <summary>
    /// Send error file to the browser
    /// </summary>
    private void DownloadLog()
    {
        try
        {
            // Prepare log output
            StringBuilder log = new();
            foreach (ClientLogData error in _localErrors)
            {
                log.AppendLine(FormatError(error));
            }
            // Send file
            App.SaveAsFile($"Log_{DateTime.Now}.txt", log.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Exception as text format
    /// </summary>
    private static string FormatError(ClientLogData error)
    {
        return $"Date:{error.Time}\r\nMessage :{error.Message}\r\nStackTrace: {error.StackTrace?.Replace("\r\n", "\\r\\n")}\r\n";
    }

    /// <summary>
    /// Clear local error
    /// </summary>
    private void ClearErrorAsync()
    {
        try
        {
            LgClientLoggerManager.ClearLocalErrors(App);
            _localErrors.Clear();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

    #region Download server side application logs

    /// <summary>
    /// Download the application log on server side
    /// </summary>
    private async Task DownloadServerLogAsync()
    {
        try
        {
            await App.DownloadAsync(Routes.LOG_DOWNLOAD_URI);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

}
