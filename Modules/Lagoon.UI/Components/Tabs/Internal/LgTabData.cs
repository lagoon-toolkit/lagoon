namespace Lagoon.UI.Components.Internal;

internal class LgTabData
{

    #region properties

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
    public string AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets if the tab is closable
    /// </summary>
    public bool Closable { get; set; }

    /// <summary>
    /// Gets or sets if the tab is enabled
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets if the tab can be moved
    /// </summary>
    public bool Draggable { get; set; }

    /// <summary>
    /// Gets or sets the tab icon name.
    /// </summary>
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets the tab unique identifier.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the tab panel content.
    /// </summary>
    public RenderFragment PanelContent { get; set; }

    /// <summary>
    /// Gets or sets if the tab can be pinned at the begining of the tab list
    /// </summary>
    public bool Pinned { get; set; }

    /// <summary>
    /// Gets or sets if tab content must be preload when created. Else tab content is load on first activation.
    /// if undifined, use the "PreloadTabContent" property of the parent "TabContainer".
    /// </summary>
    public bool PreloadContent { get; set; }

    /// <summary>
    /// Gets or sets if the tab content is reloaded when it's activate
    /// </summary>
    public bool RefreshOnActivate { get; set; }

    /// <summary>
    /// Gets or sets the tab title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets if the tab title or an icon has been set without a link.
    /// </summary>
    public bool FixedTitleAndIconName { get; set; }

    /// <summary>
    /// Gets or sets the tab tooltip.
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    public bool TooltipIsHtml { get; set; }

    /// <summary>
    /// Gets or sets Uri of the content
    /// </summary>
    public string Uri { get; set; }

    /// <summary>
    /// Gets or sets the content in an IFrame.
    /// </summary>
    public bool UseIFrame { get; set; }

    /// <summary>
    /// Get or set the Css class applied to the tab
    /// </summary>
    public string CssClass { get; set; }

    #endregion

}
