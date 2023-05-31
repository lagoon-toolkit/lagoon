using Lagoon.UI.Internal.BlazorInputFile;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Lagoon.UI.Components;


/// <summary>
/// A file input component.
/// </summary>
public partial class LgInputFile : LgAriaComponentBase
{

    #region private properties

    internal enum State
    {

        /// <summary>
        /// The component is ready to upload files
        /// </summary>
        ReadyToUpload,

        /// <summary>
        /// Upload in progress
        /// </summary>
        Uploading,

        /// <summary>
        /// An error occured while uploading selected files
        /// </summary>
        Error,

    }

    /// <summary>
    /// Gets the accept attribute that will be passed through to the input html element.
    /// </summary>
    protected string AcceptAttribute => string.Join(',', AllowedFileExtensions.Union(AllowedFileTypes));

    /// <summary>
    /// The flag defining if something is currently dragged over the zone.
    /// </summary>
    private bool IsDraggedOver { get; set; }

    /// <summary>
    /// Track component state
    /// </summary>
    private State _state;

    /// <summary>
    /// The current progress.
    /// </summary>
    private int _currentProgress;

    /// <summary>
    /// The kind of progress bar.
    /// </summary>
    private readonly Kind _progressKind = Kind.Primary;

    /// <summary>
    /// The InputFile component used by this component
    /// </summary>
    private InputFile _baseInputFile;

    /// <summary>
    /// This component reference used by JS to incoke C#
    /// </summary>
    private IDisposable _dotnetRef;

    /// <summary>
    /// The collection of selected files.
    /// </summary>
    private readonly List<IFileListEntry> _selectedFiles = new();

    /// <summary>
    /// List of file which can be downloaded / deleted
    /// </summary>
    private IEnumerable<LinkedFile> _linkedFiles;

    /// <summary>
    /// The progression label used when upload in progress
    /// </summary>
    private string _progressionLabel = "";

    /// <summary>
    /// To track file download progression
    /// </summary>
    private readonly Dictionary<string, int> _downloadInProgress = new();

    ///// <summary>
    ///// Gets the instruction message.
    ///// </summary>
    //private string InstructionMessage => IsDraggedOver ? DropMessage.CheckTranslate() : DragAndDropMessage.CheckTranslate(AllowedFileExtensions.Count > 0 ? string.Join(",", AllowedFileExtensions) : "lgFileUploadFile".Translate());

    #endregion

    #region protected properties

    /// <summary>
    /// Gets the DOM element identifier.
    /// </summary>
    protected string ElementId { get; } = GetNewElementId();

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the upload URL endpoint.
    /// </summary>
    [Parameter]
    public string UploadUri { get; set; }

    /// <summary>
    /// Gets or sets the linkef files URL.
    /// </summary>
    [Parameter]
    public string LinkedFilesUri { get; set; }

    /// <summary>
    /// Get or set the text used before the file list (default is lgFileUploadLinkedFileList dictionnary key)
    /// </summary>
    [Parameter]
    public string FileListLabel { get; set; } = "#lgFileUploadLinkedFileList";

    /// <summary>
    /// Gets or sets the flag allowing multiple files upload.
    /// </summary>
    [Parameter]
    public bool AllowMultipleFiles { get; set; }

    /// <summary>
    /// Gets or sets the flag allowing the directories' selection.
    /// </summary>
    [Parameter]
    public bool AllowDirectories { get; set; }

