using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Components;

/// <summary>
/// Add a validation summary inside EditForm. 
/// This component is automatically added in <see cref="LgEditForm" />
/// </summary>
public partial class LgValidationSummary : LgValidationBase
{

    #region parameters

    /// <summary>
    /// Optionnal title added before error message list
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        EditContext.OnValidationRequested += OnValidationRequested;
    }

    /// <summary>
    /// Unsubscribe from validation events
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        EditContext.OnValidationRequested -= OnValidationRequested;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Show validation message at the beginning of the form
    /// </summary>
    private void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
        StateHasChanged();
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("validation-error-summary");
        base.OnBuildClassAttribute(builder);
    }

    #endregion
}