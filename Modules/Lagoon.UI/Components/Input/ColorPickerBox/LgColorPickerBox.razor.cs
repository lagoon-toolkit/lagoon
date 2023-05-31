using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// LgColorPickerB Component
/// </summary>
public partial class LgColorPickerBox : LgInputRenderBase<string>, IInputCommonProperties
{
    #region Fields

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    //internal ElementReference ElementRef { get; set; }

    private List<string> _colors;

    /// <summary>
    /// Gets or sets select render
    /// </summary>
    protected RenderFragment ColorPickerBox { get; set; }
    ///// <summary>
    ///// Display or not the items list
    ///// </summary>
    private bool _dropdownVisible;


    ///// <summary>
    ///// DotNet object reference
    ///// </summary>
    //internal IDisposable _dotnetRef;
    #endregion Fields

    #region properties

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef { get; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the Palette of availlabled colors.
    /// </summary>
    [Parameter]
    public List<string> Palette { get; set; }

    /// <summary>
    /// Gets or sets the show input properties
    /// </summary>
    [Parameter]
    public bool ShowInput { get; set; }

    /// <summary>
    /// Gets or sets if color list is open on focus
    /// </summary>
    [Parameter]
    public bool OpenOnFocus { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    internal LgDropDown DropDown { get; set; }

    #endregion

    #region LgInputRenderBase Members

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _colors = Palette ?? App.BehaviorConfiguration.ColorPalette;

    }

    ///<inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        builder.AddContent(0, ColorPickerBox);
    }
    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("form-group-colorpicker", "form-group", CssClass);
        builder.AddIf(ReadOnly, "form-group-ro");
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JS.InvokeVoidAsync("Lagoon.LgColorPicker.keyBoardController", DropDown.ElementRef, ShowInput);
        }
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return false;
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out string result, out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }

    #endregion LgInputRenderBase Members

    #region Private Methods

    /// <summary>
    /// On item selection
    /// </summary>
    /// <param name="item">item selected</param>
    /// <returns></returns>
    private Task ItemSelectionAsync(string item)
    {
        _dropdownVisible = false;
        return BaseChangeValueAsync(new ChangeEventArgs() { Value = item });
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValue = value?.ToString();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Text input change
    /// </summary>
    /// <returns></returns>
    private Task InputColorChangeAsync()
    {
        return ItemSelectionAsync(CurrentValue);
    }

    #endregion Private Methods
}
