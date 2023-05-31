namespace Lagoon.UI.Components;

/// <summary>
/// Command event informations.
/// </summary>
public class CommandEventArgs : ActionEventArgs
{

    #region fields

    /// <summary>
    /// An instance of object, or a boxed value, linked to the command action.
    /// </summary>
    private readonly object _commandArgument;

    /// <summary>
    /// The name of the command to execute.
    /// </summary>
    private readonly string _commandName;

    #endregion

    #region properties

    /// <summary>
    /// An instance of object, or a boxed value, linked to the command action.
    /// </summary>
    public object CommandArgument => _commandArgument;

    /// <summary>
    /// The name of the command to execute.
    /// </summary>
    public string CommandName => _commandName;

    /// <summary>
    /// Gets or sets if the command will be send to the parent components.
    /// </summary>
    public bool Handled { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="commandArgument">An instance of object, or a boxed value, linked to the command action.</param>
    public CommandEventArgs(string commandName, object commandArgument = null)
    {
        _commandName = commandName;
        _commandArgument = commandArgument;
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="waitingContext">Waiting context.</param>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="commandArgument">An instance of object, or a boxed value, linked to the command action.</param>
    public CommandEventArgs(WaitingContext waitingContext, string commandName, object commandArgument = null)
        : this(waitingContext?.CancellationToken, commandName, commandArgument)
    {
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="commandArgument">An instance of object, or a boxed value, linked to the command action.</param>
    public CommandEventArgs(CancellationToken? cancellationToken, string commandName, object commandArgument = null)
        : base(cancellationToken)
    {
        _commandName = commandName;
        _commandArgument = commandArgument;
    }

    #endregion

}
