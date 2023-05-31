namespace Lagoon.Core.Models;


/// <summary>
/// Object used to describe a Push Notification message which will be handled by sw.showNotification.
/// See: <see href="https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerRegistration/showNotification"  />
/// </summary>
public class PushNotificationMessage
{

    /// <summary>
    /// The title that must be shown within the notification
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    // RQ: Disabled since there is a bug on android/chrome (at least Android 13) when clicking on action button
    // (which not open browser on action button while clicking on the notification itself works ...)
    /// <summary>
    /// An optional object that allows configuring the notification to display action button
    /// </summary>
    //[JsonPropertyName("actions")]
    [JsonIgnore]
#pragma warning disable IDE0051 // Remove unused private members
    private List<PushNotificationAction> Actions { get; set; }
#pragma warning restore IDE0051 // Remove unused private members

    /// <summary>
    /// A string containing the URL of an image to represent the notification when there is not enough space to display the notification itself such as for example, the Android Notification Bar. On Android devices, the badge should accommodate devices up to 4x resolution, about 96 by 96 px, and the image will be automatically masked. 
    /// </summary>
    [JsonPropertyName("badge")]
    public List<PushNotificationAction> Badge { get; set; }

    /// <summary>
    /// A string representing an extra content to display within the notification. 
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; }

    /// <summary>
    /// A string representing an extra content to display within the notification.
    /// Used by Lagoon service-worker as an action url (a client route) when clicking on a notification
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; set; }

    /// <summary>
    /// A string containing the URL of an image to be used as an icon by the notification. 
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    /// <summary>
    /// A string containing the URL of an image to be displayed in the notification. 
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; set; }

    /// <summary>
    /// A boolean that indicates whether to suppress vibrations and audible alerts when reusing a tag value. If options's <see cref="Renotify"/> is true and options's <see cref="Tag"/> is the empty string a TypeError will be thrown. The default is <c>false</c>. 
    /// </summary>
    [JsonPropertyName("renotify")]
    public bool Renotify { get; set; }

    /// <summary>
    /// Indicates that on devices with sufficiently large screens, a notification should remain active until the user clicks or dismisses it. If this value is absent or <c>false</c>, the desktop version of Chrome will auto-minimize notifications after approximately twenty seconds. The default value is <c>false</c>. 
    /// </summary>
    [JsonPropertyName("requireInteraction")]
    public bool RequireInteraction { get; set; }

    /// <summary>
    /// An ID for a given notification that allows you to find, replace, or remove the notification using a script if necessary. 
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag { get; set; }

    /// <summary>
    /// When set indicates that no sounds or vibrations should be made. If options's silent is <c>true</c> and options's <see cref="Vibrate"/> is present a TypeError exception will be thrown. The default value is <c>false</c>. 
    /// </summary>
    [JsonPropertyName("silent")]
    public bool Silent { get; set; }

    /// <summary>
    /// A vibration pattern to run with the display of the notification. A vibration pattern can be an array with as few as one member. The values are times in milliseconds where the even indices (0, 2, 4, etc.) indicate how long to vibrate and the odd indices indicate how long to pause. For example, [300, 100, 400] would vibrate 300ms, pause 100ms, then vibrate 400ms. Default is [100, 0, 100]
    /// </summary>
    [JsonPropertyName("vibrate")]
    public int[] Vibrate { get; set; } = new int[] { 300, 100, 400 };

    /// <summary>
    /// Return the JSON representation of this instance
    /// </summary>
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

/// <summary>
/// Object which describe an action on a <see cref="PushNotificationMessage"/>
/// </summary>
public class PushNotificationAction
{
    /// <summary>
    /// An url to open when the user click on the action
    /// </summary>
    [JsonPropertyName("action")]
    public string Action { get; set; }

    /// <summary>
    /// A string containing action text to be shown to the user.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// A string containing the URL of an icon to display with the action.
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

}
