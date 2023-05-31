
namespace Lagoon.UI.Components;

/// <summary>
/// Configuration LgSelect and LgSelectMultiple options
/// </summary>
public class LgSelectConfiguration
{

    /// <summary>
    /// Const to inherits value from the input.
    /// </summary>
    private const string INHERIT_FROM_INPUT = "inherit_input";

    /// <summary>
    /// Display or not the reset button.
    /// </summary>
    public bool? HideAlwaysResetButton { get; set; }

    /// <summary>
    /// Text for reset Button. if <c>null</c> the close icon is used.
    /// </summary>
    public string ResetText { get; set; } = INHERIT_FROM_INPUT;

    /// <summary>
    /// Aria label associeted to reset button.
    /// </summary>
    public string ResetTextAriaLabel { get; set; } = INHERIT_FROM_INPUT;

    /// <summary>
    /// Set a waiting time between the different characters before sending the request.
    /// Represented in milliseconds. Default: 500ms
    /// </summary>
    public int DebounceTime { get; set; } = 500;


    /// <summary>
    /// Set the input default for the select  configuration for undifined properties.
    /// </summary>
    /// <param name="input">The input configuration.</param>
    internal void ApplyInputDefault(LgInputConfiguration input)
    {
        if (!HideAlwaysResetButton.HasValue)
        {
            HideAlwaysResetButton = input.HideAlwaysResetButton;
        }
        if (INHERIT_FROM_INPUT.Equals(ResetText, System.StringComparison.OrdinalIgnoreCase))
        {
            ResetText = input.ResetText;
        }
        if (INHERIT_FROM_INPUT.Equals(ResetTextAriaLabel, System.StringComparison.OrdinalIgnoreCase))
        {
            ResetTextAriaLabel = input.ResetTextAriaLabel;
        }
    }

}
