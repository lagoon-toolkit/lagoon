using Lagoon.Helpers;

namespace Lagoon.Model.Collation;

/// <summary>
/// Collation managment for PostgreSQL RDBMS
/// </summary>
public class PostgreSQLCollationProvider : CollationProvider
{

    /// <summary>
    /// Constructor
    /// </summary>      
    /// <param name="collationType"></param>
    /// <param name="locale"></param>   
    public PostgreSQLCollationProvider(CollationType collationType, string locale = "und") : base(collationType)
    {
        //Initialize collation name
        Collations[CollationType.IgnoreCase] = $"{locale}-u-ks-level2";
        Collations[CollationType.IgnoreCaseAndAccent] = $"{locale}-u-ks-level1";
    }

}