namespace Lagoon.UI.Components;

/// <summary>
/// LgCardSeparator (vertical) component for LgCard
/// </summary>
public partial class LgCardSeparator : LgComponentBase
{

    #region Public parameters
    /// <summary>
    /// Gets or sets css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    #endregion 

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("vertical-separator", CssClass);
    }

    #endregion region
}
