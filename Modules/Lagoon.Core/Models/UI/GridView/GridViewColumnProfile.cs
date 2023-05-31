namespace Lagoon.UI.Components;

/// <summary>
/// GridView column profile
/// </summary>    
public class GridViewColumnProfile
{

    #region properties

    /// <summary>
    /// Gets or sets unique column identifier
    /// </summary>
    [JsonPropertyName("uniqueKey")]
    public string UniqueKey { get; set; }

    /// <summary>
    /// Gets or sets sort direction
    /// </summary>
    [JsonPropertyName("sort")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DataSortDirection? Sort { get; set; }

    /// <summary>
    /// Gets or sets the Json filter value.
    /// </summary>
    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Filter Filter { get; set; }

    /// <summary>
    /// Gets or sets if the column is frozen.
    /// </summary>
    [JsonPropertyName("frozen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Frozen { get; set; }

    /// <summary>
    /// Gets or sets column visibility
    /// </summary>
    [JsonPropertyName("hidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Hidden { get; set; }

    /// <summary>
    /// Gets or sets column order
    /// </summary>
    [JsonPropertyName("order")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Order { get; set; }

    /// <summary>
    /// Gets or sets column width
    /// </summary>
    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Width { get; set; }

    /// <summary>
    /// Gets or sets order of sorting
    /// </summary>
    [JsonPropertyName("sortingOrder")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SortingOrder { get; set; }

    #endregion

}
