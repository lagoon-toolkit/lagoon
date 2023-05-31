namespace Lagoon.UI.Components;

/// <summary>
/// Banner.
/// </summary>
public partial class LgBanner : LgComponentBase
{
    #region parameters

    /// <summary>
    /// Gets or sets if the banner is closable.
    /// </summary>
    [Parameter]
    public bool Closable { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the
    /// created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Banner content
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the alert Class css
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the kind of the banner
    /// </summary>
    [Parameter]
    public Kind Kind { get; set; } = Kind.Info;

    /// <summary>
    /// Gets or sets the button icon
    /// </summary>
    [Parameter]
    public string IconName { get; set; }

    /// <summary>
    /// Gets or sets modal display
    /// </summary>
    [Parameter]
    public bool Show
    {
        get => _displayBanner;
        set
        {
            if (_displayBanner == value) return;
            _displayBanner = value;
        }
    }

    #endregion

    #region private properties

    /// <summary>
    /// Display banner
    /// </summary>
    private bool _displayBanner = true;

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        string cssAlertKind = Kind switch
        {
            Kind.Info => "alert-info",
            Kind.Success => "alert-success",
            Kind.Warning => "alert-warning",
            Kind.Error => "alert-danger",
            Kind.Primary => "alert-primary",
            Kind.Secondary => "alert-secondary",
            _ => "alert-info",
        };
        builder.Add("alert", cssAlertKind, CssClass);
    }

    private void CloseBanner()
    {
        _displayBanner = false;
        StateHasChanged();
    }

    #endregion
}
