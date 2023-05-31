namespace Lagoon.UI.Components.Layouts.Internal;


/// <summary>
/// Internal component used to render application header
/// </summary>
public partial class LgHeader : LgComponentBase
{

    #region fields

    /// <summary>
    /// Application name.
    /// </summary>
    private string _applicationName;

    /// <summary>
    /// Environment name.
    /// </summary>
    private string _environmentName;

    /// <summary>
    /// Environment color.
    /// </summary>
    private string _environmentColor;

    #endregion

    #region private properties

    /// <summary>
    /// Menu is collapsed ? (mobile)
    /// </summary>
    internal bool CollapsedNavMenu { get; set; } = true;

    #endregion region 

    #region methods

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _applicationName = App.ApplicationInformation.Name;
        _environmentName = App.ApplicationInformation.EnvironmentDisplayName;
        _environmentColor = App.ApplicationInformation.EnvironmentColor;
    }

    /// <summary>
    /// Navigation menu toggle management
    /// </summary>
    internal void ToggleNavMenu()
    {
        CollapsedNavMenu = !CollapsedNavMenu;
    }

    #endregion

}
