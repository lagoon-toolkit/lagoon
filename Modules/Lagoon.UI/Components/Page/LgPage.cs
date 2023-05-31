namespace Lagoon.UI.Components;

/// <summary>
/// Base class for "Page".
/// Use Title and Icon to set tab properties in Tabbed mode
/// </summary>
/// <remark>
/// By default an authenticated user is required but can be disabled by adding @attribute [AllowAnonymous()]
/// </remark>
[Authorize()]
public partial class LgPage : LgComponentBase
{

    #region constants

    /// <summary>
    /// Title show while loading.
    /// </summary>
    private const string LOADING_TITLE = "#LgPageTitleLoading";

    /// <summary>
    /// Icon name to show while loading.
    /// </summary>
    private const string LOADING_ICON_NAME = IconNames.Loading;

    #endregion

    #region fields

    /// <summary>
    /// Gets or sets if a descriptive property (Title, IconName, ...) changed.
    /// </summary>
    private bool _hasPendingChange;

    /// <summary>
    /// Gets or sets if the OnActivated event has been called.
    /// </summary>
    private bool _isActivated;

    /// <summary>
    /// Indicate if the closing event hasn't been cancelled.
    /// </summary>
    private bool _isCloseAccepted;

    /// <summary>
    /// Indicate if the page have been successfully loaded at least one time.
    /// </summary>
    private bool _isLoaded;

    /// <summary>
    /// Indicate if a loading task is already running.
    /// </summary>
    private bool _hasPendingTask;

    /// <summary>
    /// Icon name for the tab
    /// </summary>
    private string _iconName = IconNames.All.Image;

    /// <summary>
    /// Get if the page icon name has been set.
    /// </summary>
    private bool _setIconName;

    /// <summary>
    /// Title for the tab
    /// </summary>
    private string _title;

    /// <summary>
    /// Get if the page title has been set.
    /// </summary>
    private bool _setTitle;

    /// <summary>
    /// CSS class of the page.
    /// </summary>
    private string _cssClass;

    /// <summary>
    /// CSS class for the tab.
    /// </summary>
    private string _tabCssClass;

    /// <summary>
    /// The list of cancellable token for running tasks.
    /// </summary>
    internal readonly List<PageWaitingContext> _waitingContextList = new();

    private int _waitingState;

    // Flag used to track pending state for the change indicator
    private bool _previousHasModification;

#if DEBUG

    /// <summary>
    /// Time of the intialisation.
    /// </summary>
    private DateTime _initialized;

#endif

    #endregion

    #region cascading parameters

    /// <summary>
    /// Component provider for waiting context to LgComponent base components.
    /// </summary>
    [CascadingParameter]
    private LgPageContext PageContext { get; set; }

    /// <summary>
    /// Handle the activate, close for the page.
    /// </summary>
    [CascadingParameter]
    private IPageManager PageManager { get; set; }

    /// <summary>
    /// Class handling the close method.
    /// </summary>
    [CascadingParameter]
    private IPageCloser PageCloser { get; set; }

