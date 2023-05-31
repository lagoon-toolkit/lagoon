namespace Lagoon.Model.Models;


/// <summary>
/// Lagoon setting table
/// </summary>
/// <typeparam name="TUser">For the User foreign key and union with entity server.</typeparam>
public class LagoonSettings<TUser> : LagoonSettingsBase
{

    /// <summary>
    /// The user identifier.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// The user attached to the setting.
    /// </summary>
    public virtual TUser User { get; set; }

}
