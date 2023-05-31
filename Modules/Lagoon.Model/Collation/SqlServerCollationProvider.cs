using Lagoon.Helpers;

namespace Lagoon.Model.Collation;

/// <summary>
/// Collation managment for SQLServer RDBMS
/// </summary>
public class SqlServerCollationProvider : CollationProvider
{
    /// <summary>
    /// Constructor
    /// </summary>    
    /// <param name="collationType"></param>
    /// <param name="locale"></param>   
    public SqlServerCollationProvider(CollationType collationType, string locale) : base(collationType)
    {
        //Initialize collation name
        Collations[CollationType.CaseSensitive] = $"{locale}_CS_AS";
        Collations[CollationType.IgnoreCase] = $"{locale}_CI_AS";
        Collations[CollationType.IgnoreCaseAndAccent] = $"{locale}_CI_AI";
    }

}