using Lagoon.UI.Demo.Pages;
using Lagoon.UI.Helpers;

namespace Lagoon.UI.Demo.Pages;

/// <summary>
/// Parameter page menu link
/// </summary>
public class SettingsMenuItem : LgMenuItem
{
    #region dependency injection

    /// <summary>
    /// Window manager
    /// </summary>
    /// <value></value>
    [Inject]
    public WindowManager WindowManager { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// is tabbed boolean
    /// </summary>
    [Parameter]
    public bool IsTabbed { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        LgPageLink link = PageSettings.Link();
        Text = link.Title;
        IconName = IconNames.All.Tools;
        OnClick = EventCallback.Factory.Create<ActionEventArgs>(this, () =>
        {
            WindowManager.OpenModal(link.Uri, null, null, ModalSize.Medium);
        });
    }

    #endregion

}
