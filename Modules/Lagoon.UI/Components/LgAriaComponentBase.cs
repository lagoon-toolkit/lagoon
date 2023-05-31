namespace Lagoon.UI.Components;

/// <summary>
/// Base component with ARIA attributes.
/// </summary>
public class LgAriaComponentBase : LgComponentBase
{

    #region fields

    /// <summary>
    /// Arial label
    /// </summary>
    private string _ariaLabel;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets aria label
    /// </summary>
    [Parameter]
    public string AriaLabel { get => _ariaLabel.CheckTranslate(); set => _ariaLabel = value; }

    /// <summary>
    /// Gets or sets id of component for aria label
    /// </summary>
    [Parameter]
    public string AriaLabelledBy { get; set; }

    #endregion

}
