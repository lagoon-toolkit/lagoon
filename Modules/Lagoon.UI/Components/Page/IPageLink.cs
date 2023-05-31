namespace Lagoon.UI.Components;

/// <summary>
/// 
/// </summary>
public interface IPageLink
{
    /// <summary>
    /// The URI to use to show the page.
    /// </summary>
    public string Uri { get; }

    /// <summary>
    /// The page title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// The icon name of page.
    /// </summary>
    public string IconName { get; }
}
