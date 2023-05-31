namespace Lagoon.UI.Components;

/// <summary>
/// Describe the parameters needed to handle an action.
/// </summary>
public interface IActionComponent
{
    #region cascading parameters

    #endregion region

    #region parameters

    /// <summary>
    /// The argument value associated to the command.
    /// </summary>
    /// <remarks>Use primitive types or string to optimise the render performances.</remarks>
    object CommandArgument { get; }

    /// <summary>
    /// The name of the command to send to the handler of commands.
    /// </summary>
    string CommandName { get; }

    /// <summary>
    /// Gets or sets the confirmation message to show before doing the action.
    /// </summary>
    string ConfirmationMessage { get; }

    /// <summary>
    /// Informations to create link to specific page.
    /// </summary>
    LgPageLink Link { get; }

    /// <summary>
    /// Gets or sets the button Onclick eventCallback
    /// </summary>
    EventCallback<ActionEventArgs> OnClick { get; }

    /// <summary>
    /// The parent command listener.
    /// </summary>
    ICommandListener ParentCommandListener { get; }

    /// <summary>
    /// The name of the browser window in which the URI should be opened.
    /// </summary>
    string Target { get; }

    /// <summary>
    /// The URI to open.
    /// </summary>
    string Uri { get; }

    #endregion

    #region methods

    /// <summary>
    /// Indicate if the component has any action configured.
    /// </summary>
    /// <returns><c>true</c> if the component as any action configured.</returns>
    public bool HasAction()
    {
        return OnClick.HasDelegate || !string.IsNullOrEmpty(CommandName) || !string.IsNullOrEmpty(Uri ?? Link?.Uri);
    }

    #endregion

}
