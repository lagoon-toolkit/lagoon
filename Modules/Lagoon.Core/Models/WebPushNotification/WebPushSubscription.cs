namespace Lagoon.Core.Models;


/// <summary>
/// Class used to describe a WebPush subsciption 
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/PushSubscription"/>
/// </summary>
public class WebPushSubscription
{

    /// <summary>
    /// Get or set the endpoint associated with the push subscription.
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Get or set the P256dh. An<see href="https://en.wikipedia.org/wiki/Elliptic-curve_Diffie%E2%80%93Hellman">Elliptic curve Diffie–Hellman</see> public key on the P-256 curve (that is, the NIST secp256r1 elliptic curve). The resulting key is an uncompressed point in ANSI X9.62 format.  
    /// </summary>
    public string P256dh { get; set; }

    /// <summary>
    /// Get or set the Auth. An authentication secret, as described in <see href="https://datatracker.ietf.org/doc/html/draft-ietf-webpush-encryption-08">Message Encryption for Web Push</see>
    /// </summary>

    public string Auth { get; set; }

    /// <summary>
    /// Get or set the old endpoint which will be non null in case of refreshing a subsciption (eg. when the browser has changed an <see cref="Endpoint"/>)
    /// </summary>
    public string OldEndpoint { get; set; }

    /// <summary>
    /// User associated to push notification endpoint
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// The subsciption is valid if we have <see cref="Endpoint"/>, <see cref="P256dh"/> and <see cref="Auth"/> non null.
    /// (only the <see cref="OldEndpoint"/> can be nul)
    /// </summary>
    [JsonIgnore]
    public bool IsValid
    {
        get
        {
            return !string.IsNullOrEmpty(Auth) && !string.IsNullOrEmpty(P256dh) && !string.IsNullOrEmpty(Auth);
        }
    }

    /// <summary>
    /// Return the json string representation
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

}
