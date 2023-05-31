#pragma warning disable IDE0060 // Remove unused parameter

namespace TemplateLagoonWeb.Model.Seeds;

/// <summary>
/// Allows you to add a test data set for the development of the application.
/// </summary>
public static class DataSeeder
{

    #region methods

    /// <summary>
    /// Fill the database for development environnement.
    /// </summary>
    /// <param name="db">The database context of the application.</param>        
    /// <param name="userManager">The user manager.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken)
    {
        // Create the default admin account
        await CreateAdminUserAsync(userManager);
    }

    /// <summary>
    /// Create the default admin account.
    /// </summary>
    /// <param name="userManager"></param>
    private static async Task CreateAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            // Create fake user Account: "admin" / Password: "to_be_renewed"
            ApplicationUser newAdmin = new() {
                Id = Guid.NewGuid(),
                /* #UserNameAsId: Remove the following line if the "UserName" property isn't used to identify a user. */
                UserName = "admin", 
                EmailConfirmed = true, 
                Email = "templatelagoonweb-admin@{domain}", 
                FirstName = "Admin", 
                LastName = "TemplateLagoonWeb"
            };
            await userManager.CreateAsync(newAdmin, "to_be_renewed");
            await userManager.AddToRoleAsync(newAdmin, Roles.Admin);
        }
    }

    #endregion

}
