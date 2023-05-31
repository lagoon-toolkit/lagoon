using Lagoon.Helpers;

namespace Lagoon.Model.Collation;

/// <summary>
/// Collation managment for MariaDB RDBMS
/// </summary>
public class MariaDBCollationProvider : CollationProvider
{
    /// <summary>
    /// Constructor
    /// </summary>      
    /// <param name="collationType"></param>
    /// <param name="locale"></param>    
    public MariaDBCollationProvider(CollationType collationType, string locale = "utf8mb4_uca1400") : base(collationType)
    {
        //Initialize collation name
        Collations[CollationType.CaseSensitive] = $"{locale}_as_cs";
        Collations[CollationType.IgnoreCase] = $"{locale}_as_ci";
        Collations[CollationType.IgnoreCaseAndAccent] = $"{locale}_ai_ci";
    }

}