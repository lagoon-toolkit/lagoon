namespace Lagoon.UI.Components;

/// <summary>
/// Edit form behaviours.
/// </summary>
public class LgEditFormConfiguration
{
    /// <summary>
    /// Gets or sets how errors are displayed in forms.
    /// </summary>
    public EditFormErrorsDisplayOptions ErrorsDisplayOptions { get; set; } = EditFormErrorsDisplayOptions.ToastrGenericMessage | EditFormErrorsDisplayOptions.InlineValidation;

    /// <summary>
    /// Customize if required and/or optional input fields are indicated.
    /// </summary>
    public RequiredInputDisplayMode RequiredInputDisplayMode { get; set; } = RequiredInputDisplayMode.MandatoryOnly;

}
