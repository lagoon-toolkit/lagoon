using Lagoon.Console.Application;
using Lagoon.Model.Context;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;

namespace Lagoon.Console.Commands;


/// <summary>
/// Apply pending migrations to the database.
/// </summary>
[Command(Name = "updateDB", Description = "Apply pending migrations to the database.")]
public class UpdateDbCommand
{

    /// <summary>
    /// Force the update even if unknown migrations have been applied to the database.
    /// </summary>
    [Option("-f|--force", "Force the update even if unknown migrations have been applied to the database.", CommandOptionType.NoValue)]
    public bool Force { get; set; }

    /// <summary>
    /// Apply pending migrations to the database.
    /// </summary>
    /// <param name="db">Injected Database context.</param>
    public virtual Task OnExecuteAsync(ILgApplicationDbContext db)
    {
        ConsoleEx.WriteLine("Checking applied migrations...");
        List<string> knownMigrations = db.Database.GetMigrations().ToList();
        List<string> appliedMigrations = db.Database.GetAppliedMigrations().ToList();
        List<string> pendingMigrations = knownMigrations.Except(appliedMigrations).ToList();
        bool hasUnknownMigrations = appliedMigrations.Except(knownMigrations).Any();

        if (pendingMigrations.Count == 0)
        {
            ConsoleEx.WriteLine($"Last migration applied : {(knownMigrations.LastOrDefault() ?? "none")}");
            ConsoleEx.WriteSuccess("The database is already up to date !");
            if (hasUnknownMigrations)
            {
                ConsoleEx.WriteWarning("Warning: There are unknown migrations applied to the database, check that it is the latest version of the application...");
            }
        }
        else
        {
            if (hasUnknownMigrations && !Force)
            {
                throw new CriticalException("There are unknown migrations applied to the database:\nCheck that it is the latest version of the application.\nIf you still want to perform the migration, use the --force option.");
            }
            ConsoleEx.Write($"Update the database by applying {pendingMigrations.Count} migration{(pendingMigrations.Count > 1 ? "s" : "")}...");
            try
            {
                db.Database.Migrate();
                ConsoleEx.WriteLine("OK");
                ConsoleEx.WriteSuccess("The database is up to date !");
            }
            catch
            {
                ConsoleEx.WriteError("Failed!");
                throw;
            }
        }
        return Task.CompletedTask;
    }

}
