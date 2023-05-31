namespace Lagoon.UI.Components;


/// <summary>
/// Describe a file linked to a LgInputFile
/// </summary>
public class LinkedFile
{

    /// <summary>
    /// An unique identifier for a file
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// File name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// File size
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// File content type
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Can the file be deleted ?
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Can the file be donwloaded ?
    /// </summary>
    public bool CanDownload { get; set; }

}
