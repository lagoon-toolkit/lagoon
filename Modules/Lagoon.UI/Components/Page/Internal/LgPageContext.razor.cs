using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Layout adding a LgPageWaitingContextProvider as cascading parameter aroud the page content.
/// </summary>
public partial class LgPageContext : LgErrorBoundary, IWaitingContextProvider, IFormTrackerHandler, ICommandListener
{

    #region fields

    private LgPage _page;

    private readonly List<LgFormTracker> _formTrackerList = new();

    #endregion

    #region cascading parameters

    /// <summary>
    /// LgApp instance.
    /// </summary>
    [CascadingParameter]
    private IPageDefaultLayoutProvider DefaultLayoutProvider { get; set; }

    /// <summary>
    /// Parent "FormTrackerHandler".
    /// </summary>
    [CascadingParameter]
    private IFormTrackerHandler ParentFormTrackerHandler { get; set; }

    /// <summary>
    /// Class handling the SetTitle method.
    /// </summary>
    [CascadingParameter]
    internal IPageTitleHandler PageTitleHandler { get; set; }

    /// <summary>
    /// Tab information when tab navigation is enabled.
    /// </summary>
    [CascadingParameter]
    public ITab Tab { get; set; }

    #endregion

    #region render fragments

    /// <summary>
    /// Gets the content to be rendered inside the layout.
    /// </summary>
    [Parameter]
    public RenderFragment Body { get; set; }

    #endregion

    #region properties

    List<LgFormTracker> IFormTrackerHandler.FormTrackerList => _formTrackerList;

    IFormTrackerHandler IFormTrackerHandler.ParentFormTracker => ParentFormTrackerHandler;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgPageContext()
    {
        ChildContent = RenderChildenContent;
    }

    #endregion

    #region methods

    /// <inheritdoc />
    // Derived instances of LayoutComponentBase do not appear in any statically analyzable
    // calls of OpenComponent<T> where T is well-known. Consequently we have to explicitly provide a hint to the trimmer to preserve
    // properties. (See LayoutComponentBase sources)
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LgPageContext))]
    public override Task SetParametersAsync(ParameterView parameters)
    {
        return base.SetParametersAsync(parameters);
    }

    /// <summary>
    /// Invoked by the base class when an error is being handled. The default implementation
    /// logs the error.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> being handled.</param>
    protected override async Task OnErrorAsync(Exception exception)
    {
        await PageTitleHandler.SetPageErrorTitleAsync(Tab, "pgErrorTitle".Translate(), IconNames.Error);
        await base.OnErrorAsync(exception);
    }

    private void RenderChildenContent(RenderTreeBuilder builder)
    {
        builder.AddCascadingValueComponent(1, this, (RenderFragment)RenderLayoutView, true);
    }

    private void RenderLayoutView(RenderTreeBuilder builder)
    {
        builder.OpenComponent<LayoutView>(0);
        builder.AddAttribute(1, nameof(LayoutView.Layout), DefaultLayoutProvider?.GetDefaultLayout());
        builder.AddAttribute(2, nameof(LayoutView.ChildContent), Body);
        builder.CloseComponent(); // LayoutView - 0
    }

    /// <summary>
    /// Return a new waiting context with a cancellation token source.
    /// </summary>
    /// <returns>A new waiting context with a cancellation token source.</returns>
    WaitingContext IWaitingContextProvider.GetNewWaitingContext()
    {
        return _page?.OnGetNewWaitingContext();
    }

    /// <inheritdoc />
    public void RaiseFieldChanged()
    {
        _page?.OnEditFormFieldChanged();
        // Inform potential parents of change of FormTracker state
        IFormTrackerHandler tracker = ParentFormTrackerHandler;
        while (tracker != null)
        {
            tracker.RaiseFieldChanged();
            tracker = tracker.ParentFormTracker;
        }
    }

    /// <summary>
    /// Register the page in the context.
    /// </summary>
    /// <param name="page">The page of the context.</param>
    internal void RegisterPage(LgPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Remove the page from the context when the page is disposed.
    /// </summary>
    /// <param name="page">The page.</param>
    internal void UnregisterPage(LgPage page)
    {
        if (ReferenceEquals(_page, page))
        {
            _page = null;
        }
    }

    #endregion

    #region methods - ICommandListener

    /// <summary>
    /// The parent command listener.
    /// </summary>
    ICommandListener ICommandListener.ParentCommandListener => null;

    /// <summary>
    /// Method invoked when a command is received from a sub component.
    /// </summary>
    /// <param name="args">The command event args.</param>
    public virtual async Task BubbleCommandAsync(CommandEventArgs args)
    {
        if (_page is not null)
        {
            await _page.OnBubbleCommandAsync(args);
        }
        if (!args.Handled)
        {
            throw new NotImplementedException($"The command \"{args.CommandName}\" has not been set as \"handled\". (Add \"args.Handled = true\" into the event handler)");
        }
    }

    #endregion

}
