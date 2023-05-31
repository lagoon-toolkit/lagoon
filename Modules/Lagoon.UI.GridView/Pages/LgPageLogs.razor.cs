using Lagoon.Core.Application.Logging;
using Lagoon.Internal;
using Lagoon.Shared;
using Lagoon.UI.Application.Logging;

namespace Lagoon.UI.GridView.Pages;

/// <summary>
/// Page to download application logs.
/// </summary>
[Authorize(Policy = Policies.LogReader)]
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

    #region LogDataItem class

    [JsonConverter(typeof(LogDataConverter<LogDataItem>))]
    private class LogDataItem : LogData, IEquatable<LogDataItem>
    {

        /// <summary>
        /// Automatic Id for the log entry.
        /// </summary>
        public Guid Id { get; set; }


        public LogDataItem(ClientLogData error) : this()
        {
            Time = error.Time;
            LogLevel = error.LogLevel;
            IsAppCategory = error.IsAppCategory;
            Category = error.Category;
            Message = error.Message;
            Side = LogSide.Client;
            StackTrace = error.StackTrace;
        }

        public LogDataItem()
        {
            Id = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LogDataItem);
        }

        public bool Equals(LogDataItem other)
        {
            return Id == other?.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }

    #endregion

    #region fields

    /// <summary>
    /// Page loading status.
    /// </summary>
    private bool _isLoading = true;

    /// <summary>
    /// All loaded errors
    /// </summary>
    private List<LogDataItem> _entries;

    /// <summary>
    /// Minimal level to show.
    /// </summary>
    private string _minimumLevel = "WEC";

    private LogDataItem _selectedLog;

    #endregion

    #region Initialization

    /// <summary>
    /// Method to load data for the current page.
    /// </summary>
    /// <param name="e"></param>
    protected override async Task OnLoadAsync(PageLoadEventArgs e)
    {
        try
        {
            await base.OnLoadAsync(e);
            // Initialize the title and the icon of the page
            await SetTitleAsync(Link());
            // Download server messages and client error stores in the datastore
            await LoadLogAsync(e.CancellationToken);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Reload the load content.
    /// </summary>
    private async Task MinimumLevelChangeAsync()
    {
        _isLoading = true;
        using (WaitingContext wc = GetNewWaitingContext())
        {
            await LoadLogAsync(wc.CancellationToken);
        }
    }

    /// <summary>
    /// Download server messages and client error stores in the datastore
    /// </summary>
    private async Task LoadLogAsync(CancellationToken cancellationToken)
    {
        // Start the download
        Task<List<LogDataItem>> task = GetServerLogAsync(cancellationToken);
        IEnumerable<LogDataItem> localErrorsQuery = LgClientLoggerManager.GetLocalErrors(App).Select(e => new LogDataItem(e));
        if (_minimumLevel == "C")
        {
            localErrorsQuery = localErrorsQuery.Where(e => e.LogLevel == LogLevel.Critical);
        }
        List<LogDataItem> localErrors = localErrorsQuery.ToList();
        _entries = await task;
        if (!cancellationToken.IsCancellationRequested)
        {
            // Merge lists
            _entries.AddRange(localErrors);
            _entries.Sort(SortByTimeDesc);
            // End of loading
            _isLoading = false;
        }
    }

    private int SortByTimeDesc(LogData x, LogData y)
    {
        return x is null ? y is null ? 0 : -1 : y is null ? 1 : x.Time.CompareTo(y.Time);
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

    /// <summary>
    /// Download the application log on server side
    /// </summary>
    private async Task<List<LogDataItem>> GetServerLogAsync(CancellationToken cancellationToken)
    {
        List<LogDataItem> result = null;
        try
        {
            result = await Http.TryGetAsync<List<LogDataItem>>($"{Routes.LOG_ENTRIES_URI}?filter={_minimumLevel}", cancellationToken);
        }
        catch (Exception)
        {
            ShowWarning("Unable to get server logs.");
        }
        return result;
    }

    #endregion

    #region GridView events

    private void ShowLogDetails(GridViewSelectionEventArgs<LogDataItem> args)
    {
        try
        {
            _selectedLog = _selectedLog != null && _selectedLog.Equals(args.SelectedItem) ? null : args.SelectedItem;
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

    #region LogLevel colors

    /// <summary>
    /// Get the color of the level.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns></returns>
    private static string GetLevelColor(LogLevel level)
    {
        string var = level switch
        {
            LogLevel.Information => "--green",
            LogLevel.Warning => "--warning",
            LogLevel.Error => "--danger",
            LogLevel.Critical => "--purple",
            _ => "--blue",
        };
        return $"color:var({var})";
    }

    #endregion

}
