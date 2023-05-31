using Lagoon.Shared.Model;

namespace Lagoon.UI.Components;

/// <summary>
/// Gridview storage profile
/// </summary>
public class GridViewProfile : EntityBase
{

    #region properties

    /// <summary>
    /// Profile label
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets page size
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Is shared profile
    /// </summary>
    [JsonPropertyName("isSharedProfile")]
    public bool IsSharedProfile
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets list of columns profiles
    /// </summary>
    [JsonPropertyName("columns")]
    public List<GridViewColumnProfile> Columns { get; set; } = new();

    /// <summary>
    /// Gets or sets list of groups profiles
    /// </summary>
    [JsonPropertyName("groups")]
    public List<GridViewGroupProfile> Groups { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public GridViewProfile()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    public GridViewProfile(string id, string label, bool isShared)
    {
        Id = id;
        Label = label;
        IsSharedProfile = isShared;
    }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="profile"></param>
    public GridViewProfile(ProfileItem profile)
    {
        Id = profile.Id;
        Label = profile.Label;
        IsSharedProfile = profile.IsSharedProfile;
    }

    #endregion

}
