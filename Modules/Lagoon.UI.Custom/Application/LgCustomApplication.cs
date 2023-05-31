namespace Lagoon.UI.Application;

/// <summary>
/// Base class for custom applications.
/// </summary>
public abstract class LgCustomApplication : LgApplication
{

    ///<inheritdoc/>
    protected override void InternalConfigureBehavior(ApplicationBehavior app)
    {
        base.InternalConfigureBehavior(app);
        app.ToolbarButtonSize = Components.ButtonSize.Medium;
        app.FrameToolbarButtonSize = Components.ButtonSize.Small;
        app.ActionPanelToolbarButtonSize = Components.ButtonSize.Small;
        // About page logos
        app.DevelopedByLogoUri = new List<string> { LOGO_URI_INFOTEL, LOGO_URI_DZ };
    }

}
