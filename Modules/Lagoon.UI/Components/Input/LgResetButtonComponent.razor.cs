namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Reset button component
/// </summary>
public partial class LgResetButtonComponent
{
    #region parameters
    /// <summary>
    /// Display reset button if true
    /// </summary>
    [Parameter]
    public string ResetButtonText { get; set; }

    /// <summary>
    /// Display reset button if true
    /// </summary>
    [Parameter]
    public string ResetButtonAriaLabel { get; set; }

    /// <summary>
    /// EventCallBack fire when user click on reset
    /// </summary>
    [Parameter]
    public EventCallback OnClickReset { get; set; }

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Get the list of additional attributes to add to reset button.
    /// </summary>
    /// <returns>The list of additional attributes to add to component.</returns>
    protected IEnumerable<KeyValuePair<string, object>> GetResetButtonAttributes()
    {
        if (!string.IsNullOrEmpty(ResetButtonText))
        {
            yield return new KeyValuePair<string, object>("Text", ResetButtonText);
            if (!string.IsNullOrEmpty(ResetButtonAriaLabel))
            {
                yield return new KeyValuePair<string, object>("AriaLabel", ResetButtonAriaLabel);
            }
        }
        else
        {
            yield return new KeyValuePair<string, object>("IconName", IconNames.Close);
        }
    }

    /// <summary>
    /// Event fired when user clicks on the reset button
    /// </summary>
    private async Task OnResetAsync()
    {
        if (OnClickReset.HasDelegate)
        {
            await OnClickReset.InvokeAsync();
        }
    }
    #endregion
}