    /// <summary>
    /// Class handling the reload method.
    /// </summary>
    [CascadingParameter]
    private IPageReloader PageReloader { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets the child content.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region properties        

    /// <summary>
    /// Gets or sets icon name to display in the tab
    /// </summary>
    public string IconName
    {
        get => _iconName;
        private set
        {
            _setIconName = true;
            _iconName = value;
            _hasPendingChange = true;
        }
    }

    /// <summary>
    /// Indicates if the page has not completed its first load.
    /// </summary>
    public bool IsLoading => !_isLoaded;

    /// <summary>
    /// Get or sets the loading icon name to replace the icon name;
    /// </summary>
    internal string LoadingIconName { get; private set; }

    /// <summary>
    /// CSS classes to add to the page container.
    /// </summary>
    internal string ModalCssClass
    {
        get => _cssClass;
        set
        {
            _cssClass = value;
            _hasPendingChange = true;
        }
    }

    /// <summary>
    /// Tab information when tab navigation is enabled.
    /// </summary>
    public ITab Tab => PageContext?.Tab;

    /// <summary>
    /// CSS classes to add to the page container.
    /// </summary>
    public string TabCssClass
    {
        get => _tabCssClass;
        set
        {
            _tabCssClass = value;
            _hasPendingChange = true;
        }
    }

    /// <summary>
    /// Gets or sets title to display in the tab
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            _setTitle = true;
            _hasPendingChange = true;
        }
    }

    #endregion

    #region methods - Waiting Context

    /// <summary>
    /// Return a new waiting context with a cancellation token source.
    /// </summary>
    /// <returns>A new waiting context with a cancellation token source.</returns>
    internal WaitingContext OnGetNewWaitingContext()
    {
        PageWaitingContext wc = new(this);
        StartWaiting();
        return wc;
    }

    internal void StartWaiting()
    {
        _waitingState++;
        if (_waitingState == 1)
        {
            LoadingIconName = LOADING_ICON_NAME;
            TryUpdatePageTitleAsync();
        }
    }

    internal void StopWaiting()
    {
        _waitingState--;
        if (_waitingState == 0)
        {
            LoadingIconName = null;
            TryUpdatePageTitleAsync();
        }
    }

    #endregion

    #region methods - Initialization

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
#if DEBUG

        _initialized = DateTime.UtcNow;

