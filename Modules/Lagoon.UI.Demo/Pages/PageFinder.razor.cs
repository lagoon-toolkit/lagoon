using Lagoon.UI.Demo.ViewModel;

namespace Lagoon.UI.Demo.Pages;

[Route(ROUTE)]
public partial class PageFinder : DemoPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/finder";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "Finder", IconNames.All.Wrench);
    }

    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadData();
        SetTitle(Link());
        DocumentationComponent = "414";
    }

    private List<Pet> _petsItems = new();
    private List<Pet> _petsSource;

    private Pet _petFilter = new();

    private void LoadData()
    {
        _petsItems.Add(new Pet() { Id = _petsItems.Count + 1, Name = "Cala", Type = "Dog", Color = "Black" });
        _petsItems.Add(new Pet() { Id = _petsItems.Count + 1, Name = "Tigrou", Type = "Cat", Color = "White" });
        _petsItems.Add(new Pet() { Id = _petsItems.Count + 1, Name = "Caramel", Type = "Cat", Color = "Brown" });
        _petsItems.Add(new Pet() { Id = _petsItems.Count + 1, Name = "Jack", Type = "Horse", Color = "Brown" });
        _petsItems.Add(new Pet() { Id = _petsItems.Count + 1, Name = "Okami", Type = "Dog", Color = "Gray" });
        _petsSource = new List<Pet>(_petsItems);
    }

    private void FilterPerson()
    {
        List<Pet> petsSearchList = new(_petsSource);

        if (!string.IsNullOrEmpty(_petFilter.Name))
        {
            petsSearchList = petsSearchList.Where(x => x.Name.ToLowerInvariant().Contains(_petFilter.Name.ToLowerInvariant().ToString())).ToList();
        }

        if (!string.IsNullOrEmpty(_petFilter.Type))
        {
            petsSearchList = petsSearchList.Where(x => x.Type.ToLowerInvariant().Contains(_petFilter.Type.ToLowerInvariant().ToString())).ToList();
        }

        _petsItems = petsSearchList;
        StateHasChanged();
    }

    #region parameters
    public string TitleComponent { get; set; } = "Title";

    public void OnUpdateTitleComponent(ChangeEventArgs args)
    {
        TitleComponent = (string)args.Value;
    }
    #endregion
}
