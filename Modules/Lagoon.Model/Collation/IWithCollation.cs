using Lagoon.Helpers;

namespace Lagoon.Model.Collation;

/// <summary>
/// Interface to access to the collation of the object.
/// </summary>
public interface IWithCollation
{

    /// <summary>
    /// Gets the collation type to use.
    /// </summary>
    public CollationType? CollationType { get; }

}
