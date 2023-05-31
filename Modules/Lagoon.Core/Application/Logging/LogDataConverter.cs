namespace Lagoon.Core.Application.Logging;

/// <summary>
/// The log entry JSON converter.
/// </summary>
public class LogDataConverter<TLogData> : JsonConverter<TLogData>
    where TLogData : LogData, new()
{

    #region constants

    /// <summary>
    /// The format to use to format dates.
    /// </summary>
    public const string DATETIME_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

    #endregion

    #region read methods

    /// <summary>
    /// Create a new TLogData instance from a JSON.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serialisation options.</param>
    /// <returns>The new instance.</returns>
    public override TLogData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }
        TLogData entry = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (entry.Category is null)
                { 
                    entry.IsAppCategory = true; 
                }
                return entry;
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propName = reader.GetString();
                reader.Read();
                switch (propName)
                {
                    case "Time":
                        entry.Time = DateTime.Parse(reader.GetString());
                        break;
                    case "Level":
                        entry.LogLevel = Enum.Parse<LogLevel>(reader.GetString());
                        break;
                    case "Message":
                        entry.Message = ReadMultiLineString(reader);
                        break;
                    case "Stack":
                        entry.StackTrace = ReadMultiLineString(reader);
                        break;
                    case "Category":
                        entry.Category = reader.GetString();
                        break;
                    case "App":
                        entry.IsAppCategory = reader.GetString() != "n";
                        break;
                    case "Side":
                        entry.Side = Enum.Parse<LogSide>(reader.GetString());
                        break;
                    case "Context":
                        entry.Context = ReadMultiLineString(reader);
                        break;
                }
            }
        }
        throw new JsonException();
    }

    /// <summary>
    /// Read a string from a line splited array or from an array.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <returns>The multiline string.</returns>
    private static string ReadMultiLineString(Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // Compatibility with the old format
            return reader.GetString();
        }
        else
        {
            StringBuilder sb = new();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (sb.Length != 0)
                {
                    sb.AppendLine();
                }
                sb.Append(reader.GetString());
            }
            return sb.ToString();
        }

    }

    #endregion

    #region write methods

    /// <summary>
    /// Serialize the log entry.
    /// </summary>
    /// <param name="writer">The JSON Writer.</param>
    /// <param name="entry">The entry to log.</param>
    /// <param name="options">The serialization option.</param>
    public override void Write(Utf8JsonWriter writer, TLogData entry, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Level", entry.LogLevel.ToString());
        writer.WriteString("Time", entry.Time.ToString(DATETIME_FORMAT));
        if (entry.Side == LogSide.Client)
        {
            writer.WriteString("Side", nameof(LogSide.Client));
        }
        if (entry.Category is not null)
        {
            if (entry.IsAppCategory)
            {
                writer.WriteString("App", "y");
            }
            writer.WriteString("Category", entry.Category);
        }
        if (entry.Context is not null)
        {
            writer.WriteString("Context", entry.Context);
        }
        writer.WriteString("Message", entry.Message);
        if (entry.StackTrace is not null)
        {
            writer.WriteString("Stack", entry.StackTrace);
        }
        writer.WriteEndObject();
    }

    #endregion

}
