namespace Lagoon.UI.Components;

/// <summary>
/// Container to show in modal window.
/// </summary>
public partial class LgModal : LgAriaComponentBase, IPageTitleHandler, IFormTrackerHandler
{
    #region fields

    /// <summary>
    /// Header unique Id for rgaa
    /// </summary>
    /// <returns></returns>
    private readonly string _headerId = GetNewElementId();

    /// <summary>
    /// Modal unique Id
    /// </summary>
    /// <returns></returns>
    private readonly string _id = GetNewElementId();

    /// <summary>
    /// Indicate if the modal has been showed
    /// </summary>
    private bool _hasBeenVisible;

    /// <summary>
    /// Title defined in the LgPage.
    /// </summary>
    private string _pageTitle;

    /// <summary>
    /// Iconname defined in the LgPage.
    /// </summary>
    private string _pageIconName;

    /// <summary>
    /// CSS classes associated to the LgPage.
    /// </summary>
    private string _pageCssClass;

    /// <summary>
    /// List des form trackers
    /// </summary>
    private readonly List<LgFormTracker> _formTrackerList = new();

    /// <summary>
    /// Reference to modal
    /// </summary>
    private DotNetObjectReference<LgModal> _dotNetObjectReference;

    /// <summary>
    /// Reference to the DOM modal root
    /// </summary>
    private ElementReference _modalRef;

    /// <summary>
    /// Flag to indicate if the view should be render
    /// </summary>
    private bool _shouldRefresh = false;

    /// <summary>
    /// State of the icon used to display the drag mode
    /// </summary>
    private bool _allowDragLeave = false;

    /// <summary>
    /// Icon used to display the drag mode
    /// </summary>
    private string _iconDrag = IconNames.All.MoveRestricted;

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets modal body content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets modal footer content
    /// </summary>
    [Parameter]
    public RenderFragment FooterContent { get; set; }

    /// <summary>
    /// Get or set the toolbar content
    /// </summary>
    [Parameter]
    public RenderFragment Toolbar { get; set; }

    #endregion

    #region cascading parameters

    /// <summary>
    /// Gets or sets form tracker parent component 
    /// </summary>
    [CascadingParameter]
    public LgFormTracker FormTracker { get; set; }

    /// <summary>
    /// Parent "FormTrackerHandler".
    /// </summary>
    [CascadingParameter]
    public IFormTrackerHandler ParentFormTrackerHandler { get; set; }

    #endregion

    #region properties

    List<LgFormTracker> IFormTrackerHandler.FormTrackerList => _formTrackerList;

    IFormTrackerHandler IFormTrackerHandler.ParentFormTracker => ParentFormTrackerHandler;

    /// <summary>
    /// ID in the DOM.
    /// </summary>
    internal string Id => _id;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or set modal closable
    /// </summary>
    [Parameter]
    public bool Closable { get; set; } = true;

    /// <summary>
    /// Gets or sets modal draggable
    /// </summary>
    [Parameter]
    public bool Draggable { get; set; } = true;

    /// <summary>
    /// Gets or sets modal centered
    /// </summary>
    [Parameter]
    public bool Centered { get; set; }

    /// <summary>
    /// Gets or sets the modal cssclass
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the modal title
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the modal header icon
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets modal description content
    /// </summary>
    [Parameter]
    public string Summary { get; set; }

    /// <summary>
    /// Gets or sets the modal size
    /// </summary>
    [Parameter]
    public ModalSize ModalSize { get; set; } = ModalSize.Medium;

    /// <summary>
    /// Gets or sets if the modal is displayed by default.
    /// </summary>
    [Parameter]
    public bool? DefaultVisible { get; set; }

    /// <summary>
    /// Gets or sets modal display
    /// </summary>
    [Parameter]
    public bool Visible { get; set; }

    /// <summary>
    /// Event raised when modal show value.
    /// </summary>
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    #endregion

