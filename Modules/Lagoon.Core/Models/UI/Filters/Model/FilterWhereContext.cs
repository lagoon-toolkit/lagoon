namespace Lagoon.Helpers;

/// <summary>
/// The context for the where condition creation (EFCore or IEnumerable in memory).
/// </summary>
public class FilterWhereContext
{

    /// <summary>
    /// Indicates if the query will be executed by EFCore.
    /// </summary>
    public bool TargetEF { get; }

    /// <summary>
    /// Indicate if the DB has a default collation defined.
    /// </summary>
    public bool UseDefaultCollation { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="targetEF">Indicates if the query will be executed by EFCore.</param>
    /// <param name="hasCollation">Indicates if the DB has a default collation defined.</param>
    public FilterWhereContext(bool targetEF, bool hasCollation)
    {
        TargetEF = targetEF;
        UseDefaultCollation = hasCollation;
    }

}
