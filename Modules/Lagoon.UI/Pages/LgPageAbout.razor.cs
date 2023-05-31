namespace Lagoon.UI.Components;

/// <summary>
/// Page to display application informations
/// </summary>
[AllowAnonymous]
[Route(ROUTE)]
public partial class LgPageAbout : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    public const string ROUTE = "about";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "#pgAboutTitle", IconNames.About);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Additional informations.
    /// </summary>
    protected AboutAdditionalInformationList AdditionalInformationList { get; set; } = new AboutAdditionalInformationList();

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        ModalCssClass = "about";
        await SetTitleAsync(Link());
    }

    /// <summary>
    /// Return the displayed name.
    /// </summary>
    /// <returns>The displayed name.</returns>
    protected virtual string GetApplicationName()
    {
        return App.ApplicationInformation.Name;
    }

    /// <summary>
    /// Return the displayed version.
    /// </summary>
    /// <returns>The displayed version.</returns>
    protected virtual string GetVersion()
    {
        return App.ApplicationInformation.Version.ToString() + (App.ApplicationInformation.IsDebug ? " [DEBUG]" : "");
    }

    /// <summary>
    /// Return the displayed owner logo.
    /// </summary>
    /// <returns>The displayed owner logo.</returns>
    protected virtual string GetOwnerLogo()
    {
        return App.BehaviorConfiguration.OwnerLogoUri;
    }

    /// <summary>
    /// Return the displayed customer logo.
    /// </summary>
    /// <returns>The displayed customer logo.</returns>
    protected virtual List<string> GetDevelopedByLogos()
    {
        return App.BehaviorConfiguration.DevelopedByLogoUri;
    }



    #endregion

}
