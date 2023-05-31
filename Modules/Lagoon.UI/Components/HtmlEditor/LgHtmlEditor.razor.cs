using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// HTML editor component.
/// </summary>
public partial class LgHtmlEditor : LgInputRenderBase<string>
{
    #region public properties   

    /// <summary>
    /// The available font size list
    /// </summary>
    [Parameter]
    public string[] FontSizeList { get; set; } = new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "24", "30", "36", "48", "60", "72", "96" };

    /// <summary>
    /// Style applied on the jodit main container.
    /// </summary>
    /// <remarks>Theses styles are not in the content of the HtmlEditor</remarks>
    [Parameter]
    public Dictionary<string, string> Style { get; set; }

    /// <summary>
    /// The list of font-family available in the dropdow in the editor
    /// </summary>
    [Parameter]
    public Dictionary<string, string> FontFamilyList { get; set; } = new()
    {
       // { "Arial,Helvetica,Times,sans-serif", "Arial (default)" },
        { "Arial,Helvetica,sans-serif", "Arial" },
        { "Helvetica,sans-serif", "Helvetical" },
        { "Georgia,serif", "Georgia" },
        { "Impact,Charcoal,sans-serif", "Impact" },
        { "Tahoma,Geneva,sans-serif", "Tahoma" },
        { "'Times New Roman',Times,serif", "Times New Roman" },
        { "Verdana,Geneva,sans-serif", "Verdana" }
    };

    /// <summary>
    /// Font size unit defined. Must be 'px' or 'pt'. The default is 'px'.
    /// </summary>
    [Parameter]
    public string DefaultFontSizePoints { get; set; } = "px";

    /// <summary>
    /// Replace SVG icons to text
    /// </summary>
    [Parameter]
    public bool TextIcons { get; set; }

    /// <summary>
    /// Inline editing mode
    /// </summary>
    [Parameter]
    public bool Inline { get; set; }

    /// <summary>
    /// Whether the toolbar should be shown
    /// </summary>
    [Parameter]
    public bool Toolbar { get; set; } = true;

    /// <summary>
    /// Options specifies whether the editor is to have its spelling and grammar checked or not
    /// </summary>
    [Parameter]
    public bool Spellcheck { get; set; } = true;

    /// <summary>
    /// Language by default. if `auto` language set by document.documentElement.lang 
    /// || (navigator.language and navigator.language.substr(0, 2))
    /// || (navigator.browserLanguage and navigator.browserLanguage.substr(0, 2)) || 'en'
    /// </summary>
    [Parameter]
    public string Language { get; set; }

    /// <summary>
    /// Set Html editor height. It automatically will add resize handle.
    /// </summary>
    [Parameter]
    public string Height { get; set; }

    /// <summary>
    /// Allow disable/enable horizintal resizing
    /// </summary>
    [Parameter]
    public bool AllowResizeX { get; set; }


    /// <summary>
    /// Allow disable/enable vertical resizing
    /// </summary>
    [Parameter]
    public bool AllowResizeY { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    /// <summary>
    /// The list of buttons that appear in the editor's toolbar on large places (≥ options.sizeLG). Note - this is not the width of the device, the width of the editor
    /// For example :  "source", "bold", "italic", "underline", "strikethrough", "eraser", "superscript", "subscript", "ul", "ol", "indent", "outdent", "left", "font", "fontsize", "paragraph", "classSpan".
    /// </summary>
    [Parameter]
    public List<string> Buttons { get; set; }

    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs> OnInput { get; set; }

    #endregion

    #region private properties

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    /// <summary>
    /// Js Object 
    /// </summary>
    private JsObjectRef _elementObject;

    private bool _joditLoaded = false;

    private HtmlEditorOptions HtmlEditorOptions
    {
        get
        {
            return new HtmlEditorOptions()
            {
                Disabled = Disabled,
                TextIcons = TextIcons,
                Inline = Inline,
                Toolbar = Toolbar,
                Spellcheck = Spellcheck,
                Language = Language,
                Height = Height,
                AllowResizeX = AllowResizeX,
                AllowResizeY = AllowResizeY,
                Placeholder = Placeholder,
                Buttons = Buttons,
                DefaultFontSizePoints = DefaultFontSizePoints,
                Style = Style,
                FontSizeList = FontSizeList,
                FontFamilyList = FontFamilyList
            };
        }
    }
    #endregion

    #region method
    /// <summary> Remark ElementReference will be available only after OnAfterRender/OnAfterRenderAsync  </summary>
    /// <param name="firstRender">Is it the first render for this component ?</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!ReadOnly)
        {
            if (firstRender || !_joditLoaded)
            {
                // Load the Jodit script
                await IncludeJoditScript(JS);
                // Call a method which need to return a JSObject (that we must keep a reference)              
                _joditLoaded = true;
                _elementObject = await JS.ScriptGetNewRefAsync("Lagoon.HtmlEditor.Load", ElementRef, HtmlEditorOptions);
            }
        } 
        else if (_joditLoaded)
        {
            // In readonly mode, jodit must be destroyed
            await JS.ScriptGetNewRefAsync("Lagoon.HtmlEditor.Unload", _elementObject);
            _elementObject.Dispose();
            _joditLoaded = false;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Register the Jodit script.
    /// </summary>
    /// <param name="js">JS runtime.</param>
    /// <returns>The jodit script.</returns>
    public static ValueTask<string> IncludeJoditScript(IJSRuntime js)
    {
        return js.ScriptIncludeAsync("_content/Lagoon.UI/js/jodit.es2018.min.js");
    }

    /// <summary> Get the html data from the editor  </summary>
    protected async Task<string> GetContentAsync()
    {
        return await JS.InvokeAsync<string>("Lagoon.HtmlEditor.GetData", _elementObject);
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out string result, out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }

    ///<inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", "textarea");
            builder.AddMultipleAttributes(2, AdditionalAttributes);
            builder.AddAttribute(3, "placeholder", Placeholder.CheckTranslate());
            builder.AddAttribute(4, "class", $"form-control {CssClass}");
            builder.AddAttribute(5, "title", Tooltip.CheckTranslate());
            builder.AddAttribute(6, "value", CurrentValueAsString);
            builder.AddAttribute(7, "disabled", Disabled);
            builder.AddAttribute(8, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnChangeAsync));
            builder.AddAttribute(9, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, OnInputAsync));
            builder.AddAttribute(10, "id", ElementId);
            builder.AddElementReferenceCapture(11, capturedRef => ElementRef = capturedRef​​​​​);
            builder.CloseElement();
        }
        else
        {
            // Readonly render
            builder.OpenElement(0, "span");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", $"ntb-ro");
            builder.AddMarkupContent(3, !string.IsNullOrEmpty(CurrentValue) ? CurrentValue : "emptyReadonlyValue".Translate());
            builder.CloseElement();
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("form-group-htmleditor", "form-group", CssClass);
        builder.AddIf(ReadOnly, "form-group-ro");
        builder.AddIf(Disabled, "disabled");
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return false;
    }
    /// <summary>
    /// Invoked when input content change
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async void OnInputAsync(ChangeEventArgs args)
    {
        // Update current value (binding)
        if (OnInput.HasDelegate)
            await OnInput.TryInvokeAsync(App, args);
    }

    /// <summary>
    /// Invoked when content change (lost focus)
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnChangeAsync(ChangeEventArgs args)
    {
        await BaseChangeValueAsync(args);
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value.ToString();
        return Task.CompletedTask;
    }

    #endregion
}
