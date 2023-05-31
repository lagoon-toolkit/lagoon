using Lagoon.Core.Models;
using Lagoon.Server.Services.LagoonSettings;
using System.Net;
using WebPush;

namespace Lagoon.Server.Services;


/// <summary>
/// Service used to manage WebPush notification
/// </summary>
public interface IWebPushNotificationManager
{

    /// <summary>
    /// Send a message/notification to all registration associated to the user
    /// </summary>
    /// <param name="userId">User id for wich we want send notification to</param>
    /// <param name="title">Message title</param>
    /// <param name="message">Message body</param>
    /// <param name="icon">Optional icon</param>
    /// <returns><c>true</c> if a notification has been sent successfully, <c>false</c> otherwise</returns>
    Task<bool> SendNotificationAsync(string userId, string title, string message, string icon);

    /// <summary>
    /// Send a message/notification to all registration associated to the user
    /// </summary>
    /// <param name="userId">User id for wich we want send notification to</param>
    /// <param name="message">Message to send</param>
    /// <returns><c>true</c> if a notification has been sent successfully, <c>false</c> otherwise</returns>
    Task<bool> SendNotificationAsync(string userId, PushNotificationMessage message);

    /// <summary>
    /// Return the list of userId which have subscribed to WebPush notification
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetUserWithNotification();

}

/// <summary>
/// Service used to manage WebPush notification
/// </summary>
public class WebPushNotificationManager : IWebPushNotificationManager
{

    #region fields

    // Lagoon Settings Manager 
    private readonly ILagoonSettingsManager _lgSettingsManager;

    // Logger
    private readonly ILogger<WebPushNotificationManager> _logger;

    // Keys container for WebPush notification
    private VapidDetails _vapidDetails;

    #endregion

    #region initialization

    /// <summary>
    /// Initialize a new WebPushNotification
    /// </summary>
    /// <param name="lgSettingsManager">LgSetting manager used to store and retrieve WebPush registration</param>
    /// <param name="config">Used to retrieve the public / private key from appSettings.json</param>
    /// <param name="logger">Logger</param>
    public WebPushNotificationManager(ILagoonSettingsManager lgSettingsManager, IConfiguration config, ILogger<WebPushNotificationManager> logger)
    {
        _lgSettingsManager = lgSettingsManager;
        _logger = logger;
        _vapidDetails = new VapidDetails(config["VapidSubject"], config["App:VapidPublicKey"], config["VapidPrivateKey"]);
    }

    #endregion

    #region IWebPushNotificationManager implementation

    /// <inheritdoc />
    public Task<bool> SendNotificationAsync(string userId, string title, string message, string icon = "")
    {
        return SendNotificationAsync(userId, new PushNotificationMessage() { Title = title, Body = message, Icon = icon });
    }

    /// <inheritdoc />
    public Task<bool> SendNotificationAsync(string userId, PushNotificationMessage message)
    {
        return InternalSendNotificationAsync(userId, message.ToString());
    }

    /// <inheritdoc />
    public IEnumerable<string> GetUserWithNotification()
    {
        return _lgSettingsManager.GetUsersBySettingParam(SettingParam.WebPushNotification);
    }

    #endregion

    #region methods

    /// <summary>
    /// Send a WebPush notification
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<bool> InternalSendNotificationAsync(string userId, string message)
    {
        // Retrieve all registration associated to the user
        var registrations = _lgSettingsManager.GetLgSettingFromSubParamContains<WebPushSubscription>(userId, SettingParam.WebPushNotification, "");
        bool oneNotifSendAtLeast = false;
        if (registrations != null)
        {
            foreach (var reg in registrations)
            {
                try
                {
                    var webPushClient = new WebPushClient();
                    await webPushClient.SendNotificationAsync(new PushSubscription()
                    {
                        Endpoint = reg.Endpoint,
                        Auth = reg.Auth,
                        P256DH = reg.P256dh
                    }, message, _vapidDetails);
                    oneNotifSendAtLeast = true;
                }
                catch (WebPushException ex)
                {
                    if (ex.HttpResponseMessage.StatusCode == HttpStatusCode.Gone || ex.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        // In case of invalid / expired subscription, delete them silently
                        _logger.LogWarning($"Deleting expired / invalid subscription. StatusCode:{ex.StatusCode}, Message:{ex.Message}. Subscription: {reg}");
                        _lgSettingsManager.DeleteLgSetting(userId, SettingParam.WebPushNotification, reg.Endpoint);
                    }
                    else if (ex.StatusCode == HttpStatusCode.RequestEntityTooLarge || ex.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                    {
                        throw ex;
                    }
                    else
                    {
                        // Don't fire an exeption
                        _logger.LogWarning(ex, $"An error occured when sending pushnotification to {userId}. StatusCode: {ex.StatusCode}");
                    }
                }
            }
        }
        return oneNotifSendAtLeast;
    } 

    #endregion

}
