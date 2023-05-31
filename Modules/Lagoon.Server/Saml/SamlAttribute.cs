namespace Lagoon.Server.Saml;


/// <summary>
/// Assertion attribute.
/// </summary>
public partial class SamlAttribute
{

    #region properties

    /// <summary>
    /// The attribute name.
    /// </summary>
    /// <value>The attribute name.</value>
    /// <returns>The attribute name.</returns>
    public string Name { get; }

    /// <summary>
    /// Sets the unique value for the attribute or gets the first value of the attribute.
    /// </summary>
    /// <value>The first value of the attribute.</value>
    /// <returns>The first value of the attribute.</returns>
    public string Value
    {
        get => Values.Count == 0 ? null : Values[0];
        set
        {
            Values.Clear();
            Values.Add(value);
        }
    }

    /// <summary>
    /// The list of values for the attribute.
    /// </summary>
    /// <value>The list of values for the attribute.</value>
    /// <returns>The list of values for the attribute.</returns>
    public List<string> Values { get; } = new();

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    public SamlAttribute(string name)
    {
        Name = name;
    }

    #endregion

}
