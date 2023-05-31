using System.ComponentModel.DataAnnotations;

namespace Lagoon.UI.Demo.Pages.Gridviews;


public class GridViewData
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int? Weight { get; set; }
    public string Mail { get; set; }
    public HairColor HairColor { get; set; }
    public bool? HasGlasses { get; set; }
    public DateTime? BirthDate { get; set; }
    public int PrefixSuffixGrid { get; set; }
    public bool? IsVaccinated { get; set; }
    public string FavoriteColor { get; set; }
}

public enum HairColor
{
    [Display(Name = "#grdBlond")]
    Blond,
    [Display(Name = "#grdBrown")]
    Brown,
    [Display(Name = "#grdGinger")]
    Ginger
}
