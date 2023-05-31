using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Components;


/// <summary>
/// Validator used by input to display validation's message
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class LgValidationMessage<TValue> : LgValidationBase
{
    #region parameters

    /// <summary>
    /// Gets or sets the linked field's identifier.
    /// </summary>
    [Parameter]
    public Expression<Func<TValue>> For { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// The linked field.
    /// </summary>
    private FieldIdentifier _fieldIdentifier;

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _fieldIdentifier = FieldIdentifier.Create(For);
        EditContext.OnValidationStateChanged += HandleValidationStateChanged;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
        base.Dispose(disposing);
    }

    /// <summary>
    /// EditContext validation changes : notify ui
    /// </summary>
    /// <param name="o"></param>
    /// <param name="args"></param>
    private void HandleValidationStateChanged(object o, ValidationStateChangedEventArgs args)
    {
        StateHasChanged();
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("validation-messages-container");
        base.OnBuildClassAttribute(builder);
    }

    #endregion
}