using Demo.Model.Context;

namespace Demo.Model.Seeds;

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
#pragma warning disable IDE0060 // Remove unused parameter
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        // Create the default admin account
        await CreateAdminUserAsync(userManager);

        /* EXAMPLE :
        // Fill the "Foo" DbSet
        if (!db.Foos.Any())
        {
            db.Foos.Add(new() {
                Id = Guid.Parse("68a03652-ce5c-4a6c-995b-f2b0cbee516d"),
                Label = "Foo 1"
            });
            db.Foos.Add(new()
            {
                Id = Guid.Parse("91726b93-a4ee-45a8-b6a5-8b263c9d14e0"),
                Label = "Foo 2"
            });
            db.SaveChanges();
        }
        */
    }

    /// <summary>
    /// Create the default admin account.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    private static async Task CreateAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        // Create the default administrator user account : "admin" / Password: "Test$1234"
        if (!userManager.Users.Any())
        {
            ApplicationUser newAdmin = new() {
                Id = Guid.NewGuid(),
                UserName = "admin", 
                EmailConfirmed = true, 
                Email = "demo-admin@lagoon.fr", 
                FirstName = "Admin", 
                LastName = "Demo"
            };
            await userManager.CreateAsync(newAdmin, "Test$1234");
            await userManager.AddToRoleAsync(newAdmin, Roles.Admin);
            await userManager.AddToRoleAsync(newAdmin, Roles.User);
        }
    }

    #endregion

}
