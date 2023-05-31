using Lagoon.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Lagoon.Model.Collation;

internal static class PostgreSQLExtension
{

    /// <summary>
    /// Add Lagoon PostgreSQL extension
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <param name="collationType"></param>
    /// <returns></returns>
    public static DbContextOptionsBuilder ConfigurePosgreSQLCollation(this DbContextOptionsBuilder optionsBuilder, CollationType collationType)
    {
        if (collationType != CollationType.CaseSensitive)
        {
            PostgreSQLDbContextOptionsExtension extension =
                optionsBuilder.Options.FindExtension<PostgreSQLDbContextOptionsExtension>()
                   ?? new PostgreSQLDbContextOptionsExtension(collationType);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
        }
        return optionsBuilder;
    }

}