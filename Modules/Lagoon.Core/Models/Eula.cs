namespace Lagoon.Shared.Model;


/// <summary>
/// Eula model used to store eula data into DB
/// </summary>
public class Eula
{

    /// <summary>
    /// Primary key
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Eula value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Version key (used in DB)
    /// </summary>
    public const string VersionKey = "EulaVersion";

    /// <summary>
    /// Eula content key (vf. dico.xml)
    /// </summary>
    public const string ContentKey = "EulaContent";


}
