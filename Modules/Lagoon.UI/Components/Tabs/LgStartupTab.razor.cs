namespace Lagoon.UI.Components;

/// <summary>
/// Define a tab that must be openned when the application start.
/// </summary>
public class LgStartupTab : LgComponentBase
{

    #region cascading parameter

    /// <summary>
    /// Application tab container.
    /// </summary>
    [CascadingParameter]
    public LgTabContainer TabContainer { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets if the tab is closable.
    /// </summary>
    [Parameter]
    public bool Closable { get; set; }

    /// <summary>
    /// Gets or sets if the tab is activated by default.
    /// </summary>
    [Parameter]
    public bool Default { get; set; }

    /// <summary>
    /// The icon name of page.
    /// </summary>
    public string IconName { get; set; }

    /// <summary>
    /// Link to the page to open.
    /// </summary>
    [Parameter]
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Gets or sets the policy required to view the TextBox
    /// </summary>
    [Parameter]
    public string PolicyVisible { get; set; }

    /// <summary>
    /// The page title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The URI to use to show the page.
    /// </summary>
    public string Uri { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (TabContainer is LgAppTabContainerLayout appTabContainer)
        {
            appTabContainer.AddStartupPage(Default, Uri ?? Link?.Uri, Title ?? Link.Title, IconName ?? Link.IconName, Closable, PolicyVisible);
        }
    }

    #endregion

}
