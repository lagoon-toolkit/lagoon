namespace Lagoon.UI.Components.Input.Internal;

/// <summary>
/// Interface to implement cancel and confirmation in input
/// </summary>
public interface IInputBusy
{
    /// <summary>
    /// Gets or sets the working in progress indicator.
    /// </summary>
    bool IsBusy { get; }

    /// <summary>
    /// Set input to the previous value.
    /// </summary>
    /// <returns></returns>
    Task CancelValueAsync();

}
