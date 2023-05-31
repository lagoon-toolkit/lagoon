namespace Lagoon.Model.Models;


/// <summary>
/// The application user base class with a GUID typed Id.
/// </summary>
public class LgIdentityUserGuid : IdentityUser<Guid>, ILgIdentityUser
{

    /// <summary>
    /// Get the Id as string format.
    /// </summary>
    string ILgIdentityUser.Id
    {
        get => Id.ToString();
    }

    /// <summary>
    /// New instance.
    /// </summary>
    public LgIdentityUserGuid()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
    }

}
