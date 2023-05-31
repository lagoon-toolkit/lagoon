using Lagoon.Helpers;

namespace Lagoon.Model.Collation;

/// <summary>
/// Collation managment for MariaDB RDBMS
/// </summary>
public class OracleCollationProvider : CollationProvider
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="collationType"></param>
    /// <param name="locale"></param>                
    public OracleCollationProvider(CollationType collationType, string locale = "BINARY") : base(collationType)
    {
        //Initialize collation name
        Collations[CollationType.CaseSensitive] = $"{locale}_CS";
        Collations[CollationType.IgnoreCase] = $"{locale}_CI";
        Collations[CollationType.IgnoreCaseAndAccent] = $"{locale}_AI";
    }

}