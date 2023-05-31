namespace Lagoon.UI.Components;

/// <summary>
/// Loader indicator
/// </summary>
public partial class LgLoaderView : LgComponentBase
{

    #region shared fields

    /// <summary>
    /// Uri for the default indeterminated image
    /// </summary>
    private static string _loaderImgUri;

    #endregion

    #region fields

    /// <summary>
    /// Progress state.
    /// </summary>
    private Progress _progress;

    /// <summary>
    /// Indicate if a progression has been passed as parameter.
    /// </summary>
    private bool _hasProgression;

    /// <summary>
    /// Indicate if the current state.
    /// </summary>
    private bool _wasLoading;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets css class of the container.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets content to display after loading.
    /// </summary>
    [Parameter]
    public RenderFragment<Progress> CustomTemplate { get; set; }

    /// <summary>
    /// Gets or sets content to display after loading.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Get or sets the content to display when the parameter IsError is true.
    /// </summary>
    [Parameter]
    public RenderFragment ErrorContent { get; set; }

    /// <summary>
    /// Get or sets the message to display when the parameter IsError is true.
    /// </summary>
    [Parameter]
    public string ErrorTitle { get; set; }

    /// <summary>
    /// Get or sets the message to display when the parameter IsError is true.
    /// </summary>
    [Parameter]
    public string ErrorDescription { get; set; }

    /// <summary>
    /// Gets or sets if the error content must be displayed.
    /// </summary>
    [Parameter]
    public bool IsError { get; set; }

    /// <summary>
    /// Gets or sets loading indicator.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets description label
    /// </summary>
    [Parameter]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets loading rgaa text.
    /// </summary>
    [Parameter]
    public string RgaaTextLoading { get; set; } = "#RgaaPageLoading";

    /// <summary>
    /// Gets or sets loading finished rgaa text.  
    /// </summary>
    [Parameter]
    public string RgaaTextLoaded { get; set; } = "#RgaaPageLoaded";

    /// <summary>
    /// Get or sets the progress parameter.
    /// </summary>
    [Parameter]
    public Progress Progress { get; set; }

    /// <summary>
    /// Gets or sets the progress type.
    /// </summary>
    [Parameter]
    public ProgressType ProgressType { get; set; } = ProgressType.Circle;

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        UnregisterProgressParameter();
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // Register the Progress parameter
        RegisterProgressParameter();
        // Get the current state
        UpdateLoadingState();
    }

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        // Initialise the app image URL
        _loaderImgUri ??= await JS.InvokeAsync<string>("Lagoon.JsUtils.getAppleTouchIcon");
    }

    private void RegisterProgressParameter()
    {
        if (Progress != _progress)
        {
            UnregisterProgressParameter();
            _progress = Progress;
            _progress.OnProgress += OnProgress;
            _progress.OnEnd += OnEndProgress;
        }
    }

    private void UnregisterProgressParameter()
    {
        if (_progress is not null)
        {
            _progress.OnProgress -= OnProgress;
            _progress.OnEnd -= OnEndProgress;
            _progress = null;
        }
    }

    /// <summary>
    /// Running progression.
    /// </summary>
    /// <param name="progress">Source.</param>
    private void OnProgress(Progress progress)
    {
        _hasProgression = !_progress.IsIndeterminate;
        StateHasChanged();
    }

    /// <summary>
    /// End of the progression.
    /// </summary>
    /// <param name="progress">Source.</param>
    private void OnEndProgress(Progress progress)
    {
        UpdateLoadingState();
        StateHasChanged();
    }

    /// <summary>
    /// Update the loading state.
    /// </summary>
    private void UpdateLoadingState()
    {
        _hasProgression = !(_progress is null || _progress.IsIndeterminate);
        IsLoading = !_progress?.IsEnded ?? IsLoading;
        if (IsLoading != _wasLoading)
        {
            _wasLoading = IsLoading;
            if (_wasLoading)
            {
                ShowScreenReaderInformation(RgaaTextLoading);
            }
            else
            {
                ShowScreenReaderInformation(RgaaTextLoaded);
            }
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("lg-loader");
        builder.AddIf(ProgressType == ProgressType.Bar, "lg-loader-bar", "lg-loader-circle");
        builder.Add(CssClass);
    }

    #endregion
}
