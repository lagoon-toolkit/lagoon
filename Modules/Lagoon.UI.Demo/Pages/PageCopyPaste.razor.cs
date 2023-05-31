namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageCopyPaste : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/CopyPaste";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "CopyPaste", IconNames.All.ClipboardPlus);
    }

    #endregion

    #region parameters
    private string TextValue { get; set; } = "Value";
    private string PastedTextValue { get; set; }
    private double DoubleValue { get; set; } = 10.3;
    private double PastedDoubleValue { get; set; }
    private DateTime DateValue { get; set; } = DateTime.Now;
    private DateTime PastedDateValue { get; set; }
    #endregion

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "1315";
    }

    private void OnPasteText(ClipboardTextEventArg args)
    {
        PastedTextValue = args.Value;
    }

    private void OnPasteDoubleValue(ClipboardTextEventArg args)
    {
        try
        {
            ShowInformation("Double : " + args.Value.ToString());
            PastedDoubleValue = double.Parse(args.Value.ToString());
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    private void OnCopyDate(ClipboardTextEventArg args)
    {
        args.Value = DateTime.Now.ToString();
        ShowSuccess(DateTime.Now.ToString());
    }

    private void OnPasteDate(ClipboardTextEventArg args)
    {
        try
        {
            ShowInformation(args.Value.ToString());
            PastedDateValue = DateTime.Parse(args.Value.ToString());
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

}
