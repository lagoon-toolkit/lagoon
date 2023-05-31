namespace Lagoon.UI.Components;

/// <summary>
/// Left menu shown in sidebar.
/// </summary>
public partial class LgSideBar : LgComponentBase
{
    #region public properties

    /// <summary>
    /// Sidebar is collapsed ? 
    /// </summary>
    [Parameter]
    public bool Collapsed { get; set; } = false;

    /// <summary>
    /// Event call for mainlayout razor page to notify the  sidebar state change
    /// </summary>
    [Parameter]
    public EventCallback<ToggleSidebarEventArgs> OnToggleSidebar { get; set; }

    /// <summary>
    /// Gets or sets the Sidebar collapsed state (default value)
    /// </summary>
    [Parameter]
    public bool? DefaultCollapsed { get; set; }

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Collapsed = DefaultCollapsed ?? true;
    }

    /// <summary>
    /// OnCollapse event collapse button
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task OnCollapseAsync()
    {
        Collapsed = !Collapsed;
        // Notify to maincontentlayout the sidebar collapse or expand 
        // to define right content position into the page
        return OnToggleSidebar.TryInvokeAsync(App, new ToggleSidebarEventArgs() { Collapsed = Collapsed });
    }
    
    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("navbar navbar-vertical-fixed",
            Collapsed ? "sidebar-collapsed" : "sidebar-expanded", CssClass);
    }

    #endregion

}
