namespace Lagoon.UI.Internal.BlazorInputFile;


/// <summary>
/// This is public only because it's used in a JSInterop method signature,
/// but otherwise is intended as internal
/// </summary>
public class FileListEntryImpl : IFileListEntry
{
    #region fields

    internal InputFile Owner { get; set; }

    private Stream _stream;

    #endregion

    #region properties

    /// <summary>
    /// Read event
    /// </summary>
    public event EventHandler OnDataRead;

    /// <summary>
    /// Get or set the file identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Get or set the last modification date
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Get or set the file name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Get or set the file size
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Get or set the file content type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Get or set the original file path
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Access to the underlying file stream
    /// </summary>
    public Stream Data
    {
        get
        {
            _stream ??= Owner.OpenFileStream(this);
            return _stream;
        }
    }

    #endregion

    #region public methods

    /// <summary>
    /// Create a new file as an image with the specified dimension.
    /// The file content must be retrieved with the <see cref="Data"/> stream
    /// </summary>
    /// <param name="format">Content type</param>
    /// <param name="maxWidth">Max width</param>
    /// <param name="maxHeight">Max height</param>
    /// <returns>A new file entry</returns>
    public async Task<IFileListEntry> ToImageFileAsync(string format, int maxWidth, int maxHeight)
    {
        return await Owner.ConvertToImageFileAsync(this, format, maxWidth, maxHeight);
    }

    /// <summary>
    /// Create a blob url for the specified file
    /// </summary>
    /// <returns>A local blob url</returns>
    public Task<string> ToBlobUrl()
    {
        return Owner.ToBlobUrlAsync(this);
    }

    #endregion

    #region internal methods

    internal void RaiseOnDataRead()
    {
        OnDataRead?.Invoke(this, null);
    } 

    #endregion
}
