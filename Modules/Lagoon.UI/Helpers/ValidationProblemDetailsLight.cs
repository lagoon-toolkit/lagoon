namespace Lagoon.Helpers;

[JsonConverter(typeof(ValidationProblemDetailsLightJsonConverter))]
internal class ValidationProblemDetailsLight
{

    public string Title { get; set; }

    public Dictionary<string, List<string>> Errors { get; set; }

}

internal class ValidationProblemDetailsLightJsonConverter : JsonConverter<ValidationProblemDetailsLight>
{
    private static readonly JsonEncodedText _errors = JsonEncodedText.Encode("errors");
    private static readonly JsonEncodedText _title = JsonEncodedText.Encode("title");

    public override ValidationProblemDetailsLight Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ValidationProblemDetailsLight problemDetails = new();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.ValueTextEquals(_errors.EncodedUtf8Bytes))
            {
                problemDetails.Errors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(ref reader, options);
            }
            else
            {
                if (TryReadStringProperty(ref reader, _title, out string propertyValue))
                {
                    problemDetails.Title = propertyValue;
                }
                else
                {
                    reader.Read();
                }
            }
        }
        if (reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException($"Unespected JSon end");
        }
        return problemDetails;
    }

    private static bool TryReadStringProperty(ref Utf8JsonReader reader, JsonEncodedText propertyName, out string value)
    {
        if (!reader.ValueTextEquals(propertyName.EncodedUtf8Bytes))
        {
            value = default;
            return false;
        }
        reader.Read();
        value = reader.GetString()!;
        return true;
    }

    public override void Write(Utf8JsonWriter writer, ValidationProblemDetailsLight value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
