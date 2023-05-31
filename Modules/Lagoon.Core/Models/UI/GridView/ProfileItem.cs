namespace Lagoon.UI.Components;

/// <summary>
/// Profile item
/// </summary>
public class ProfileItem : IComparable<ProfileItem>
{
    #region properties

    /// <summary>
    /// Gets or sets profile identifier
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Profile label
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// Is shared profile
    /// </summary>
    [JsonPropertyName("isSharedProfile")]
    public bool IsSharedProfile
    {
        get; set;
    }

    #endregion

    #region methods

    /// <summary>
    /// Gets if this profile item is the default profile.
    /// </summary>
    /// <returns><c>true</c> if this profile item is the default profile.</returns>
    public bool IsDefault()
    {
        return Id is not null && Id.EndsWith("-0");
    }

    /// <summary>
    /// Initialise a new profile from the profile item informations.
    /// </summary>
    /// <returns>The new profile.</returns>
    public GridViewProfile GetNewProfile()
    {
        return new GridViewProfile(Id, Label, IsSharedProfile);
    }

    /// <summary>
    /// Compare a profile item to another.
    /// </summary>
    /// <param name="other">The other profile item.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    /// <exception cref="NotImplementedException"></exception>
    int IComparable<ProfileItem>.CompareTo(ProfileItem other)
    {
        int compare = IsSharedProfile.CompareTo(other.IsSharedProfile);
        return compare != 0 ? compare : string.Compare(Label, other.Label, true);
    }

    #endregion

}
