namespace Lagoon.Server.Application.IdentitySources;

/// <summary>
/// Used in the authentication workflow to build application profile (a profile is a group of application role)
/// </summary>
public class GroupChoice
{

    /// <summary>
    /// Group description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Associated roles
    /// </summary>
    public List<string> Roles { get; set; }

    /// <summary>
    /// GroupChoice intialisation
    /// </summary>
    /// <param name="description">Group name</param>
    /// <param name="roles">Associated roles</param>
    public GroupChoice(string description, List<string> roles)
    {
        Description = description;
        Roles = roles;
    }

    /// <summary>
    /// Initalize an empty GroupChoice 
    /// </summary>
    public GroupChoice()
    { }

}
