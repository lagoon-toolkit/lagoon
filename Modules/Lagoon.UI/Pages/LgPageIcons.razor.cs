using System.Xml.Linq;

namespace Lagoon.UI.Pages;

/// <summary>
/// Page showing the icon library.
/// </summary>
[AllowAnonymous]
[Route(ROUTE)]
public partial class LgPageIcons : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "lg/icon-library";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgIconsTitle", IconNames.All.Grid3x3Gap);
    }

    #endregion

    #region private fields

    /// <summary>
    /// Names of all icons;
    /// </summary>
    private List<string> _appIcons;

    #endregion

    #region Private properties

    /// <summary>
    /// List of icons
    /// </summary>
    private List<string> ResultIcons { get; set; }

    /// <summary>
    /// Search term into list
    /// </summary>
    private string SearchTerm { get; set; }

    #endregion

    #region Initialization

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetTitleAsync(Link());
        await LoadIconListAsync();
    }

    /// <summary>
    /// Load icon library.
    /// </summary>
    private async Task LoadIconListAsync()
    {
        // Load icon list from navigator cached file
        await LoadIconNameListAsync();
        // How many icons to render on page load
        int max = 50;
        int initialIconRendering = _appIcons.Count > max ? max : _appIcons.Count;
        // Init list of icoons
        ResultIcons = _appIcons.Take(initialIconRendering).ToList();
        // Returns a task that will complete the icon list
        await Task.Run(() =>
        {
            ResultIcons.AddRange(_appIcons.Skip(initialIconRendering));
        });
    }

    /// <summary>
    /// Load icon names from SVG file.
    /// </summary>
    private async Task LoadIconNameListAsync()
    {
        List<string> list = new();

        // Load icon list from navigator cached file
        System.Net.Http.HttpResponseMessage response = await AnonymousHttpClient.GetAsync("icons.svg");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception("Error while trying to get \"icons.svg\".");
        }
        // Load as XML document
        XDocument doc = XDocument.Parse(await response.Content.ReadAsStringAsync());
        foreach(var node in doc.Root.Elements())
        {
            list.Add(node.Attribute("id")?.Value);
        }
        // Return the fullfill list
        _appIcons = list;
    }

    #endregion

    #region methods

    private static string GetAliasPrefix(string id)
    {
        if (id.StartsWith("i-a-")) return "i-a-";
        if (id.StartsWith("A-")) return "A-";
        return null;
    }

    /// <summary>
    /// Build constant name from svg id.
    /// </summary>
    /// <param name="id">SVG id.</param>
    /// <returns>Le nom de la constante.</returns>
    private static string BuildName(string id)
    {
        string prefix = GetAliasPrefix(id);
        if (prefix != null)
        {
            //IconNames.<Alias>
            return id[prefix.Length..];
        }
        else
        {
            //IconNames.All.<Id>
            bool ucase = true;
            System.Text.StringBuilder sb = new(id.Length + 4);
            foreach (char c in id)
            {
                switch (c)
                {
                    case '-':
                        ucase = true;
                        break;
                    default:
                        sb.Append(ucase ? char.ToUpper(c) : c);
                        ucase = false;
                        break;
                }
            }
            return sb.ToString(1, sb.Length - 1);
        }
    }

    /// <summary>
    /// Search icon action
    /// </summary>
    /// <param name="eventArgs">Event arguments</param>
    private void SearchIcon(ChangeEventArgs eventArgs)
    {
        string value = (string)eventArgs.Value;
        //Search only is search term contains at least three caracters
        if (value.Length >= 3)
        {
            //Search term into icon list
            ResultIcons = _appIcons.Where(x => x.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        else if (value.Length == 0)
        {
            //Reset list of icons
            ResultIcons = _appIcons.ToList();
        }
    }

    #endregion

}
