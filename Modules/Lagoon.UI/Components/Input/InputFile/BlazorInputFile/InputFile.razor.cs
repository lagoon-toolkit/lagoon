using Microsoft.JSInterop.WebAssembly;

namespace Lagoon.UI.Internal.BlazorInputFile;


/// <summary>
/// Not intended to be used directly. This component is used by the LgInputFile
/// </summary>
public partial class InputFile : LgComponentBase, IDisposable
{

    ElementReference _inputFileElement;

    IDisposable _thisReference;

    /// <summary>
    /// 
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> UnmatchedParameters { get; set; }

    /// <summary>
    /// Event fired when the selected file list change
    /// </summary>
    [Parameter]
    public EventCallback<IFileListEntry[]> OnChange { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public int MaxMessageSize { get; set; } = 20 * 1024; // TODO: Use SignalR default

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public int MaxBufferSize { get; set; } = 1024 * 1024;

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _thisReference = DotNetObjectReference.Create(this);
            await JS.InvokeAsync<object>("BlazorInputFile.init", _inputFileElement, _thisReference);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [JSInvokable]
    public Task NotifyChange(FileListEntryImpl[] files)
    {
        foreach (var file in files)
        {
            // So that method invocations on the file can be dispatched back here
            file.Owner = (InputFile)(object)this;
        }

        return OnChange.TryInvokeAsync(App, files);
    }

    internal Stream OpenFileStream(FileListEntryImpl file)
    {
        return SharedMemoryFileListEntryStream.IsSupported(JS)
            ? (Stream)new SharedMemoryFileListEntryStream(JS, _inputFileElement, file)
            : new RemoteFileListEntryStream(JS, _inputFileElement, file, MaxMessageSize, MaxBufferSize);
    }

    internal async Task<FileListEntryImpl> ConvertToImageFileAsync(FileListEntryImpl file, string format, int maxWidth, int maxHeight)
    {
        var imageFile = await JS.InvokeAsync<FileListEntryImpl>("BlazorInputFile.toImageFile", _inputFileElement, file.Id, format, maxWidth, maxHeight);

        // So that method invocations on the file can be dispatched back here
        imageFile.Owner = (InputFile)(object)this;

        return imageFile;
    }

    /// <summary>
    /// Create a local blobl url for the specified file
    /// </summary>
    /// <param name="file">File for which we want a blob url</param>
    /// <returns>A local blob url</returns>
    internal async Task<string> ToBlobUrlAsync(FileListEntryImpl file)
    {
        return await JS.InvokeAsync<string>("BlazorInputFile.toBlobUrl", _inputFileElement, file.Id);
    }

    /// <summary>
    /// Upload file from JS without using the HttpClient to be able to show the upload progression
    /// </summary>
    /// <param name="dotnetRef">A dotnef red</param>
    /// <param name="uploadUrl">Upload url</param>
    /// <param name="filesName">Files name to upload</param>
    /// <param name="token">An optional token to add to post header in authorization</param>
    /// <returns></returns>
    internal async Task<bool> DirectJsUploadAsync(IDisposable dotnetRef, string uploadUrl, string[] filesName, string token)
    {
        return await JS.InvokeAsync<int>("BlazorInputFile.uploadWithJs", dotnetRef, _inputFileElement, uploadUrl, filesName, token) == 1;
    }

    /// <summary>
    /// Cancel the upload and clear the selection list
    /// </summary>
    /// <param name="upload"><c>true</c> to cancel an upload in progress</param>
    /// <param name="download"><c>true</c> to cancel all download in progress</param>
    internal void CancelDirectJs(bool upload, bool download)
    {
        JS.InvokeVoid("BlazorInputFile.cancelUpload", _inputFileElement, upload, download);
    }

    /// <summary>
    /// Download a file from JS side
    /// </summary>
    /// <param name="dotnetRef">C# object red</param>
    /// <param name="fileId">File identifier</param>
    /// <param name="filename">File name</param>
    /// <param name="downloadUrl">Download url</param>
    /// <param name="token">Optional authorization header</param>
    /// <returns><c>true</c> if success, <c>false</c> otherwise</returns>
    internal async Task<bool> DirectJsDownloadAsync(IDisposable dotnetRef, string fileId, string filename, string downloadUrl, string token)
    {
        return await JS.InvokeAsync<int>("BlazorInputFile.downloadWithJs", dotnetRef, _inputFileElement, fileId, filename, downloadUrl, token) == 1;
    }

    /// <summary>
    /// Cancel the download
    /// </summary>
    internal void CancelDirectJsDownload(string fileId)
    {
        JS.InvokeVoid("BlazorInputFile.cancelDownload", _inputFileElement, fileId);
    }

    /// <summary>
    /// Remove a file from the selection
    /// </summary>
    internal void RemoveFile(IFileListEntry file)
    {
        JS.InvokeVoid("BlazorInputFile.removeFile", _inputFileElement, ((FileListEntryImpl)file).Id);
    }

    /// <summary>
    /// Clear all selected files
    /// </summary>
    /// <param name="files"></param>
    internal void ClearSelectedFiles(List<IFileListEntry> files)
    {
        foreach (IFileListEntry file in files)
        {
            JS.InvokeVoid("BlazorInputFile.removeFile", _inputFileElement, ((FileListEntryImpl)file).Id);
        }
    }

    /// <summary>
    /// Release the reference
    /// </summary>
    void IDisposable.Dispose()
    {
        _thisReference?.Dispose();
    }

}
