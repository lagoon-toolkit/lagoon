namespace Lagoon.UI.Attributes;

/// <summary>
/// Indicate if the service worker is enabled for this application.
/// </summary>
public sealed class ServiceWorkerEnabledAttribute : Attribute
{
    /// <summary>
    /// Indicate if the service worker is enabled for this application.
    /// </summary>
    public bool Value { get; }

    /// <summary>
    /// Initialise a new instance.
    /// </summary>
    /// <param name="value"></param>
    public ServiceWorkerEnabledAttribute(bool value)
    {
        Value = value;
    }
}
