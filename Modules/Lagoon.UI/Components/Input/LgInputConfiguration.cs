namespace Lagoon.UI.Components;

/// <summary>
/// Configuration Input reset options
/// </summary>
public class LgInputConfiguration
{

    /// <summary>
    /// Display or not the reset button.
    /// </summary>
    public bool HideAlwaysResetButton { get; set; }

    /// <summary>
    /// Text for reset Button. if <c>null</c> the close icon is used.
    /// </summary>
    public string ResetText { get; set; }

    /// <summary>
    /// Aria label associeted to reset button.
    /// </summary>
    public string ResetTextAriaLabel { get; set; }

}
