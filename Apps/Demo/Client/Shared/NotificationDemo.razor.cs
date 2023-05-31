using Demo.Shared.ViewModels;

namespace Demo.Client.Shared;

public partial class NotificationDemo : LgComponentBase
{

    #region events

    /// <summary>
    ///  On click action when notification item is clicked
    /// </summary>
    /// <param name="itemClicked"></param>
    /// <returns></returns>
    private void OnClickNotif(NotificationEventArgs<NotificationVm> itemClicked)
    {
        ShowInformation($"item clicked : {itemClicked.Item.Title}");
    }

    private string ItemIconName(NotificationVmBase item)
    {
        return IconNames.All.BellFill;
    }


    #endregion

}
