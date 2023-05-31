namespace Lagoon.UI.Components.Internal;

internal interface IPageTitleHandler
{
    /// <summary>
    /// Method called when an unhandled error is raised on the page.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <param name="title">The tab title.</param>
    /// <param name="iconName">The icon name.</param>
    Task SetPageErrorTitleAsync(ITab tab, string title, string iconName);

    /// <summary>
    /// Method called by page when title or icon change.
    /// </summary>
    /// <param name="page">Page instance.</param>
    Task SetPageTitleAsync(LgPage page);

}
