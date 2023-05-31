namespace Lagoon.UI.Components;

/// <summary>
/// HTML editor options.
/// </summary>
public class HtmlEditorOptions
{

    /// <summary>
    /// The available font size list
    /// </summary>
    public string[] FontSizeList { get; set; } = new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "24", "30", "36", "48", "60", "72", "96" };

    /// <summary>
    /// Style applied on the jodit main container.
    /// </summary>
    /// <remarks>Theses styles are not in the content of the HtmlEditor</remarks>
    public Dictionary<string, string> Style { get; set; }

    /// <summary>
    /// The list of font-family available in the dropdow in the editor
    /// </summary>
    public Dictionary<string, string> FontFamilyList { get; set; }

    /// <summary>
    /// Font size unit defined. Must be 'px' or 'pt'. The default is 'px'.
    /// </summary>
    public string DefaultFontSizePoints { get; set; } = "px";

    /// <summary>
    /// Change the disabled state of the editor
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Change the readOnly state of the editor
    /// </summary>
    public bool Readonly { get; set; }

    /// <summary>
    /// Buttons in toolbar without SVG - only texts
    /// </summary>
    public bool TextIcons { get; set; }

    /// <summary>
    /// Inline editing mode
    /// </summary>
    public bool Inline { get; set; }

    /// <summary>
    /// Whether the toolbar should be shown
    /// </summary>
    public bool Toolbar { get; set; } = true;

    /// <summary>
    /// Options specifies whether the editor is to have its spelling and grammar checked or not
    /// </summary>
    public bool Spellcheck { get; set; } = true;

    /// <summary>
    /// Set Html editor height. It automatically will add resize handle.
    /// </summary>
    public string Height { get; set; }

    /// <summary>
    /// Allow disable/enable horizintal resizing
    /// </summary>
    public bool AllowResizeX { get; set; }

    /// <summary>
    /// Allow disable/enable vertical resizing
    /// </summary>
    public bool AllowResizeY { get; set; }

    /// <summary>
    /// Language by default. if `auto` language set by document.documentElement.lang 
    /// || (navigator.language and navigator.language.substr(0, 2))
    /// || (navigator.browserLanguage and navigator.browserLanguage.substr(0, 2)) || 'en'
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    public string Placeholder { get; set; } = string.Empty;

    /// <summary>
    /// Adaptative mode can cause problem inside a bootstrap tab. So disabled by default
    /// </summary>
    public bool ToolbarAdaptive { get; set; } = false;

    /// <summary>
    /// To disable status bar
    /// </summary>
    public bool AutoFocus { get; set; } = true;

    /// <summary>
    /// To disable status bar
    /// </summary>
    public bool ShowCharsCounter { get; set; } = false;

    /// <summary>
    /// To disable status bar
    /// </summary>
    public bool ShowWordsCounter { get; set; } = false;

    /// <summary>
    /// To disable status bar
    /// </summary>
    public bool ShowXPathInStatusbar { get; set; } = false;

    /// <summary>
    /// The list of buttons that appear in the editor's toolbar on large places (≥ options.sizeLG). Note - this is not the width of the device, the width of the editor
    /// For example :  "source", "bold", "italic", "underline", "strikethrough", "eraser", "superscript", "subscript", "ul", "ol", "indent", "outdent", "left", "font", "fontsize", "paragraph", "classSpan".
    /// </summary>
    public List<string> Buttons { get; set; }

}
