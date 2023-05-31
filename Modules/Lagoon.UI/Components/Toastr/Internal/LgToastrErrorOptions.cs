namespace Lagoon.UI.Components.Internal;


internal class LgToastrErrorOptions : LgToastrCustomOptions
{

    /// <summary>
    /// Prevent message from auto-close after delay.
    /// </summary>
    public int TimeOut { get; set; } = 0;

}