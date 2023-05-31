using Lagoon.Core.Application.Logging;
using Lagoon.Internal;
using Lagoon.Shared;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Error log files controller.
/// </summary>
[ApiController]
[Route(Routes.LOG_ROUTE)]
[ApiExplorerSettings(IgnoreApi = true)]
public class LgLogController : LgControllerBase
{

    #region fields

    /// <summary>
    /// Retrieve the logger to trace error
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Access to application manager for signin user.
    /// </summary>
    private readonly ILgApplication _app;

    #endregion

    #region constructors

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="logger">DI Ilogger</param>
    /// <param name="app">Lagoon application manager.</param>
    public LgLogController(ILogger<LgLogController> logger, ILgApplication app)
    {
        _logger = logger;
        _app = app;
    }

    #endregion

    #region Log endpoints

    /// <summary>
    /// Trace an error
    /// </summary>
    /// <param name="error">Error to trace</param>
    /// <returns>True if the error successfully logged, false otherwise</returns>
    [HttpPost(Routes.LOG_ADD)]
    [Authorize]
    public IActionResult Add(ClientLogData error)
    {
        try
        {
            LogClientApplicationException(error);
            return Ok();
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    /// <summary>
    /// Trace an error list
    /// </summary>
    /// <param name="list">List of ClientLogData</param>
    /// <returns>True if the error successfully logged, false otherwise</returns>
    [HttpPost(Routes.LOG_ADD_LIST)]
    [Authorize]
    public IActionResult Add(List<ClientLogData> list)
    {
        try
        {
            foreach (ClientLogData logData in list)
            {
                LogClientApplicationException(logData);
            }
            return Ok();
        }
        catch (Exception)
        {
            return Problem();
        }
    }

    /// <summary>
    /// Save the exception information from the client application in a new exception.
    /// </summary>
    /// <param name="error">The client application exception.</param>
    private void LogClientApplicationException(ClientLogData error)
    {
        _logger.Log(error.LogLevel, new LgClientApplicationException(error), "Client application exception.");
    }

    /// <summary>
    /// Export application (server) logs file
    /// </summary>
    /// <returns>A zip file with all application logs</returns>
    [HttpGet(Routes.LOG_DOWNLOAD)]
    [Authorize(Policies.LogReader)]
    public IActionResult DownloadLog()
    {
        System.IO.MemoryStream ms = LgFileLoggerManager.ExtractLogFile();
        return ms != null ? File(ms, "application/gzip", $"{_app.ApplicationInformation.RootName}Log{DateTime.Now}.log.gz") : NoContent();
    }

    /// <summary>
    /// Export application (server) logs data
    /// </summary>
    /// <param name="filter">A string to filter the levels to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An item list.</returns>
    [HttpGet]
    [Route(Routes.LOG_ENTRIES)]
    [Authorize(Policies.LogReader)]
    public ActionResult<IEnumerable<LogData>> GetServerLog(string filter, CancellationToken cancellationToken)
    {
        return Ok(LgFileLoggerManager.ExtractLogData(filter, cancellationToken));
    }

}

#endregion

