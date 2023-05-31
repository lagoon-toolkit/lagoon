using Demo.Client.Services;
using NavigationModeService = Lagoon.UI.Demo.Pages.NavigationModeService;

namespace Demo.Client.Shared;

public partial class MenuConfiguration
{
    #region constants

    private const string LOCAL_STORAGE = "FavoritePages";

    #endregion

    #region fields

    private DateTime _start;
    private List<string> _favorites;

    #endregion

    #region dependencies injections

    /// <summary>
    /// Navigation mode handler.
    /// </summary>
    [Inject]
    public NavigationModeService Navigation { get; set; }

    #endregion

    #region methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Navigation.OnNavigationModeChanged += OnNavigationModeChanged;
    }

    protected override void Dispose(bool disposing)
    {
        Navigation.OnNavigationModeChanged -= OnNavigationModeChanged;
        base.Dispose(disposing);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _favorites = App.LocalStorage.GetItem<List<string>>(LOCAL_STORAGE);
    }

    private int CompareLinkTitle(LgPageLink x, LgPageLink y)
    {
        return string.CompareOrdinal(x.Title.CheckTranslate(), y.Title.CheckTranslate());
    }

    protected override void OnParametersSet()
    {
        _start = DateTime.Now;
        base.OnParametersSet();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        Console.WriteLine("Menu configuration : " + NavigationManager.Uri + " ==> " + DateTime.Now.Subtract(_start).TotalMilliseconds.ToString() + "ms");
    }

    private void ToggleFavorite()
    {
        string path = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        _favorites ??= new();
        int index = _favorites.IndexOf(path);
        if (index >= 0)
        {
            ShowInformation($"Remove {path} to the favorites");
            _favorites.RemoveAt(index);
        }
        else
        {
            ShowInformation($"Add {path} from the favorites");
            _favorites.Add(path);
        }
        App.LocalStorage.SetItem(LOCAL_STORAGE, _favorites);
    }

    private void OnNavigationModeChanged()
    {
        StateHasChanged();
    }

    #endregion

}
