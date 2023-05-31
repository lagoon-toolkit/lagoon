using P = Demo.Shared.Policies;

namespace Demo.Shared;

/// <summary>
/// Policies of the application. 
/// </summary>
public class Policies : Lagoon.Shared.Policies
{

    #region Role required policy names

    /// <summary>
    /// Allows you to see ...
    /// </summary>
    public const string ContentReader = "content.r";

    /// <summary>
    /// Allows you to modify ...
    /// </summary>
    public const string ContentEditor = "content.rw";

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
            {Roles.User, P.ContentReader},
            // Administrator profile (Admin Role = User Role + administration policies)
            {Roles.Admin, Roles.User, P.ContentEditor, P.EulaEditor, P.LogReader, P.SettingsAdministrator, P.UserAdministrator}
        };
    }

    #endregion

}