namespace Lagoon.UI.Components;

/// <summary>
/// Interface to listen command raised by sub components.
/// </summary>
public interface ICommandListener
{

    /// <summary>
    /// The parent command listener.
    /// </summary>
    ICommandListener ParentCommandListener { get; }

    /// <summary>
    /// Method invoked when a command is received from a sub component.
    /// </summary>
    /// <param name="args">The command event args.</param>
    Task BubbleCommandAsync(CommandEventArgs args);

    /// <summary>
    /// Handle the command and propagate the event to the parent listener if not handled;
    /// </summary>
    /// <param name="args">The command event args.</param>
    public async Task RaiseBubbleCommandAsync(CommandEventArgs args)
    {
        await BubbleCommandAsync(args);
        if (!args.Handled && ParentCommandListener is not null)
        {
            await ParentCommandListener.RaiseBubbleCommandAsync(args);
        }
    }

}