    /// <summary>
    /// Gets or sets a list of allowed file extensions.
    /// </summary>
    [Parameter]
    public List<string> AllowedFileExtensions { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a list of allowed file types.
    /// </summary>
    [Parameter]
    public List<string> AllowedFileTypes { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the flag used to display the file list.
    /// </summary>
    [Parameter]
    public bool ShowFileList { get; set; } = true;

    /// <summary>
    /// Gets or sets the file card template used to display selected files to upload.
    /// </summary>
    [Parameter]
    public RenderFragment<IFileListEntry> FileCardTemplate { get; set; }

    /// <summary>
    /// Gets or sets the maximum total size in bytes for a each upload.
    /// </summary>
    [Parameter]
    public long MaxUploadSize { get; set; } = -1;

    /// <summary>
    /// Gets or sets the action message.
    /// </summary>
    [Parameter]    
    public string ActionMessage { get; set; } = "#lgFileUploadDragAndDropMessage";

    /// <summary>
    /// Gets or sets the drag and drop message.
    /// </summary>
    [Parameter]
    [Obsolete("Use 'ActionMessage' property")]
    public string DragAndDropMessage { get; set; } = "#lgFileUploadDragAndDropMessage";

    /// <summary>
    /// Gets or sets the drop message.
    /// </summary>
    [Parameter]
    [Obsolete("Use 'ActionMessage' property")]
    public string DropMessage { get; set; } = "#lgFileUploadDropMessage";

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    [Parameter]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the render fragment to customize the label.
    /// </summary>
    [Parameter]
    public RenderFragment LabelContent { get; set; }

    /// <summary>
    /// Gets or sets the uploading message.
    /// </summary>
    [Parameter]
    public string UploadingMessage { get; set; } = "lgFileUploadUploadingMessage".Translate();

    /// <summary>
    /// Gets or sets the uploaded message for success.
    /// </summary>
    [Parameter]
    [Obsolete("This parameter is no more used.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string UploadedSuccessMessage { get; set; }

    /// <summary>
    /// Gets or sets the uploaded message for failure.
    /// </summary>
    [Parameter]
    public string UploadedFailureMessage { get; set; } = "lgFileUploadUploadedFailureMessage".Translate();

    /// <summary>
    /// Gets or sets the onchange event callback.
    /// </summary>
    [Parameter]
    public EventCallback<IFileListEntry[]> OnChange { get; set; }

    /// <summary>
    /// Event fired when file are successfully uploaded to the server
    /// </summary>
    [Parameter]
    public EventCallback OnUploadComplete { get; set; }

    /// <summary>
    /// Event fired when a file is successfully deleted from the server
    /// </summary>
    [Parameter]
    public EventCallback<LinkedFile> OnDeleteComplete { get; set; }

    /// <summary>
    /// Event fired when an error occurs when file are uploaded
    /// </summary>
    [Parameter]
    public EventCallback<Exception> OnErrorHandling { get; set; }

    /// <summary>
    /// Upload behavior (default is Automatic). <see cref="UploadMode" /> enum.
    /// </summary>
    [Parameter]
    public UploadMode UploadMode { get; set; } = UploadMode.Automatic;

    /// <summary>
    /// If set to true (the default) then the POST request will integrate the authorization header with a bearer token. 
    /// If false no authorization header will be sent.
    /// </summary>
    /// <remarks>
    /// If you don't handle the <see cref="OnTokenRequested"/>, the token will be retrived with an <see cref="IAccessTokenProvider"/>
    /// </remarks>
    [Parameter]
    public bool Authenticate { get; set; } = true;

    /// <summary>
    /// Get or set a callback to provide a token to used in the http request authorization header
    /// </summary>
    /// <remarks>
    /// Only called if <see cref="Authenticate"/> is set to <c>true</c>
    /// </remarks>
    [Parameter]
    public Func<Task<string>> OnTokenRequested { get; set; }

    /// <summary>
    /// Get or set the flag used to display upload speed progression
    /// </summary>
    [Parameter]
    public bool ShowUploadSpeed { get; set; } = false;

    /// <summary>
    /// Get or set the flag used to active / desactive the upload functionnality
    /// </summary>
    [Parameter]
    public bool ShowUpload { get; set; } = true;

    /// <summary>
    /// Get or set the flag which define de layout mode (<c>true</c> by default)
    /// </summary>
    [Parameter]
    public bool VerticalDisplay { get; set; } = true;

    /// <summary>
    /// Get or the the flag wich define if user can select two files with a same file name
    /// </summary>
    [Parameter]
    public bool AllowDuplicate { get; set; } = true;

    /// <summary>
    /// Get or set the upload icon to display
    /// </summary>
    [Parameter]
    public string UploadIconName { get; set; } = @IconNames.UploadBoxIcon;

    #endregion

    #region Initialisation

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _dotnetRef = DotNetObjectReference.Create(this);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await DownloadFileListAsync();
            await base.OnInitializedAsync();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #endregion

    #region JSInvokable

    /// <summary>
    /// Upload progression callback
    /// </summary>
    /// <param name="progression">Progression in %</param>
    /// <param name="bytesPerSecond">Progression speed</param>
    /// <returns>Task</returns>
    [JSInvokable]
    public Task UploadProgressionAsync(int progression, int bytesPerSecond)
    {
        if (progression != _currentProgress)
        {
            _currentProgress = progression;
            _progressionLabel = $"{_currentProgress}% {(ShowUploadSpeed && bytesPerSecond > 0 ? $"({Tools.BytesToHumanReadable(bytesPerSecond, "{0:0.##}{1}")})" : "")}";
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Upload progression callback
    /// </summary>
    /// <param name="fileId">File identifier</param>
    /// <param name="progression">Download progression (%)</param>
    /// <returns>Task</returns>
    [JSInvokable]
    public Task DownloadProgressionAsync(string fileId, int progression)
    {
        if (progression != _currentProgress)
        {
            if (_downloadInProgress.ContainsKey(fileId))
            {
                _downloadInProgress[fileId] = progression;
                StateHasChanged();
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Upload complete callback
    /// </summary>
    /// <returns>Task</returns>
    [JSInvokable]
    public async Task UploadCompleteAsync()
    {
        _baseInputFile.ClearSelectedFiles(_selectedFiles);
        _selectedFiles.Clear();
        _state = State.ReadyToUpload;
        StateHasChanged();
        if (OnUploadComplete.HasDelegate)
        {
            await OnUploadComplete.TryInvokeAsync(App);
        }
    }

    /// <summary>
    /// Upload/Download failed callback
    /// </summary>
    /// <param name="statusCode">HttpStatusCode</param>
    /// <param name="errorMessage">Raw server response</param>
    /// <param name="download">false if upload, true if download</param>
    /// <returns>Task</returns>
    [JSInvokable]
    public async Task UploadFailedAsync(int statusCode, string errorMessage, bool download = false)
    {
        try
        {
            if (!download)
            {
                // Apply only for upload
                _state = State.Error;
            }
            System.Net.Http.LagoonExtensions.ThrowParsedError((System.Net.HttpStatusCode)statusCode, errorMessage);
        }
        catch (Exception ex)
        {
            if (OnErrorHandling.HasDelegate)
            {
                await OnErrorHandling.TryInvokeAsync(App, ex);
            }
            else
            {
                ShowException(ex);
            }
        }
        finally
        {
            StateHasChanged();
        }
    }

    #endregion

    #region public methods

    /// <summary>
    /// Start to upload selected files
    /// </summary>
    /// <returns><c>true</c> if files successfully uploaded, <c>false</c> if no file selected or upload cancelled or upload failed</returns>
    public async Task<bool> DoUploadAsync()
    {
        if (!CheckUploadSize())
        {
            throw new UserException($"Max size excedeed. Limit:{MaxUploadSize} Current:{GetTotalSizeToUpload()}");
        }
        if (_state == State.Uploading)
        {
            throw new Exception("Upload already in progress");
        }
        if (string.IsNullOrEmpty(UploadUri))
        {
            throw new Exception($"The {nameof(UploadUri)} is not specified. Please set your controller url");
        }
        if (_selectedFiles.Count > 0)
        {
            // Update ui state
            _state = State.Uploading;
            // Retrieve a valid token if necessary
            string token = await GetTokenAsync();
            // Start uploading selected files
            if (await _baseInputFile.DirectJsUploadAsync(_dotnetRef, UploadUri, _selectedFiles.Select(x => x.Name).ToArray(), token))
            {
                await DownloadFileListAsync();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Cancels an upload and reset files selection
    /// </summary>
    public Task CancelUploadAsync()
    {
        _baseInputFile.CancelDirectJs(true, false);
        _selectedFiles.Clear();
        _state = State.ReadyToUpload;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Load the list of file attached
    /// </summary>
    /// <returns>Task</returns>
    public async Task RefreshFilesListAsync()
    {
        await DownloadFileListAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Compute the size of the POST
    /// </summary>
    /// <returns>Sum of selected file size</returns>
    public long GetTotalSizeToUpload()
    {
        return _selectedFiles.Select(f => f.Size).Sum();
    }

    /// <summary>
    /// Return true if the size of all selected file is under the <see cref="MaxUploadSize"/>
    /// </summary>
    /// <returns>true if under <see cref="MaxUploadSize"/>, false otherwise</returns>
    public bool CheckUploadSize()
    {
        return MaxUploadSize < 0 || GetTotalSizeToUpload() <= MaxUploadSize;
    }

    #endregion

    #region private methods

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("lg-input-file", CssClass);
    }

    /// <summary>
    /// Handles the selection of file.
    /// </summary>
    /// <param name="files">The collection of files.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    private async Task HandleSelectionAsync(IFileListEntry[] files)
    {
        List<IFileListEntry> duplicates = new();
        if (!AllowMultipleFiles)
        {
            _selectedFiles.Clear();
        }
        if (!AllowDuplicate)
        {
            // Extract the list of duplicate file name
            duplicates = files.Where(x => _selectedFiles.Select(x => x.Name).Contains(x.Name)).ToList();
        }
        List<string> notAllowed = new();
        foreach (IFileListEntry file in files)
        {
            if (AllowedFileExtensions.Count == 0 || (AllowedFileExtensions.Count > 0 && AllowedFileExtensions.Contains(Path.GetExtension(file.Name))))
            {
                // don't add duplicate file name
                if (AllowDuplicate || !duplicates.Select(x => x.Name).Contains(file.Name))
                {
                    _selectedFiles.Add(file);
                }                
            }
            else
            {
                notAllowed.Add(file.Name);
            }
        }
        if (notAllowed.Count > 0)
        {
            string acceptedFormat = (AllowedFileExtensions.Count == 1 ? "lblAcceptFormat" : "lblAcceptFormats").Translate(string.Join(", ", AllowedFileExtensions));
            string msg = (notAllowed.Count > 1 ? "lgFilesNotAllowed" : "lgFileNotAllowed").Translate(string.Join(", ", notAllowed), acceptedFormat);
            ShowWarning(msg);
        }
        if (duplicates.Count > 0)
        {
            ShowWarning("lgFileDuplicated".Translate(string.Join(", ", duplicates.Select(x => x.Name))));
        }
        if (OnChange.HasDelegate)
        {
            await OnChange.TryInvokeAsync(App, _selectedFiles.ToArray());
        }
        if (notAllowed.Count == 0 && UploadMode == UploadMode.Automatic && CheckUploadSize())
        {
            await DoUploadAsync();
        }
    }

    /// <summary>
    /// Removes the specified file.
    /// </summary>
    /// <param name="file">The file to remove.</param>
    private async Task RemoveFileAsync(IFileListEntry file)
    {
        _selectedFiles.Remove(file);
        _baseInputFile.RemoveFile(file);
        if (OnChange.HasDelegate)
        {
            await OnChange.TryInvokeAsync(App, _selectedFiles.ToArray());
        }
    }

    /// <summary>
    /// Download the linked file list if a <see cref="UploadUri"/> is provided
    /// </summary>
    /// <returns>Task</returns>
    private async Task DownloadFileListAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(LinkedFilesUri))
            {
                using (var wc = GetNewWaitingContext())
                {
                    HttpClient http = Authenticate ? Http : AnonymousHttpClient;
                    _linkedFiles = await http.TryGetAsync<IEnumerable<LinkedFile>>(LinkedFilesUri, wc?.CancellationToken ?? CancellationToken.None);
                }
            }
        }
        catch (LgHttpFetchException)
        {
            Console.WriteLine($"DownloadFileListAsync ========= LgHttpFetchException");
        }
    }

    /// <summary>
    /// Send a delete request to the LinkedFilesUrl 
    /// </summary>
    /// <param name="file">File to delete</param>
    private Task SendDeleteFileRequestAsync(LinkedFile file)
    {
        try
        {
            // Send the delete request & reload file list
            async Task onConfirm() {
                // Send the delete request
                using (var wc = GetNewWaitingContext())
                {
                    await Http.TryDeleteAsync($"{LinkedFilesUri}/{file.Id}", wc?.CancellationToken ?? CancellationToken.None);
                }
                // Retrieve file list
                await DownloadFileListAsync();
                if (OnDeleteComplete.HasDelegate)
                {
                    await OnDeleteComplete.TryInvokeAsync(App, file);
                }
                StateHasChanged();
            }
            // Ask confirmation before send the delete request
            ShowConfirm("lgFileDeleteConfirm".Translate(), onConfirm);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Start to download a dile
    /// </summary>
    /// <param name="file">File to download</param>
    private async Task SendDownloadFileRequestAsync(LinkedFile file)
    {
        try
        {
            // Track download file
            _downloadInProgress.Add(file.Id.ToString(), 0);
            // Launch the download
            bool state = await _baseInputFile.DirectJsDownloadAsync(_dotnetRef, file.Id.ToString(), file.Name, $"{LinkedFilesUri}/{file.Id}", await GetTokenAsync());
            // Remove the file from the download list
            _downloadInProgress.Remove(file.Id.ToString());
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Cancel the download
    /// </summary>
    /// <param name="file">File for which we want to abort the download</param>
    private Task CancelDownload(LinkedFile file)
    {
        _baseInputFile.CancelDirectJsDownload(file.Id.ToString());
        ShowWarning("lgFileUploadDownloadCancelled".Translate(file.Name));
        return Task.CompletedTask;
    }

    /// <summary>
    /// If <see cref="Authenticate"/> is set to true a token must be sent with all http request
    /// </summary>
    /// <returns>A token or null if no Authentication</returns>
    private Task<string> GetTokenAsync()
    {
        if (Authenticate)
        {
            if (OnTokenRequested != null)
            {
                return OnTokenRequested();
            }
            return App.GetAccessTokenValueAsync();
        }
        return Task.FromResult<string>(null);
    }

    private bool HasContentAfter()
    {
        return ShowUpload && ((ShowFileList && _selectedFiles.Any()) || !string.IsNullOrEmpty(LinkedFilesUri));
    }

    #endregion

    #region Free resources

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _baseInputFile.CancelDirectJs(true, true);
            _dotnetRef?.Dispose();
        }
        base.Dispose(disposing);
    } 

    #endregion

}