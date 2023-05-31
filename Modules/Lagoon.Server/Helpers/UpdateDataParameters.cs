namespace Lagoon.Helpers;

/// <summary>
/// Class used to deserialize data on the server.
/// </summary>
/// <typeparam name="TItem"></typeparam>
[Bind(nameof(Data), nameof(ColumnKey), nameof(Value), nameof(PreviousValue))]
public class UpdateDataParameters<TItem>
{

    /// <summary>
    /// Gets object of the row
    /// </summary>
    /// <value></value>
    public TItem Data { get; set; }

    /// <summary>
    /// Gets field name
    /// </summary>
    /// <value></value>
    public string ColumnKey { get; set; }

    /// <summary>
    /// Gets new value of the cell
    /// </summary>
    /// <value></value>
    public object Value { get; set; }

    /// <summary>
    /// Gets previous value of the cell
    /// </summary>
    /// <value></value>
    public object PreviousValue { get; set; }

    #region obsolete

    /// <summary>
    /// Gets field name
    /// </summary>
    /// <value></value>
    [Obsolete("Use the \"ColumnKey\" property.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JsonIgnore]
    public string FieldName => ColumnKey;

    #endregion

    #region constructors

    /// <summary>
    /// New instance used by JSon deserialization.
    /// </summary>
    public UpdateDataParameters()
    {
        //NEEDED FOR JSON DESERIALIZATION    
    }

    #endregion

}
