namespace Lagoon.UI.Components;

/// <summary>
/// Informations needed to create link on specific page.
/// </summary>
public class LgPageLink : IPageLink
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

    /// <summary>
    /// New link instance.
    /// </summary>
    /// <param name="uri">URI of the page.</param>
    /// <param name="title">Title for the page.</param>
    /// <param name="iconName">Icon name for the page</param>
    public LgPageLink(string uri, string title, string iconName = "")
    {
        Uri = uri;
        Title = title;
        IconName = iconName;
    }
}
