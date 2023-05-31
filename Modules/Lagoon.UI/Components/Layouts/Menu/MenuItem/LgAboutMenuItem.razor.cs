namespace Lagoon.UI.Components;

/// <summary>
/// About page menu link
/// </summary>
public class LgAboutMenuItem : LgMenuItem
{

    #region dependency injection

    /// <summary>
    /// Window manager
    /// </summary>
    /// <value></value>
    [Inject]
    public WindowManager WindowManager { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var link = LgPageAbout.Link();
        Text = link.Title;
        IconName = link.IconName;
        OnClick = EventCallback.Factory.Create<ActionEventArgs>(this, () => {
            WindowManager.OpenModal(link.Uri, null, null, ModalSize.Medium);
        });
    }

    #endregion

}
