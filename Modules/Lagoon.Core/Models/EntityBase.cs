namespace Lagoon.Shared.Model;

/// <summary>
/// Entity base 
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets profile identifier
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
