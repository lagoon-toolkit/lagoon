namespace Lagoon.UI.Components;

/// <summary>
/// Toastr parameters
/// </summary>
public class LgToastrOptions
{

    /// <summary>
    /// Show a close button
    /// </summary>
    public bool CloseButton { get; set; } = true;

    /// <summary>
    /// Toastr debug mode
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Show new message on top if true, append a the end otherwise
    /// </summary>
    public bool NewestOnTop { get; set; } = false;

    /// <summary>
    /// Show a progress bar which indicates the display time
    /// </summary>
    public bool ProgressBar { get; set; } = true;

    /// <summary>
    /// Don't display to message at the same time with the same content
    /// </summary>
    public bool PreventDuplicates { get; set; } = false;

    /// <summary>
    /// Toastr position
    /// </summary>
    /// <value></value>
    public Position Position { get; set; } = Position.TopRight;

    /// <summary>
    /// Toastr position class.
    /// </summary>
    public string PositionClass
    {
        get
        {
            return Position switch
            {
                Position.BottomCenter => "toast-bottom-center",
                Position.TopCenter => "toast-top-center",
                Position.BottomLeft => "toast-bottom-left",
                Position.BottomRight => "toast-bottom-right",
                Position.TopLeft => "toast-top-left",
                Position.TopRight => "toast-top-right",
                _ => "",
            };
        }
    }

    /// <summary>
    /// Toastr onclick event callback
    /// </summary>
    public string Onclick { get; set; } = null;

    /// <summary>
    /// Show animation duration (ms)
    /// </summary>
    public int ShowDuration { get; set; } = 300;

    /// <summary>
    /// Hide animation duration (ms)
    /// </summary>
    /// <value></value>
    public int HideDuration { get; set; } = 1000;

    /// <summary>
    /// Delay before hiding messages non error messages.
    /// </summary>
    public int TimeOut { get; set; } = 5000;

    /// <summary>
    /// Delay before hidding messages after hover.
    /// </summary>
    public int ExtendedTimeOut { get; set; } = 1000;

    /// <summary>
    /// CSS Show transition
    /// </summary>
    public string ShowEasing { get; set; } = "swing";

    /// <summary>
    /// CSS hide transition
    /// </summary>
    public string HideEasing { get; set; } = "linear";

    /// <summary>
    /// CSS show transition animation
    /// </summary>
    public string ShowMethod { get; set; } = "fadeIn";

    /// <summary>
    /// CSS hide transition animation
    /// </summary>
    public string HideMethod { get; set; } = "fadeOut";

}