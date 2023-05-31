namespace Lagoon.UI.GridView.Internal;

/// <summary>
/// Cell confirmation parameters
/// </summary>
internal class GridViewCellConfirmation
{
    /// <summary>
    /// Gets or sets cancel button callback
    /// </summary>
    public Func<Task> CancelCallback { get; set; }

    /// <summary>
    /// Gets or sets confirmation button callback
    /// </summary>
    public Func<Task> ConfirmCallback { get; set; }

    /// <summary>
    /// Gets or sets confirmation modal visibility
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets confirmation message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Return message formatted to display
    /// </summary>
    /// <returns></returns>
    public MarkupString MarkupMessage()
    {
        return (MarkupString)(System.Net.WebUtility.HtmlEncode(Message)?.Replace("\n", "<br />"));
    }
}
