namespace Lagoon.UI.Demo.Pages.Gridviews;

[Route(ROUTE)]
public partial class PageGridViewCard
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/GridViewCard";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "GridView Style Card", IconNames.All.Grid1x2);
    }

    #endregion

    #region properties

    public List<GridViewData> Data { get; set; }

    #endregion

    #region methods
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTitle(Link());
        DocumentationComponent = "683";


        Data ??= new List<GridViewData>()
            {
                new GridViewData() { Id = 1, Name = "John", Surname = "Valdez", PrefixSuffixGrid = 10, Weight = 78, Mail = "JohnRValdez@gmail.com", HairColor = HairColor.Brown, HasGlasses = true, BirthDate = new DateTime(1969, 06, 13), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 2, Name = "Anne", Surname = "Birney", PrefixSuffixGrid = 20, Weight = 65, Mail = "AnneDBirney@yahoo.com", HairColor = HairColor.Ginger, HasGlasses = false, BirthDate = new DateTime(1976, 12, 08), FavoriteColor = "#388DAE" },
                new GridViewData() { Id = 3, Name = "Jennifer", Surname = "Price", PrefixSuffixGrid = 3, Weight = 60, Mail = "JenniferFPrice@gmail.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1998, 10, 30), FavoriteColor = "#70AD47" },
                new GridViewData() { Id = 4, Name = "William", Surname = "Lopez", PrefixSuffixGrid = 2, Weight = 86, Mail = "WilliamJLopez@outlook.com", HairColor = HairColor.Blond, HasGlasses = false, BirthDate = new DateTime(1982, 08, 01), FavoriteColor = "#FFC000" },
                new GridViewData() { Id = 5, Name = "Nicole", Surname = "Rojas", PrefixSuffixGrid = 2, Weight = 86, Mail = "NicoleRojas@outlook.com", HairColor = HairColor.Blond, HasGlasses = false, BirthDate = new DateTime(1982, 08, 01), FavoriteColor = "#C00000" },
                new GridViewData() { Id = 6, Name = "Jill", Surname = "Perry", PrefixSuffixGrid = 2, Weight = 86, Mail = "JillPerry@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1990, 05, 14), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 7, Name = "Dalton", Surname = "Burton", PrefixSuffixGrid = 2, Weight = 86, Mail = "DaltonBurton@outlook.com", HairColor = HairColor.Blond, HasGlasses = false, BirthDate = new DateTime(1980, 11, 21), FavoriteColor = "#FFC000" }
            };
    }

    #endregion
}