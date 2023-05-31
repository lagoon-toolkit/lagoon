namespace Lagoon.Model.Models;


/// <summary>
/// The application user base class with a string typed Id.
/// </summary>
public class LgIdentityUser : IdentityUser<string>, ILgIdentityUser
{
    /// <summary>
    /// New instance.
    /// </summary>
    public LgIdentityUser()
    {
        Id = Guid.NewGuid().ToString();
        SecurityStamp = Guid.NewGuid().ToString();
    }

}