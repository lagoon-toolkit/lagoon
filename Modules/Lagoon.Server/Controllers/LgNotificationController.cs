using Lagoon.Model.Models;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Notification controller.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize()]
[ApiExplorerSettings(IgnoreApi = true)]
public class LgNotificationController : LgNotificationBaseController<NotificationBase, NotificationVmBase>
{

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="notificationManager">notification manager</param>
    public LgNotificationController(ILgNotificationManager<NotificationVmBase, NotificationBase> notificationManager) : base(notificationManager)
    {
    }
}
