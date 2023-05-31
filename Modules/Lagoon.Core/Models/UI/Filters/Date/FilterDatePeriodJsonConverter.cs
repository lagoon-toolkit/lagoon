namespace Lagoon.UI.Components.Internal;


/// <summary>
/// Keep only the day without hours and timezone to adapt to the local timezone.
/// </summary>
public class FilterDatePeriodJsonConverter : JsonConverter<FilterDatePeriod>
{
    private const string DATE_FORMAT = "yyyy-MM-dd";
    private const string KIND_PROPERTY = "k";
    private const string START_PROPERTY = "d";
    private const string DURATION_PROPERTY = "x";

    ///<inheritdoc/>
    public override FilterDatePeriod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }
        FilterDatePeriodKind kind = FilterDatePeriodKind.Day;
        DateTime? start = null;
        int count = 1;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new FilterDatePeriod(kind, start.Value, count);
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propName = reader.GetString();
                reader.Read();
                switch (propName)
                {
                    case KIND_PROPERTY:
                        kind = (FilterDatePeriodKind)reader.GetInt32();
                        break;
                    case START_PROPERTY:
                        string value = reader.GetString();
                        if (DateTime.TryParseExact(value, DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime date))
                        {
                            start = date;
                        }
                        else
                        {
                            start = DateTime.Parse(value);
                        }
                        break;
                    case DURATION_PROPERTY:
                        count = reader.GetInt32();
                        break;
                }
            }
        }
        throw new JsonException();
    }

    ///<inheritdoc/>
    public override void Write(Utf8JsonWriter writer, FilterDatePeriod period, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if(period.Kind!=FilterDatePeriodKind.Day)
        {
            writer.WriteNumber(KIND_PROPERTY, (int)period.Kind);
        }
        writer.WriteString(START_PROPERTY, period.IncludedStart.ToString(DATE_FORMAT));
        if(period.Duration > 1)
        {
            writer.WriteNumber(DURATION_PROPERTY, period.Duration);
        }
        writer.WriteEndObject();
    }

}