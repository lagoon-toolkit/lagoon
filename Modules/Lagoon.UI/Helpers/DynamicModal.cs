namespace Lagoon.UI.Components;

/// <summary>
/// Modal parameters
/// </summary>
internal class DynamicModal
{
    /// <summary>
    /// Gets or sets unique key
    /// </summary>
    /// <value></value>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets title
    /// </summary>
    /// <value></value>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets css class
    /// </summary>
    /// <value></value>
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets modal size
    /// </summary>
    /// <value></value>
    public ModalSize ModalSize { get; set; }

    /// <summary>
    /// Gets or sets display state
    /// </summary>
    /// <value></value>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Gets or sets if modal is an confirm
    /// </summary>
    /// <value></value>
    public bool IsConfirm { get; set; }

    /// <summary>
    /// Gets or sets confirmation message
    /// </summary>
    /// <value></value>
    public string ConfirmMessage { get; set; }

    /// <summary>
    /// Gets or sets on close modal event
    /// </summary>
    /// <value></value>
    public Func<Task> ConfirmCallback { get; set; }

    /// <summary>
    /// Gets or sets route of the modal content
    /// </summary>
    /// <value></value>
    public RouteData RouteData { get; set; }

}
