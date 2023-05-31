namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageProgress : DemoPage
{
    #region constants

    /// <summary>
    /// Seconds
    /// </summary>
    private const int DELAY = 4;

    /// <summary>
    /// Max progression
    /// </summary>
    private const int MAX = 58;

    #endregion

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/progress";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Progress", IconNames.All.Wrench);
    }

    #endregion

    private string Label { get; set; } = "Label";
    private int Value { get; set; }

    private int ValueError { get; set; }
    private Kind KindError { get; set; } = Kind.Primary;
    private Kind Kind { get; set; } = Kind.Primary;
    private int MinValue { get; set; } = 0;
    private int MaxValue { get; set; } = MAX;
    private bool Stripped { get; set; } = true;
    private bool Animated { get; set; } = true;
    private bool KindAutoSuccess { get; set; } = false;

    private ProgressLabelPosition ProgressLabelPosition { get; set; } = ProgressLabelPosition.Center;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "815";
    }

    protected override async Task OnLoadAsync(PageLoadEventArgs e)
    {
        await base.OnLoadAsync(e);
        await MySimpleTaskAsync();
    }

    private Task RunAllTasksAsync()
    {
        return Task.WhenAll(MySimpleTaskAsync(), MyTaskWithErrorAsync());
    }

    private async Task MySimpleTaskAsync()
    {
        for (int i = 0; i <= MAX; i++)
        {
            Value = i;
            await Task.Delay(DELAY * 10);
            StateHasChanged();
        }
    }
    private async Task MyTaskWithErrorAsync()
    {
        try
        {
            KindError = Kind.Primary;
            for (int i = 0; i <= MAX; i++)
            {
                ValueError = i;
                if (ValueError > MAX / 3)
                {
                    KindError = Kind.Warning;
                }
                if (ValueError > 2 * MAX / 3)
                {
                    throw new Exception("Progression error simulation !");
                }
                await Task.Delay(DELAY * 10);
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            KindError = Kind.Error;
            //                ValueError = MAX;
            ShowException(ex);
        }
    }

    public void OnUpdateLabel(ChangeEventArgs args)
    {
        Label = (string)args.Value;
    }

    public void OnChangeKind(Kind kind)
    {
        Kind = kind;
        StateHasChanged();
    }

}