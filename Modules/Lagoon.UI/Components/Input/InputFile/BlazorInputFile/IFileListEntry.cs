namespace Lagoon.UI.Components;


/// <summary>
/// Describe a file from an HTMLInputElement
/// </summary>
public interface IFileListEntry
{

    /// <summary>
    /// Get or set the last modification date
    /// </summary>
    DateTime LastModified { get; }

    /// <summary>
    /// Get or set the file name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Get or set the file size
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Get or set the file content type
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Get or set the original file path
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Access to the underlying file stream
    /// </summary>
    Stream Data { get; }

    /// <summary>
    /// Create a new file as image with the specified format and dimension
    /// </summary>
    /// <param name="format">Content Type</param>
    /// <param name="maxWidth">Desired max width</param>
    /// <param name="maxHeight">Desired max height</param>
    /// <returns>A new file entry as image</returns>
    Task<IFileListEntry> ToImageFileAsync(string format, int maxWidth, int maxHeight);

    /// <summary>
    /// Create a local blob url for this file entry
    /// </summary>
    /// <returns></returns>
    Task<string> ToBlobUrl();

    /// <summary>
    /// Event fired when data are readed
    /// </summary>
    event EventHandler OnDataRead;
}
