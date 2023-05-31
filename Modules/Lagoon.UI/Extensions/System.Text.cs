namespace System.Text;

/// <summary>
/// Extensions methods
/// </summary>
public static class LagoonExtensions
{

    #region JSON serialization / deserialization

    /// <summary>
    /// Deserialize JSON "camelCase" names response to "PascalCase" names.
    /// </summary>
    /// <typeparam name="T">The target type of the JSON value.</typeparam>
    /// <param name="json">The JSON text to parse.</param>
    /// <returns>A TValue representation of the JSON value.</returns>
    public static T JsonDeserialize<T>(this string json)
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Deserialize JSON "camelCase" names response to "PascalCase" names.
    /// </summary>
    /// <param name="json">The JSON text to parse.</param>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <returns>A returnType representation of the JSON value.</returns>
    public static object JsonDeserialize(this string json, Type returnType)
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize(json, returnType, options);
    }

    #endregion

}