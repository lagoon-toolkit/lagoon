namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageTagBox : DemoPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/TagBox";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "TagBox", IconNames.All.Tag);
    }

    #endregion

    public string Label { get; set; } = "Label";
    public bool Disable { get; set; } = false;
    public bool Readonly { get; set; } = false;

    public List<string> TagValue { get; set; } = new List<string>();

    #region Method
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "612";
    }

    #endregion

    public void TextHasChanged(string text)
    {
        Label = text;
    }

    public void DisableHasChanged(bool isDisable)
    {
        Disable = isDisable;
    }

    public void ReadonlyHasChanged(bool isReadonly)
    {
        Readonly = isReadonly;
    }

    public static void OnChangeValue(ChangeEventArgs args)
    {
        List<string> elements = (List<string>)args.Value;
        System.Console.WriteLine(string.Join(',', elements));
        System.Console.WriteLine("#########");
    }

}
