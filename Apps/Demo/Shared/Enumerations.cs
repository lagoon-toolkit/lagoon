namespace Demo.Shared;

/// <summary>
/// Gender.
/// </summary>
public enum Gender
{

    /// <summary>
    /// Unknown sex.
    /// </summary>
    [Display(Name = "#SexUnknown")]
    Unknown,

    /// <summary>
    /// Female.
    /// </summary>
    [Display(Name = "#SexFemale")]
    Female,

    /// <summary>
    /// Male.
    /// </summary>
    [Display(Name = "#SexMale")]
    Male

}

/// <summary>
/// Hair color.
/// </summary>
public enum HairColor
{
    /// <summary>
    /// Blond.
    /// </summary>
    [Display(Name = "#HairColorBlond")]
    Blond,
    
    /// <summary>
    /// Brown.
    /// </summary>
    [Display(Name = "#HairColorBrown")]
    Brown,
    
    /// <summary>
    /// Ginger.
    /// </summary>
    [Display(Name = "#HairColorGinger")]
    Ginger
}