namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageLoader : DemoPage
{

    #region constants

    /// <summary>
    /// Seconds
    /// </summary>
    private const int DELAY = 4;

    #endregion

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/loader";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Loader", IconNames.All.ArrowClockwise);
    }

    #endregion

    #region Fields

    private bool _isLoading;
    private bool _isLoadingComplex;

    private readonly Progress _progress = new();
    private Progress _progressComplex;
    private bool _isCircle;
    private int _loaderDelay = DELAY;

    #endregion

    #region methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        //x            StartCountdown();
        DocumentationComponent = "763";
    }

    ///<inheritdoc/>
    protected override async Task OnLoadAsync(PageLoadEventArgs e)
    {
        await base.OnLoadAsync(e);
        await RunAllTaskAsync();
    }

    private Task RunAllTaskAsync()
    {
        Task task1 = MyTaskWithoutProgressionAsync();
        Task task2 = MyTaskWithProgressionAsync();
        return Task.WhenAll(task1, task2);
    }

    private async Task MyTaskWithoutProgressionAsync()
    {
        _isLoading = true;
        await Task.Delay(_loaderDelay * 1000);
        _isLoading = false;
    }

    private async Task MyTaskWithProgressionAsync()
    {
        _progress.Reset(58);
        for (int i = 0; i <= 58; i++)
        {
            await Task.Delay(_loaderDelay * 10);
            _progress.Report(i, $"Etape {i} / {_progress.EndPosition}");
        }
    }

    private async Task MyMainTaskAsync(bool parallel)
    {
        _isLoadingComplex = true;
        DateTime start = DateTime.UtcNow;
        _progressComplex = new Progress(100);
        if (!parallel)
        {
            await MySubTaskAsync(_progressComplex.OpenSubProgress(50), "A");
            await MySubTaskAsync(_progressComplex.OpenSubProgress(50), "B");
        }
        else
        {
            _progressComplex.Report("Tasks A + B");
            await Task.WhenAll(
                MySubTaskAsync(_progressComplex.OpenSubProgress(50, true), "A"),
                MySubTaskAsync(_progressComplex.OpenSubProgress(50, true), "B")
                );
        }
        ShowInformation($"MyMainTask : {DateTime.UtcNow.Subtract(start).TotalMilliseconds} s");
        _isLoadingComplex = false;
    }

    private async Task MySubTaskAsync(Progress progress, string id)
    {
        progress.Reset(100);
        for (int i = 0; i <= 100; i++)
        {
            await Task.Delay(_loaderDelay * 5);
            progress.Report(i, $"Task {id}: {i}%");
        }
    }

    /*private void ChangeDelay()
    {
        StateHasChanged();
    }*/

    #endregion

}
