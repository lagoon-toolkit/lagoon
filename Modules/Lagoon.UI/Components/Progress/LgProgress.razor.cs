namespace Lagoon.UI.Components;

/// <summary>
/// Component used to display progression.
/// </summary>
public partial class LgProgress : LgProgressBase
{
    #region fields

    /// <summary>
    /// Button Css class defined by kind value.
    /// </summary>
    private string _cssProgressKind;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the label position
    /// </summary>
    [Parameter]
    public ProgressLabelPosition LabelPosition { get; set; } = ProgressLabelPosition.Center;

    /// <summary>
    /// Gets or sets the striped flag.
    /// </summary>
    [Parameter]
    public bool Striped { get; set; }

    /// <summary>
    /// Gets or sets the animated flag.
    /// </summary>
    [Parameter]
    public bool Animated { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("progress", CssClass);
    }

    /// <summary>
    /// Gets the progress bar CSS class.
    /// </summary>
    /// <returns>The progress bar CSS class</returns>
    private string GetProgressBarClass()
    {
        Kind kind = Kind;
        _cssProgressKind = kind switch
        {
            Kind.Primary => "bg-primary",
            Kind.Default => "bg-default",
            Kind.Secondary => "bg-secondary",
            Kind.Success => "bg-success",
            Kind.Error => "bg-danger",
            Kind.Warning => "bg-warning",
            Kind.Info => "bg-info",
            _ => throw new NotImplementedException(),
        };

        LgCssClassBuilder builder = new("progress-bar");
        builder.Add(_cssProgressKind);
        builder.AddIf(Striped, "progress-bar-striped");
        builder.AddIf(Animated, "progress-bar-animated");
        return builder.ToString();
    }
    
    #endregion
}