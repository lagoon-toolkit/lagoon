using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Components;

/// <summary>
/// Provides a base implementation for validation related classes.
/// </summary>
public abstract class LgValidationBase : LgComponentBase
{

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the form editing context .
    /// </summary>
    [CascadingParameter]
    protected EditContext EditContext { get; set; }

    /// <summary>
    /// Gets or sets the error display option for this <see cref="LgValidationSummary" />.
    /// </summary>
    [Parameter]
    public EditFormErrorsDisplayOptions? ErrorsDisplayOptions { get; set; }

    /// <summary>
    /// Gets or sets the optional cascading parameter provided by <see cref="LgEditForm" />.
    /// </summary>
    [CascadingParameter]
    protected LgEditFormConfiguration CascadingErrorsDisplayOptions { get; set; }

    /// <summary>
    /// Gets the error display options.
    /// </summary>
    /// <remarks>The options are retrieved in that order: local => cascading => application configuration.</remarks>
    protected EditFormErrorsDisplayOptions ActualErrorsDisplayOptions
        => ErrorsDisplayOptions ?? CascadingErrorsDisplayOptions?.ErrorsDisplayOptions ?? App.BehaviorConfiguration.EditForm.ErrorsDisplayOptions;


    #region Render

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(CssClass);
    }

    #endregion
}