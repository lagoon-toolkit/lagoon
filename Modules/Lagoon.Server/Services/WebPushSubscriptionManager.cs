using Lagoon.Core.Models;
using Lagoon.Server.Services.LagoonSettings;

namespace Lagoon.Server.Services;


/// <summary>
/// Interface for the service which manage PushNotification subcription/unsubscription.
/// Should not be used directly, <see cref="WebPushNotificationManager"/> for functionnal service
/// </summary>
public interface IWebPushSubscriptionManager
{

    /// <summary>
    /// Save a push registration for an user (without effect if already saved)
    /// </summary>
    /// <param name="reg">Object which contain the registration information</param>
    void SaveRegistration(WebPushSubscription reg);

    /// <summary>
    /// Update an existing registration
    /// </summary>
    /// <param name="reg">Registration to update</param>
    void UpdateRegistration(WebPushSubscription reg);

    /// <summary>
    /// Delete an existing registration
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="endpoint">Old endpoint which is invalid and should be deleted</param>
    void DeleteRegistration(string userId, string endpoint);

}

/// <summary>
/// Service which manage PushNotification subcription/unsubscription.
/// Should not be used directly, <see cref="WebPushNotificationManager"/> for functionnal service
/// </summary>
public class WebPushSubscriptionManager : IWebPushSubscriptionManager
{

    #region fields

    // Lagoon Settings Manager 
    private readonly ILagoonSettingsManager _lgSettingsManager;

    // Logger
    private readonly ILogger<WebPushSubscriptionManager> _logger;


    #endregion

    #region initialization

    /// <summary>
    /// Initialize a new WebPushNotification
    /// </summary>
    /// <param name="lgSettingsManager">LgSetting manager used to store and retrieve WebPush registration</param>
    /// <param name="logger">Logger</param>
    public WebPushSubscriptionManager(ILagoonSettingsManager lgSettingsManager, ILogger<WebPushSubscriptionManager> logger)
    {
        _lgSettingsManager = lgSettingsManager;
        _logger = logger;
    }

    #endregion

    #region IWebPushSubscriptionManager implementation

    /// <inheritdoc />
    public void SaveRegistration(WebPushSubscription reg)
    {
        var existingSubscription = _lgSettingsManager.GetLgSetting<WebPushSubscription>(reg.UserId, SettingParam.WebPushNotification, reg.Endpoint);
        // Check if there is already a subscrition saved for this userId/endpoint
        if (existingSubscription == null)
        {
            _lgSettingsManager.AddLgSetting(reg, reg.UserId, SettingParam.WebPushNotification, reg.Endpoint);
        }
    }

    /// <inheritdoc />
    public void UpdateRegistration(WebPushSubscription reg)
    {
        try
        {
            if (!string.IsNullOrEmpty(reg.OldEndpoint))
            {
                var prevSubscription = _lgSettingsManager.GetLgSetting<WebPushSubscription>(SettingParam.WebPushNotification, reg.OldEndpoint);
                if (prevSubscription != null)
                {
                    _lgSettingsManager.DeleteLgSetting(prevSubscription.UserId, SettingParam.WebPushNotification, prevSubscription.Endpoint);
                    _lgSettingsManager.AddLgSetting(reg, reg.UserId, SettingParam.WebPushNotification, reg.Endpoint);
                }
            }
        }
        catch (Exception ex)
        {
            // rq: this method should be called by the service-worker on an anonymous controller
            // don't fire any exception to avoid leak
            _logger.LogError(ex, "WebPushNotification - UpdateRegistration failed");
        }
    }

    /// <inheritdoc />
    public void DeleteRegistration(string userId, string endpoint)
    {
        _lgSettingsManager.DeleteLgSetting(userId, SettingParam.WebPushNotification, endpoint);
    } 

    #endregion

}
