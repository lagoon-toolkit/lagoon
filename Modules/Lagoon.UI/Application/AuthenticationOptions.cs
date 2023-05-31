namespace Lagoon.UI.Application;

/// <summary>
/// Describe Lagoon authentication options.
/// </summary>
public class AuthenticationOptions
{

    #region properties

    /// <summary>
    /// List of policy names with them corresponding roles.
    /// </summary>
    public IEnumerable<KeyValuePair<string, List<string>>> PolicyDefinitions { get; set; }

    #endregion

}