    #region events

    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    [Parameter]
    public EventCallback<CloseModalEventArgs> OnClose { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        AriaLabelledBy = _headerId;
        if (DefaultVisible.HasValue)
        { Visible = DefaultVisible.Value; };
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Visible)
        {
            _hasBeenVisible = true;
        }
    }

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!Visible && _hasBeenVisible)
        {
            _hasBeenVisible = false;
            DisposeJs();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Initialize modal JS
        if (Visible)
        {
            await JS.InvokeVoidAsync("Lagoon.LgModal.Init", _modalRef, _dotNetObjectReference, Draggable);
        }
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        DisposeJs();
        _dotNetObjectReference?.Dispose();
        base.Dispose(disposing);
    }

    internal void DisposeJs()
    {
        JS.InvokeVoid("Lagoon.LgModal.Dispose", Id);
    }

    /// <summary>
    /// Modal body content render
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment RenderBodyContent()
    {
        return ChildContent;
    }

    /// <summary>
    /// Modal child content render
    /// </summary>
    /// <returns></returns>
    protected virtual RenderFragment RenderFooterContent()
    {
        return FooterContent;
    }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("modal fade show d-block", CssClass, _pageCssClass);
        // Add an indicator on the title if a FormTracker has pending changes
        builder.AddIf(((IFormTrackerHandler)this).HasModification(), "field-change");
    }

    /// <summary>
    /// Get the CSS class attribute for dialog part.
    /// </summary>
    /// <returns></returns>
    protected string GetDialogClassAttribute()
    {
        string modalSizeClass = ModalSize switch
        {
            ModalSize.Small => "modal-sm",
            ModalSize.Large => "modal-lg",
            ModalSize.ExtraLarge => "modal-xl",
            ModalSize.ExtraExtraLarge => "modal-xxl",
            _ => "",
        };
        LgCssClassBuilder builder = new();
        builder.Add("modal-dialog", modalSizeClass);
        builder.AddIf(Centered, "modal-dialog-centered");
        return builder.ToString();
    }

    /// <summary>
    /// invoked when display modal
    /// </summary>
    /// <returns></returns>
    public Task ShowAsync()
    {
        Visible = true;
        return VisibleChanged.TryInvokeAsync(App, true);
    }

    /// <summary>
    /// Ask to close the modal.
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        if (((IFormTrackerHandler)this).HasModification())
        {
            _shouldRefresh = true;
            ShowConfirm("LostChangesConfirmation".Translate(), ForceCloseAsync);
        }
        else
        {
            await ForceCloseAsync();
        }
    }

    /// <summary>
    /// Switch the drag mode indicator
    /// </summary>
    /// <returns></returns>
    private async Task SwitchDragAsync()
    {
        _allowDragLeave = !_allowDragLeave;
        _iconDrag = _allowDragLeave ? IconNames.All.MoveFree : IconNames.All.MoveRestricted;
        await JS.InvokeVoidAsync("Lagoon.LgModal.UpdateDragLeave", _modalRef, _allowDragLeave);
    }

    /// <summary>
    /// Close modal without confirmation.
    /// </summary>
    protected async Task ForceCloseAsync()
    {
        CloseModalEventArgs args = new();
        if (OnClose.HasDelegate)
        {
            await OnClose.TryInvokeAsync(App, args);
        }
        if (!args.Cancel)
        {
            // Hide modal
            Visible = false;
            await VisibleChanged.TryInvokeAsync(App, false);
        }
        if (_shouldRefresh)
        {
            _shouldRefresh = false;
            StateHasChanged();
        }
        await Task.CompletedTask;
    }

    /// <summary>
    /// Invokable close function
    /// </summary>
    [JSInvokable]
    public Task CloseFromJsAsync()
    {
        return CloseAsync();
    }

    ///<inheritdoc/>
    Task IPageTitleHandler.SetPageErrorTitleAsync(ITab tab, string title, string iconName)
    {
        SetPageTitle(title, iconName);
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    Task IPageTitleHandler.SetPageTitleAsync(LgPage page)
    {
        StateHasChanged();
        SetPageTitle(page.Title, page.IconName, page.ModalCssClass);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method called by page when title or icon change.
    /// </summary>
    /// <param name="title">The model title.</param>
    /// <param name="iconName">The icon name.</param>
    /// <param name="cssClass">The CSS class.</param>
    private void SetPageTitle(string title, string iconName, string cssClass = null)
    {
        _pageTitle = title;
        _pageIconName = iconName;
        _pageCssClass = cssClass;
        StateHasChanged();
    }

    /// <summary>
    /// Gets the modal title.
    /// </summary>
    /// <returns></returns>
    protected string GetRenderTitle()
    {
        return (Title ?? _pageTitle).CheckTranslate();
    }

    /// <summary>
    /// Gets the modal title.
    /// </summary>
    /// <returns></returns>
    protected string GetRenderIconName()
    {
        return (IconName ?? _pageIconName).CheckTranslate();
    }

    /// <inheritdoc />
    public void RaiseFieldChanged()
    {
        StateHasChanged();
    }

    #endregion

}
