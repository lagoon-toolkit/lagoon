namespace Lagoon.UI.Components;


/// <summary>
/// Gridview columns filters
/// </summary>
[JsonConverter(typeof(FilterJsonConverter))]
public abstract class Filter
{

    /// <summary>
    /// Indicate if the filter is empty.
    /// </summary>
    /// <returns></returns>
    public abstract bool IsEmpty { get; }

    /// <summary>
    /// Gets if the filter is numeric type
    /// </summary>
    public virtual bool IsNumeric { get; }

    /// <summary>
    /// Gets the type of the target filtred property.
    /// </summary>
    /// <returns>The type of the target filtred property.</returns>
    public abstract Type GetValueType();

    /// <summary>
    /// Try to convert a filter type to another.
    /// </summary>
    /// <param name="targetFilterType">The target filter type.</param>
    /// <returns>The new filter or null if it's failed or the source filter has no conditions.</returns>
    public abstract Filter ConvertAs(Type targetFilterType);

    /// <summary>
    /// Get all filter expressions. 
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>All filter expressions.</returns>
    public abstract IEnumerable<LambdaExpression> GetAndExpressions(FilterWhereContext context);

    /// <summary>
    /// Serialize a GridViewFilter from a JSON.
    /// </summary>
    /// <param name="writer">JSON writer/</param>
    /// <param name="options">Serialize options.</param>
    internal abstract void SerializeValues(Utf8JsonWriter writer, JsonSerializerOptions options);

    /// <summary>
    /// Deserialze a GridViewFilter from a JSON.
    /// </summary>
    /// <param name="reader">JSON reader/</param>
    /// <param name="options">Serialize options.</param>
    internal abstract void DeserializeValues(ref Utf8JsonReader reader, JsonSerializerOptions options);        

}
