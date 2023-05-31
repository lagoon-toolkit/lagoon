using Lagoon.Core.Models;
using Lagoon.Server.Services;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Controller used to handle WebPush subscription
/// </summary>
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]")]
[Authorize]
public class LgWebPushNotificationController : LgControllerBase
{

    /// <summary>
    /// Push notification manager used to save/update/delete registration
    /// </summary>
    private readonly IWebPushSubscriptionManager _pushNotificationManager;

    /// <summary>
    /// Logger
    /// </summary>
    private readonly ILogger<LgWebPushNotificationController> _logger;

    /// <summary>
    /// Initialize a new <see cref="LgWebPushNotificationController"/>
    /// </summary>
    /// <param name="pushNotificationManager">Push notification manager used to save/update/delete registration</param>
    /// <param name="logger">Logger</param>
    public LgWebPushNotificationController(IWebPushSubscriptionManager pushNotificationManager, ILogger<LgWebPushNotificationController> logger)
    {
        _pushNotificationManager = pushNotificationManager;
        _logger = logger;
    }

    /// <summary>
    /// Save a new registration
    /// </summary>
    /// <param name="reg"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult AddRegistration(WebPushSubscription reg)
    {
        try
        {
            if (reg.IsValid)
            {
                reg.UserId = ContextUserId.ToString();
                _pushNotificationManager.SaveRegistration(reg);
            }
            else
            {
                _logger.LogWarning("A WebPush registration has been received but is invalid : '{0}'", reg);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// A subscibtion need to be updated
    /// </summary>
    /// <param name="reg">Subscription to update</param>
    /// <returns></returns>
    [HttpPut]
    [AllowAnonymous]
    public ActionResult UpdateRegistration(WebPushSubscription reg)
    {
        try
        {
            _pushNotificationManager.UpdateRegistration(reg);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Delete an existing subscription
    /// </summary>
    /// <param name="endpoint">Registration endpoint</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpDelete]
    public ActionResult DeleteRegistration(string endpoint)
    {
        try
        {
            _pushNotificationManager.DeleteRegistration(ContextUserId.ToString(), endpoint);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

}
