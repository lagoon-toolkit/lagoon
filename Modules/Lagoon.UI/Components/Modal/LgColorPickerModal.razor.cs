namespace Lagoon.UI.Components;

/// <summary>
/// Window to show color picker palette.
/// </summary>
public partial class LgColorPickerModal : LgComponentBase
{

    #region fields

    private string _selectedColor;

    /// <summary>
    /// Color list
    /// </summary>
    private List<string> _colors;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    [Parameter]
    public EventCallback<string> OnClose { get; set; }

    /// <summary>
    /// Gets or sets the modal cssclass
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets modal display
    /// </summary>
    [Parameter]
    public bool Visible { get; set; }

    /// <summary>
    /// Event raised when modal show value.
    /// </summary>
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    [Parameter]
    public string SelectedColor { get; set; }

    /// <summary>
    /// Palette of availlabled colors.
    /// </summary>
    [Parameter]
    public List<string> Palette { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _selectedColor = SelectedColor?.ToUpperInvariant();
        _colors = Palette ?? App.BehaviorConfiguration.ColorPalette;

    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(CssClass);
    }

    #endregion

    #region events

    /// <summary>
    /// </summary>
    /// <param name="selectedColor">Selected color</param>
    /// <returns>A <see cref="Task"/>.</returns>
    internal async Task OnCloseClickAsync(string selectedColor)
    {
        SelectedColor = selectedColor;
        if (OnClose.HasDelegate)
        {
            await OnClose.TryInvokeAsync(App, selectedColor);
        }
    }

    #endregion
}