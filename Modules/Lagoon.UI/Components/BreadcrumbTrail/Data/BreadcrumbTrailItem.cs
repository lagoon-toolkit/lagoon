namespace Lagoon.UI.Components;

/// <summary>
/// Data to create a breadcrum trail item.
/// </summary>
public class BreadcrumbTrailItem
{

    #region properties

    /// <summary>
    /// Gets or sets the button icon name
    /// </summary>
    public string IconName { get; set; }

    /// <summary>
    /// Informations to create link to specific page.
    /// </summary>
    public LgPageLink Link { get; set; }

    /// <summary>
    /// Gets or sets OnClick action.
    /// </summary>
    public Action OnClick { get; set; }

    /// <summary>
    /// Gets or sets the button label
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The page URI to open when button is clicked.
    /// </summary>
    public string Uri { get; set; }

    #endregion

}
