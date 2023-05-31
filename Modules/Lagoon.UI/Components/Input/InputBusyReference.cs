using Lagoon.UI.Components.Input.Internal;

namespace Lagoon.UI.Components;

/// <summary>
/// Input cancellation and confirmation management
/// </summary>
public class InputBusyReference
{

    /// <summary>
    /// Gets or sets input reference.
    /// </summary>
    public IInputBusy Reference { get; set; }

    /// <summary>
    /// Gets or sets the expected input reference type.
    /// </summary>
    public Type ExpectedInputType { get; set; }

}
