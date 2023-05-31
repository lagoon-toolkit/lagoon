namespace Lagoon.Helpers;

/// <summary>
/// Information to replace a message by another.
/// </summary>
public class AliasMessage
{
    /// <summary>
    /// Regex pattern to extract the parameter from the message.
    /// </summary>
    public string ArgumentPattern { get; }

    /// <summary>
    /// Method indicate if the message must be replaced by the alias.
    /// </summary>
    public Func<string, bool> IsMatch { get; }

    /// <summary>
    /// The new message to replace the original message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The new message to replace the original message when the context(fieldname) is not needed.
    /// </summary>
    public string ShortMessage { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="isMatch">Method indicate if the message must be replaced by the alias.</param>
    /// <param name="message">The new message to replace the original message.</param>
    /// <param name="shortMessage">The new message to replace the original message when the context(fieldname) is not needed.</param>
    /// <param name="argPattern">Regex pattern to extract the parameter from the message.</param>
    public AliasMessage(Func<string, bool> isMatch, string message, string shortMessage, string argPattern)
    {
        IsMatch = isMatch;
        Message = message;
        ShortMessage = shortMessage;
        ArgumentPattern = argPattern;
    }

}
