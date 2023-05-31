using Microsoft.AspNetCore.Components.Routing;

namespace Lagoon.UI.Components;

/// <summary>
/// Navigate link for the LgRouter
/// </summary>
public partial class LgNavLink : LgHyperlink
{
    #region constants

    private const string ACTIVE_CSS_CLASS = "active";

    #endregion

    #region fields

    private bool _isActive;
    private string _absoluteUri;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the URI to navigate to. (Synonym of "Uri" property)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Parameter]
    public string Href { get => Uri; set => Uri = value; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        App.NavigationManager.LocationChanged += OnLocationChanged;
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _absoluteUri = string.IsNullOrEmpty(Uri) ? null : App.NavigationManager.ToAbsoluteUri(Uri).ToString();
        _ = UpdateState();
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        App.NavigationManager.LocationChanged -= OnLocationChanged;
        base.Dispose(disposing);
    }

    private void OnLocationChanged(object sender, LocationChangedEventArgs args)
    {
        if (UpdateState())
        {
            StateHasChanged();
        }
    }

    private bool UpdateState()
    {
        bool shouldBeActiveNow = _absoluteUri == App.NavigationManager.Uri;
        if (shouldBeActiveNow != _isActive)
        {
            _isActive = shouldBeActiveNow;
            return true;
        }
        return false;
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(_isActive, ACTIVE_CSS_CLASS);
    }

    #endregion

}
