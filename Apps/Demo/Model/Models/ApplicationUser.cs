namespace Demo.Model.Models;

/// <summary>
/// Application user
/// </summary>
public class ApplicationUser : LgIdentityUserGuid
{

    /// <summary>
    /// First name of the user.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

}