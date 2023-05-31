namespace Lagoon.UI.Components.Internal;

internal class FilterJsonConverter : JsonConverter<Filter>
{

    #region constants

    /// <summary>
    /// Fake type code for GUID.
    /// </summary>
    private const TypeCode GUID_TYPE_CODE = (TypeCode)100;

    #endregion

    #region methods

    ///<inheritdoc/>
    public override Filter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }
        TypeCode typeCode = TypeCode.Empty;
        Type enumType = null;
        bool isNullable = false;
        Filter filter = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return filter;
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propName = reader.GetString();
                reader.Read();
                switch (propName)
                {
                    case "data":
                        // Compatibility with profile saved with Lagoon version <= 1.10
                        // "data": [{"value": [2],"operator": 10}] OR "data": [{"value": "text","operator": 1}]
                        filter = ReadLegacyFilter(reader, options);
                        //x                            // Ignore the property value
                        //x                            reader.Skip();
                        break;
                    case "null":
                        isNullable = reader.GetBoolean();
                        break;
                    case "enum":
                        string et = reader.GetString();
                        enumType = Type.GetType(et, false, true);
                        if (enumType is not null && !enumType.IsEnum)
                        {
                            enumType = null;
                        }
                        break;
                    case "type":
                        typeCode = (TypeCode)reader.GetInt32();
                        break;
                    case "values":
                        filter = (Filter)Activator.CreateInstance(GetFilterType(enumType, typeCode, isNullable));
                        filter.DeserializeValues(ref reader, options);
                        break;
                }
            }
        }
        throw new JsonException();
    }

    private Filter ReadLegacyFilter(Utf8JsonReader reader, JsonSerializerOptions options)
    {
        List<LegacyFilter> list = JsonSerializer.Deserialize<List<LegacyFilter>>(ref reader, options);
#if DEBUG //TOCLEAN
        Helpers.Trace.ToConsole(this, $"{list[0].Value} / {list[0].Value?.GetType()}");
        Helpers.Trace.ToConsole(this, $"{((JsonElement)list[0].Value).ValueKind}");
#endif
        if (list.Count == 1 && list[0].Value is JsonElement element)
        {
            try
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.False:
                    case JsonValueKind.True:
                        return new BooleanFilter<bool>(element.GetBoolean());
                    case JsonValueKind.String:
                        return new TextFilter(element.GetString());
                    case JsonValueKind.Number:
                        return new NumericFilter<int>(element.GetInt32());
                    case JsonValueKind.Array:
                        return new NumericFilter<int>(element.EnumerateArray().Select(s => s.GetInt32()));
                }
            }
            catch
            {
                Console.WriteLine($"Compatibility deserialization warning for value {element} ({element.ValueKind})");
            }
        }
        return null;
    }

    ///<inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Filter filter, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        Type type = filter.GetValueType();
        Type nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType is not null)
        {
            writer.WriteBoolean("null", true);
        }
        Type rawType = nullableType ?? type;
        TypeCode typeCode = Type.GetTypeCode(rawType);
        if (rawType.IsEnum)
        {                        
            writer.WriteString("enum", $"{rawType.FullName}, {rawType.Assembly.GetName().Name}");
            typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(rawType));
        }
        else if (rawType == typeof(Guid))
        {
            typeCode = GUID_TYPE_CODE;
        }
        writer.WriteNumber("type", (int)typeCode);
        writer.WritePropertyName("values");
        filter.SerializeValues(writer, options);
        writer.WriteEndObject();
    }


    /// <summary>
    /// Get the type corresponding to the TypeCode.
    /// </summary>
    /// <param name="enumType">The enum type..</param>
    /// <param name="typeCode">Code of the type.</param>
    /// <param name="nullable">Indicate if the type must be nullable.</param>
    /// <returns>The filter type</returns>
    private static Type GetFilterType(Type enumType, TypeCode typeCode, bool nullable)
    {
        if (enumType is null)
        {
            Type rawType = typeCode switch
            {
                TypeCode.Boolean => typeof(bool),
                TypeCode.Byte => typeof(byte),
                TypeCode.Char => typeof(char),
                TypeCode.DateTime => typeof(DateTime),
                TypeCode.Decimal => typeof(decimal),
                TypeCode.Double => typeof(double),
                TypeCode.Int16 => typeof(short),
                TypeCode.Int32 => typeof(int),
                TypeCode.Int64 => typeof(long),
                TypeCode.SByte => typeof(sbyte),
                TypeCode.Single => typeof(float),
                TypeCode.String => typeof(string),
                TypeCode.UInt16 => typeof(ushort),
                TypeCode.UInt32 => typeof(uint),
                TypeCode.UInt64 => typeof(ulong),
                GUID_TYPE_CODE => typeof(Guid),
                _ => throw new InvalidOperationException($"The TypeCode \"{typeCode}\" is not supported.")
            };
            if (rawType == typeof(string))
            {
                return typeof(TextFilter);
            }
            Type finalType = nullable && rawType.IsValueType ? typeof(Nullable<>).MakeGenericType(rawType) : rawType;
            if (rawType == typeof(bool))
            {
                return typeof(BooleanFilter<>).MakeGenericType(finalType);
            }
            else if (rawType == typeof(DateTime))
            {
                return typeof(DateFilter<>).MakeGenericType(finalType);
            }
            else if (rawType == typeof(Guid))
            {
                return typeof(SelectFilter<>).MakeGenericType(finalType);
            }
            return typeof(NumericFilter<>).MakeGenericType(finalType);
        }
        else
        {
            if (nullable)
            {
                enumType = typeof(Nullable<>).MakeGenericType(enumType);
            }
            return typeof(EnumFilter<>).MakeGenericType(enumType);
        }
    }

    #endregion

    #region private class

    /// <summary>
    /// Filter parameter
    /// </summary>
    private class LegacyFilter
    {

        /// <summary>
        /// Gets or sets filter value
        /// </summary>
        [JsonPropertyName("value")]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets filter operator
        /// </summary>
        [JsonPropertyName("operator")]
        public int Operator { get; set; }

    }

    #endregion

}
