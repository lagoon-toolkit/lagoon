namespace Lagoon.Server.Application;

/// <summary>
/// Information about a file to save in the browser for offline use.
/// </summary>
public class OfflineAsset
{

    /// <summary>
    /// Gets or sets if the file must excluded from the browser cache.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool Exclude { get; set; }


    /// <summary>
    /// The Hash code of the file content.
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    /// <summary>
    /// The URL of the asset.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{Url} ({(Exclude ? "Exclude" : "include")})";
    }

}
