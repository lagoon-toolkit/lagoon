namespace Lagoon.UI.Demo.Pages.Gridviews;

[Route(ROUTE)]
public partial class PageGridViewPaging
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "demo/GridViewPaging";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "GridView Paging", IconNames.All.Grid1x2);
    }

    #endregion

    #region properties

    public int[] Pagination { get; } = new int[] { 10, 25, 50 };

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
                new GridViewData() { Id = 5, Name = "Nicole", Surname = "Rojas", PrefixSuffixGrid = 2, Weight = 54, Mail = "NicoleRojas@outlook.com", HairColor = HairColor.Blond, HasGlasses = false, BirthDate = new DateTime(1982, 08, 01), FavoriteColor = "#C00000" },
                new GridViewData() { Id = 6, Name = "Jill", Surname = "Perry", PrefixSuffixGrid = 2, Weight = 101, Mail = "JillPerry@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1990, 05, 14), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 7, Name = "Dalton", Surname = "Burton", PrefixSuffixGrid = 2, Weight = 80, Mail = "DaltonBurton@outlook.com", HairColor = HairColor.Blond, HasGlasses = false, BirthDate = new DateTime(1980, 11, 21), FavoriteColor = "#388DAE" },
                new GridViewData() { Id = 8, Name = "Patty", Surname = "Brown", PrefixSuffixGrid = 2, Weight = 78, Mail = "PattyBrown@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1991, 03, 07), FavoriteColor = "#FFC000"},
                new GridViewData() { Id = 9, Name = "John", Surname = "Trescothik", PrefixSuffixGrid = 2, Weight = 76, Mail = "JohnTrescothik@outlook.com", HairColor = HairColor.Ginger, HasGlasses = false, BirthDate = new DateTime(1991, 12, 22), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 10, Name = "Anne-Marie", Surname = "Paris", PrefixSuffixGrid = 2, Weight = 75, Mail = "AnneMarieParis@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1991, 10, 03), FavoriteColor = "#C00000" },
                new GridViewData() { Id = 11, Name = "Italus", Surname = "Pokorny", PrefixSuffixGrid = 2, Weight = 63, Mail = "ItalusPokorny@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1954, 09, 22), FavoriteColor = "#388DAE" },
                new GridViewData() { Id = 12, Name = "Kristian", Surname = "Chow", PrefixSuffixGrid = 2, Weight = 77, Mail = "KristianChow@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1966, 12, 06), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 13, Name = "Viktor", Surname = "Wild", PrefixSuffixGrid = 2, Weight = 78, Mail = "ViktorWild@outlook.com", HairColor = HairColor.Brown, HasGlasses = true, BirthDate = new DateTime(1977, 03, 30), FavoriteColor = "#FFC000" },
                new GridViewData() { Id = 14, Name = "Diklah", Surname = "Cline", PrefixSuffixGrid = 2, Weight = 94, Mail = "DiklahCline@outlook.com", HairColor = HairColor.Ginger, HasGlasses = true, BirthDate = new DateTime(1998, 05, 05), FavoriteColor = "#388DAE" },
                new GridViewData() { Id = 15, Name = "Emma", Surname = "Batts", PrefixSuffixGrid = 2, Weight = 71, Mail = "EmmaBatts@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1998, 02, 17), FavoriteColor = "#FFC000" },
                new GridViewData() { Id = 16, Name = "Elise", Surname = "Vankov", PrefixSuffixGrid = 2, Weight = 98, Mail = "EliseVankov@outlook.com", HairColor = HairColor.Ginger, HasGlasses = true, BirthDate = new DateTime(1966, 06, 08), FavoriteColor = "#70AD47" },
                new GridViewData() { Id = 17, Name = "Adam", Surname = "Feld", PrefixSuffixGrid = 2, Weight = 65, Mail = "AdamFeld@outlook.com", HairColor = HairColor.Brown, HasGlasses = true, BirthDate = new DateTime(1990, 07, 07), FavoriteColor = "#C00000" },
                new GridViewData() { Id = 18, Name = "Isak", Surname = "Kaluza", PrefixSuffixGrid = 2, Weight = 66, Mail = "IsakKaluza@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1983, 11, 28), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 19, Name = "Henry", Surname = "Miranda", PrefixSuffixGrid = 2, Weight = 78, Mail = "HenryMiranda@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1985, 09, 01), FavoriteColor = "#9E5E9B" },
                new GridViewData() { Id = 20, Name = "Theodor", Surname = "Janz", PrefixSuffixGrid = 2, Weight = 72, Mail = "TheodorJanz@outlook.com", HairColor = HairColor.Brown, HasGlasses = true, BirthDate = new DateTime(1994, 08, 16), FavoriteColor = "#C00000" },
                new GridViewData() { Id = 21, Name = "Sarah", Surname = "Daniels", PrefixSuffixGrid = 2, Weight = 81, Mail = "SarahDaniels@outlook.com", HairColor = HairColor.Blond, HasGlasses = true, BirthDate = new DateTime(1989, 07, 31), FavoriteColor = "#FFC000" },
           };
    }

    #endregion
}