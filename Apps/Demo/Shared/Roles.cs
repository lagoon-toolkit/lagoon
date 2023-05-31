namespace Demo.Shared;

/// <summary>
/// Roles of the application. 
/// </summary>
public enum Roles
{

    /// <summary>
    /// Administrator role.
    /// </summary>
    [Display(Name = "#RoleAdmin")]
    Admin,

    /// <summary>
    /// User role.
    /// </summary>
    [Display(Name = "#RoleUser")]
    User


}