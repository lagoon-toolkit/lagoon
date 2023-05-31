namespace Lagoon.UI.Components;

/// <summary>
/// Component used to display progression.
/// </summary>
public partial class LgCircleProgress : LgProgressBase
{

    #region properties

    /// <summary>
    /// Gets the circle value.
    /// </summary>
    protected double ValueCirclePath => (CircleDash * ValuePercent) / 100;

    #endregion

    #region parameters

    /// <summary>
    /// Circle circumference
    /// </summary>
    [Parameter]
    public double CircleDash { get; set; } = 295.31;

    /// <summary>
    /// When it's 100%, and the kind parameter is different from "Warning" or "Error", the kind "Success" is used.
    /// </summary>
    [Parameter]
    public bool KindAutoSuccess { get; set; } = true;

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("progress-circle", CssClass);
        Kind kind = Kind;
        if(KindAutoSuccess && IsDone && kind != Kind.Error && kind!=Kind.Warning)
        {
            kind = Kind.Success;
        }
        builder.Add(kind switch
        {
            Kind.Primary => "primary",
            Kind.Default => "default",
            Kind.Secondary => "secondary",
            Kind.Success => "success",
            Kind.Error => "danger",
            Kind.Warning => "warning",
            Kind.Info => "info",
            _ => throw new NotImplementedException(),
        });
    }

    /// <summary>
    /// Return the name of the icon when the process is done.
    /// </summary>
    /// <returns>The name of the icon when the process is done.</returns>
    protected virtual string GetDoneIconName()
    {
        return Kind switch
        {
            Kind.Error => IconNames.Error,
            Kind.Warning => IconNames.Warning,
            _ => IconNames.Success,
        };
    }

    #endregion
}