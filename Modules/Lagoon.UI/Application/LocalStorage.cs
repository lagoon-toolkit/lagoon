using System.Text.RegularExpressions;

namespace Lagoon.UI.Application;

/// <summary>
/// The browser local storage.
/// </summary>
public class LocalStorage
{

    #region private class

    /// <summary>
    /// The new Json.NET doesn't support Timespan at this time
    /// https://github.com/dotnet/corefx/issues/38641
    /// Source : https://github.com/Blazored/LocalStorage/blob/main/src/Blazored.LocalStorage/JsonConverters/TimespanJsonConverter.cs
    /// </summary>
    private class TimespanJsonConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Format: Days.Hours:Minutes:Seconds:Milliseconds
        /// </summary>
        public const string TimeSpanFormatString = @"d\.hh\:mm\:ss\:FFF";

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return TimeSpan.Zero;
            }

            return !TimeSpan.TryParseExact(s, TimeSpanFormatString, null, out TimeSpan parsedTimeSpan)
                ? throw new FormatException($"Input timespan is not in an expected format : expected {Regex.Unescape(TimeSpanFormatString)}. Please retrieve this key as a string and parse manually.")
                : parsedTimeSpan;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            string timespanFormatted = $"{value.ToString(TimeSpanFormatString)}";
            writer.WriteStringValue(timespanFormatted);
        }
    }
    #endregion

    #region fields

    /// <summary>
    /// The JS Runtime instance.
    /// </summary>
    private IJSInProcessRuntime _js;

    /// <summary>
    /// The Json serializer.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    #endregion

    #region constructor

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="js">The JS Runtime instance.</param>
    public LocalStorage(IJSInProcessRuntime js)
    {
        _js = js;
        _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        _jsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
    }

    #endregion

    #region methods

    /// <summary>
    /// Indicate if the storage contains the key.
    /// </summary>
    /// <param name="key">An item key.</param>
    /// <returns><c>true</c> if the value exists in the storage; else return <c>false</c>.</returns>
    public bool ContainKey(string key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? throw new ArgumentNullException(nameof(key))
            : _js.Invoke<bool>("localStorage.hasOwnProperty", key);
    }

    /// <summary>
    /// Gets the key's value if exists; else return <c>null</c>.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    /// <returns>The key's value if exists; else return <c>null</c>.</returns>
    public string GetItemAsString(string key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? throw new ArgumentNullException(nameof(key))
            : _js.Invoke<string>("localStorage.getItem", key);
    }

    /// <summary>
    /// Gets the item corresponding to the key.
    /// </summary>
    /// <typeparam name="T">Type of the item.</typeparam>
    /// <param name="key">The key identifier.</param>
    /// <returns>The item corresponding to the key.</returns>
    public T GetItem<T>(string key)
    {
        string serializedValue = GetItemAsString(key);
        return string.IsNullOrEmpty(serializedValue) ? default : JsonSerializer.Deserialize<T>(serializedValue, _jsonSerializerOptions);
    }

    /// <summary>
    /// Try to get the item corresponding to the key.
    /// </summary>
    /// <typeparam name="T">Type of the item.</typeparam>
    /// <param name="key">The key identifier.</param>
    /// <param name="value">The value found.</param>
    /// <returns>The item corresponding to the key.</returns>
    public bool TryGetItem<T>(string key, out T value)
    {
        string serializedValue = GetItemAsString(key);
        if (serializedValue is null)
        {
            value = default;
            return false;
        }
        else
        {
            value = JsonSerializer.Deserialize<T>(serializedValue, _jsonSerializerOptions);
            return true;
        }
    }

    /// <summary>
    /// Remove the item from the storage if it exists.
    /// </summary>
    /// <param name="key">The key of  the item you want to remove.</param>
    public void RemoveItem(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        _js.InvokeVoid("localStorage.removeItem", key);
    }

    /// <summary>
    /// Add or update the key's value.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    /// <param name="value">The value to save.</param>
    public void SetItemAsString(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value is null)
        {
            _js.InvokeVoid("localStorage.removeItem", key);
        }
        else
        {
            _js.InvokeVoid("localStorage.setItem", key, value);
        }
    }

    /// <summary>
    /// Sets the item corresponding to the key.
    /// </summary>
    /// <typeparam name="T">Type of the item.</typeparam>
    /// <param name="key">The key identifier.</param>
    /// <param name="value">The value to save.</param>
    public void SetItem<T>(string key, T value)
    {
        SetItemAsString(key, JsonSerializer.Serialize<T>(value, _jsonSerializerOptions));
    }

    #endregion

}
