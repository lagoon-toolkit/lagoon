using Lagoon.Helpers;

namespace Lagoon.Model.Collation;

/// <summary>
/// Collation managment
/// </summary>
public class CollationProvider
{
    #region constants

    /// <summary>
    /// SQL Server provider name
    /// </summary>
    public const string SQLServer = "Microsoft.EntityFrameworkCore.SqlServer";

    /// <summary>
    /// Oracle provider name
    /// </summary>
    public const string Oracle = "Oracle.EntityFrameworkCore";

    /// <summary>
    /// MySQL provider name
    /// </summary>
    public const string MySQL = "Pomelo.EntityFrameworkCore.MySql";

    /// <summary>
    /// MariaDB provider name
    /// </summary>
    public const string MariaDB = "MariaDB";

    /// <summary>
    /// PostgreSQL provider name
    /// </summary>
    public const string PostgreSQL = "Npgsql.EntityFrameworkCore.PostgreSQL";

    #endregion

    #region properties

    /// <summary>
    /// The collaction type.
    /// </summary>
    public CollationType CollationType { get; }

    /// <summary>
    /// Collations list
    /// </summary>
    protected readonly Dictionary<CollationType, string> Collations = new()
    {
        {CollationType.CaseSensitive, null},
        {CollationType.IgnoreCase, null},
        {CollationType.IgnoreCaseAndAccent, null},
    };

    #endregion

    #region constructors

    /// <summary>
    /// Constructor
    /// </summary>        
    /// <param name="collationType"></param>
    public CollationProvider(CollationType collationType)
    {
        CollationType = collationType;
    }

    #endregion

    #region methods

    /// <summary>
    /// Return collation name
    /// </summary>
    /// <returns></returns>
    public string GetCollation()
    {
        return Collations[CollationType];
    }

    #endregion

}