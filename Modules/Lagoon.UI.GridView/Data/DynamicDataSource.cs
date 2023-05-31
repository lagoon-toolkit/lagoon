using Lagoon.Core.Helpers;

namespace Lagoon.UI.Components;

/// <summary>
/// Defines a dynamic data source.
/// </summary>
public class DynamicDataSource : List<Dictionary<string, ObjectWrapper>>
{
    #region properties

    /// <summary>
    /// Gets or sets the definition of the dynamic properties.
    /// </summary>
    public IReadOnlyDictionary<string, Type> Properties { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicDataSource"/> class.
    /// </summary>
    /// <param name="properties">The collection of properties.</param>
    public DynamicDataSource(Dictionary<string, Type> properties)
    {
        Properties = properties;
    }

    #endregion

    #region methods

    /// <summary>
    /// Adds a dictionary to the list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(IDictionary<string, object> item)
    {
        Add(item.ToDictionary(d => d.Key, d => new ObjectWrapper(d.Value)));
    }

    #endregion
}