#endif
        base.OnInitialized();
        // Change the tab title to Loading
        _title = LOADING_TITLE;
        _iconName = LOADING_ICON_NAME;
        TryUpdatePageTitleAsync();
        // Initialize waiting context provider
        PageContext?.RegisterPage(this);
        if (PageManager is IPageManager pageManager)
        {
            pageManager.OnRaiseActivatedEventAsync += OnPageManagerActivatedAsync;
            pageManager.OnRaiseClosingEventAsync += OnPageManagerClosingAsync;
            pageManager.OnRaiseCloseEventAsync += OnPageManagerCloseAsync;
        }
    }

    ///<inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        if (_isActivated)
        {
            return Task.CompletedTask;
        }
        _isActivated = true;
        return CallOnActivatedAsync(true);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        // Unregister waiting context provider
        PageContext?.UnregisterPage(this);
        // Unregistrer "Activated" event 
        if (PageManager is IPageManager pageManager)
        {
            pageManager.OnRaiseActivatedEventAsync -= OnPageManagerActivatedAsync;
            pageManager.OnRaiseClosingEventAsync -= OnPageManagerClosingAsync;
            pageManager.OnRaiseCloseEventAsync -= OnPageManagerCloseAsync;
        }
        // Try to cancel page activated tasks
        for (int i = _waitingContextList.Count; i > 0; i--)
        {
            try
            {
                _waitingContextList[i - 1].Cancel();
            }
            catch (ArgumentOutOfRangeException)
            {
                //Ignore task cancellation, if it's over before we can cancelled it
            }
        }
        // Ancestor disposing
        base.Dispose(disposing);
    }

    /// <summary>
    /// When the model of an EditForm change, we should update the state
    /// according to the modification made by the user
    /// </summary>
    internal void OnEditFormFieldChanged()
    {
        if (PageContext != null)
        {
            bool hasModification = ((IFormTrackerHandler)PageContext).HasModification();
            if (hasModification != _previousHasModification)
            {
                if (hasModification && (string.IsNullOrEmpty(TabCssClass) || !TabCssClass.Contains("field-change")))
                {
                    TabCssClass += " field-change";
                }
                else if (!hasModification && !string.IsNullOrEmpty(TabCssClass))
                {
                    TabCssClass = TabCssClass.Replace("field-change", "");
                }
                _hasPendingChange = true;
                _previousHasModification = hasModification;
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Method called when a tab is activated.
    /// </summary>
    private async Task OnPageManagerActivatedAsync()
    {
        await CallOnActivatedAsync(false);
        StateHasChanged();
    }

    /// <summary>
    /// Launch the activated method.
    /// </summary>
    /// <param name="firstActivation">Gets if it's the first activation of the page.</param>
    /// <returns>The task.</returns>
    private Task CallOnActivatedAsync(bool firstActivation)
    {
        if (_hasPendingTask)
        {
            return Task.CompletedTask;
        }
        _hasPendingTask = true;
        WaitingContext wc = OnGetNewWaitingContext();
        Task task = firstActivation
            ? OnLoadAsync(new PageLoadEventArgs(wc.CancellationToken))
            : OnActivatedAsync(new PageActivatedEventArgs(wc.CancellationToken));
        // Call customizable methods
        // Check if someting long is running during loading
        if (task.Status is not TaskStatus.RanToCompletion and not TaskStatus.Canceled)
        {
            return WaitOnActivatedAsync(firstActivation, task, wc);
        }
        else
        {
            wc.Dispose();
            _hasPendingTask = false;
            _isLoaded = true;
            UpdatePageTitleVoidAsync(firstActivation);
            // We return a completed task to avoid double render when then OnLoadAsync or OnActivatedAsync aren't overrided
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Wait until the OnLoadAsync or OnActivated task is ended to update the page state.
    /// </summary>
    /// <param name="firstActivation">Gets if it's the first activation of the page.</param>
    /// <param name="onEventTask">Task of the event.</param>
    /// <param name="wc">The waiting context.</param>
    private async Task WaitOnActivatedAsync(bool firstActivation, Task onEventTask, WaitingContext wc)
    {
        try
        {
            await onEventTask;
        }
        catch
        {
            // Ignore exceptions from task cancelletions.
            // Awaiting a canceled task may produce either an OperationCanceledException (if produced as a consequence of
            // CancellationToken.ThrowIfCancellationRequested()) or a TaskCanceledException (produced as a consequence of awaiting Task.FromCanceled).
            // It's much easier to check the state of the Task (i.e. Task.IsCanceled) rather than catch two distinct exceptions.
            if (!onEventTask.IsCanceled)
            {
                throw;
            }
        }
        finally
        {
            wc.Dispose();
            _hasPendingTask = false;
            _isLoaded = true;
        }
        await UpdatePageTitleAsync(firstActivation);
    }

    /// <summary>
    /// Init the page title after the page is activated.
    /// </summary>
    /// <param name="firstActivation">Gets if it's the first activation of the page.</param>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void UpdatePageTitleVoidAsync(bool firstActivation)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await UpdatePageTitleAsync(firstActivation);
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Init the page title after the page is activated.
    /// </summary>
    /// <param name="firstActivation">Gets if it's the first activation of the page.</param>
    private Task UpdatePageTitleAsync(bool firstActivation)
    {
        // Update the page title
        if (firstActivation)
        {
            if (_setTitle || _setIconName)
            {
                //Remove the loading values
                if (!_setTitle)
                {
                    _title = null;
                }
                if (!_setIconName)
                {
                    _iconName = null;
                }
            }
            else
            {
                // Try to set the page title from "Link" static method            
                System.Reflection.MethodInfo method = GetType().GetMethod("Link");
                if (method != null && method.Invoke(null, null) is LgPageLink link)
                {
                    _iconName = link.IconName;
                    _title = link.Title;
                }
                else
                {
                    _title = null;
                    _iconName = null;
                }
            }
        }
        // Update the page title
        return UpdatePageTitleAsync();
    }

    /// <summary>
    /// Method invoked when page is initialized.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual Task OnLoadAsync(PageLoadEventArgs e)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method invoked when page is re-activated(when the tab is selected).
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual Task OnActivatedAsync(PageActivatedEventArgs e)
    {
        return Task.CompletedTask;
    }

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        return  PageContext?.Tab is null || ((LgTabRenderData)PageContext.Tab).IsActive;
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_hasPendingChange)
        {
            await UpdatePageTitleAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
#if DEBUG
        DateTime stop = DateTime.UtcNow;
        if (firstRender)
        {
            Console.WriteLine($"Page {GetType().FriendlyName()} first render time : {stop.Subtract(_initialized).TotalMilliseconds} ms");
        }
#endif
    }

    #endregion

    #region methods - Change page title / Icon / CssClass

    /// <summary>
    /// Set the page title and icon.
    /// </summary>
    /// <param name="link">The new page title and icon name from link.</param>
    public Task SetTitleAsync(LgPageLink link)
    {
        return SetTitleAsync(link.Title, link.IconName);
    }

    /// <summary>
    /// Set the page title and icon.
    /// </summary>
    /// <param name="link">The new page title and icon name from link.</param>
    /// <param name="tabCssClass">CSS class to add to the tab header</param>
    public Task SetTitleAsync(LgPageLink link, string tabCssClass)
    {
        return SetTitleAsync(link.Title, link.IconName, tabCssClass);
    }

    /// <summary>
    /// Set the page title.
    /// </summary>
    /// <param name="title">The new page title.</param>
    /// <param name="iconName">The icon namefor the page.</param>
    public Task SetTitleAsync(string title, string iconName)
    {
        IconName = iconName;
        return SetTitleAsync(title);
    }

    /// <summary>
    /// Set the page title.
    /// </summary>
    /// <param name="title">The new page title.</param>
    /// <param name="iconName">The icon namefor the page.</param>
    /// <param name="tabCssClass">CSS class to add to the tab header</param>
    public Task SetTitleAsync(string title, string iconName, string tabCssClass)
    {
        TabCssClass = tabCssClass;
        return SetTitleAsync(title, iconName);
    }

    /// <summary>
    /// Set the page title.
    /// </summary>
    /// <param name="title">The new page title.</param>
    public Task SetTitleAsync(string title)
    {
        Title = title;
        return UpdatePageTitleAsync();
    }

    /// <summary>
    /// Ask to tabcontainer or main container to show the page title.
    /// </summary>
    private async Task UpdatePageTitleAsync()
    {
        IPageTitleHandler context = PageContext?.PageTitleHandler;
        if (context is not null)
        {
            await context.SetPageTitleAsync(this);
        }
        _hasPendingChange = false;
    }

    /// <summary>
    /// Ask to tabcontainer or main container to show the page title.
    /// </summary>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void TryUpdatePageTitleAsync()
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await UpdatePageTitleAsync();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    #region obsoletes

    /// <summary>
    /// Set the page title and icon.
    /// </summary>
    /// <param name="link">The new page title and icon name from link.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    //        [Obsolete("Use \"SetTitleAsync\" method.")]
    public void SetTitle(LgPageLink link)
    {
        _ = SetTitleAsync(link);
    }

    /// <summary>
    /// Set the page title.
    /// </summary>
    /// <param name="title">The new page title.</param>
    /// <param name="iconName">The icon namefor the page.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    //        [Obsolete("Use \"SetTitleAsync\" method.")]
    public void SetTitle(string title, string iconName)
    {
        _ = SetTitleAsync(title, iconName);
    }

    /// <summary>
    /// Set the page title.
    /// </summary>
    /// <param name="title">The new page title.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    //        [Obsolete("Use \"SetTitleAsync\" method.")]
    public void SetTitle(string title)
    {
        _ = SetTitleAsync(title);
    }

    #endregion

    #endregion

    #region methods - Close page

    /// <summary>
    /// Method called when the page is asked to close.
    /// </summary>
    /// <param name="sourceEvent">Source event.</param>
    /// <param name="confirmationMethod">Method to call when the validation message is confirmed.</param>
    private async Task<bool> OnPageManagerClosingAsync(PageClosingSourceEvent sourceEvent, Func<Task> confirmationMethod)
    {
        StringBuilder message = new();
        // If close already confirmed, don't ask again
        if (_isCloseAccepted)
        {
            return true;
        }
        // Initialise the argument
        PageClosingEventArgs e = new();
        // Check if the close must be cancelled
        await OnClosingAsync(e);
        // If close have been cancelled
        if (e.Cancel)
        {
            return false;
        }
        // Show confirmations messages if needed
        message.Append(e.ConfirmationMessage);
        if (await HasModificationAsync())
        {
            if (message.Length > 0)
            {
                message.Append("\n\n");
            }

            message.Append("LostChangesConfirmation".Translate());

        }
        // If Confirmation message have been set
        if (message.Length > 0)
        {
            e.Cancel = true;
            ShowConfirm(message.ToString(), () => OnCloseConfirmedAsync(confirmationMethod));
        }
        return !e.Cancel;
    }

    /// <summary>
    /// Indicate if the page contains unsaved modifications.
    /// </summary>
    /// <returns><c>true</c> if the page contains unsaved modifications.</returns>
    protected virtual async Task<bool> HasModificationAsync()
    {
        await Task.CompletedTask;
        return PageContext is not null && ((IFormTrackerHandler)PageContext).HasModification();
    }

    /// <summary>
    /// Execute pending action after a closing confirmation.
    /// </summary>
    /// <param name="callback">Action to execute.</param>
    /// <returns>The executing task</returns>
    private Task OnCloseConfirmedAsync(Func<Task> callback)
    {
        // Save the state "Accepted" to close the page
        _isCloseAccepted = true;
        // Execute again the orginal request
        return callback();
    }

    /// <summary>
    /// Method called when tab is closed.
    /// </summary>
    private Task OnPageManagerCloseAsync()
    {
        return OnCloseAsync(EventArgs.Empty);
    }

    /// <summary>
    /// Methods called when the interface asks to close the current page.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual async Task OnClosingAsync(PageClosingEventArgs e)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Methods called when the interface asks to close the current page.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual async Task OnCloseAsync(EventArgs e)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Close current page.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Close()
    {
        TryCloseAsync();
    }

    /// <summary>
    /// Encapsulate async method.
    /// </summary>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void TryCloseAsync()
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await CloseAsync();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Close current page without user confirmation.
    /// </summary>
    public Task ForceCloseAsync()
    {
        return CloseAsync(true);
    }

    /// <summary>
    /// Close current page.
    /// </summary>
    /// <param name="force">Indicate if we must bypass user confirmations.</param>
    public async Task CloseAsync(bool force = false)
    {
        if (PageCloser is not null)
        {
            await PageCloser.ClosePageAsync(this, force);
        }
    }

    #endregion

    #region methods - Reload page

    /// <summary>
    /// Reload current page.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Reload()
    {
        TryReloadAsync();
    }

    /// <summary>
    /// Encapsulate async method.
    /// </summary>
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    private async void TryReloadAsync()
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
    }

    /// <summary>
    /// Reload current page without user confirmation.
    /// </summary>
    public Task ForceReloadAsync()
    {
        return ReloadAsync(true);
    }

    /// <summary>
    /// Reload the current page.
    /// </summary>
    /// <param name="force">Indicate if we must bypass user confirmations.</param>
    public async Task ReloadAsync(bool force = false)
    {
        if (PageReloader is not null)
        {
            await PageReloader.ReloadPageAsync(this, force);
        }
    }

    /// <summary>
    /// Refresh content of the current page
    /// </summary>
    [Obsolete("Use \"ReloadAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Refresh()
    {
        Reload();
    }

    #endregion

    #region methods - ICommandListener

    /// <summary>
    /// Method invoked when a command is received from a sub component.
    /// </summary>
    /// <param name="args">The command event args.</param>
    public virtual Task OnBubbleCommandAsync(CommandEventArgs args)
    {
        OnBubbleCommand(args);
        if (args.Handled)
        {
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method invoked when a command is received from a sub component.
    /// </summary>
    /// <param name="args">The command event args.</param>
    public virtual void OnBubbleCommand(CommandEventArgs args)
    {
    }

    #endregion

}
