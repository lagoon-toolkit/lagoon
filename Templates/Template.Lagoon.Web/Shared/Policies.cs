using P = TemplateLagoonWeb.Shared.Policies;

namespace TemplateLagoonWeb.Shared;


/// <summary>
/// Policies of the application. 
/// </summary>
public class Policies : Lagoon.Shared.Policies
{

    #region Role required policy names

    /* EXAMPLE : 

    /// <summary>
    /// Allows you to see ...
    /// </summary>
    public const string FooReader = "content.r";

    /// <summary>
    /// Allows you to modify ...
    /// </summary>
    public const string FooEditor = "content.rw";

    */

    /// <summary>
    /// Allows you to add, modify or delete data in the application's parameter tables.
    /// </summary>
    public const string SettingsAdministrator = "misc.adm";

    /// <summary>
    /// Allows you to add, modify or delete users from the application.
    /// </summary>
    public const string UserAdministrator = "user.adm";

    #endregion

    #region Policies registration

    /// <summary>
    /// Policies initialization
    /// </summary>
    /// <param name="options">AuthorizationOption to configure</param>
    /// <returns>Configured option</returns>
    public static void ConfigureAuthorization(AuthorizationOptions options)
    {
        options.AddRequireRolePolicies(GetRequireRolePolicies());
    }

    /// <summary>
    /// Configure roles from policy names.
    /// </summary>
    /// <returns>The policies definition.</returns>
    private static RequireRolePolicies<Roles, P> GetRequireRolePolicies()
    {
        return new()
        {
            // User profile
            /* EXAMPLE : 
            {Roles.User, P.FooReader, P.FooEditor },
            */
            {Roles.User},
            // Administrator profile (Admin Role = User Role + administration policies)
            {Roles.Admin, Roles.User, P.EulaEditor, P.LogReader, P.SettingsAdministrator, P.UserAdministrator}
        };
    }

    #endregion

}