namespace Lagoon.Helpers;

/// <summary>
/// Friendly message list to replace original messages.
/// </summary>
public class AliasMessageCollection : List<AliasMessage>
{
    #region methods

    /// <summary>
    /// Add a new message alias.
    /// </summary>
    /// <param name="comparisonOperator">The method to detect the use of the alias.</param>
    /// <param name="includedText">The text that must be included in the original message.</param>
    /// <param name="message">The new message to replace the original message.</param>
    public void Add(AliasMessageComparisonOperator comparisonOperator, string includedText, string message)
    {
        Add(comparisonOperator, includedText, message, message);
    }

    /// <summary>
    /// Add a new message alias.
    /// </summary>
    /// <param name="comparisonOperator">The method to detect the use of the alias.</param>
    /// <param name="searchedText">The text that must be included in the original message.</param>
    /// <param name="message">The new message to replace the original message.</param>
    /// <param name="shortMessage">The new message to replace the original message when the context(fieldname) is not needed.</param>
    public void Add(AliasMessageComparisonOperator comparisonOperator, string searchedText, string message, string shortMessage)
    {
        switch (comparisonOperator)
        {
            case AliasMessageComparisonOperator.FromFormat:
                string endWidth = searchedText[(searchedText.LastIndexOf('}') + 1)..];
                string argPattern = "^" + System.Text.RegularExpressions.Regex.Escape(searchedText).Replace(@"\{0}", "(.*)") + "$";
                Add((originalMessage) => originalMessage.EndsWith(endWidth), message, shortMessage, argPattern);
                break;
            case AliasMessageComparisonOperator.EndsWidth:
                Add((originalMessage) => originalMessage.EndsWith(searchedText), message, shortMessage);
                break;
            case AliasMessageComparisonOperator.Contains:
                Add((originalMessage) => originalMessage.Contains(searchedText), message, shortMessage);
                break;
            case AliasMessageComparisonOperator.StartsWith:
                Add((originalMessage) => originalMessage.StartsWith(searchedText), message, shortMessage);
                break;
            case AliasMessageComparisonOperator.Equals:
                Add((originalMessage) => originalMessage == searchedText, message, shortMessage);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Add a new message alias.
    /// </summary>
    /// <param name="isMatch">Method indicate if the message must be replaced by the alias.</param>
    /// <param name="message">The new message to replace the original message.</param>
    public void Add(Func<string, bool> isMatch, string message)
    {
        Add(isMatch, message, message, null);
    }

    /// <summary>
    /// Add a new message alias.
    /// </summary>
    /// <param name="isMatch">Method indicate if the message must be replaced by the alias.</param>
    /// <param name="message">The new message to replace the original message.</param>
    /// <param name="shortMessage">The new message to replace the original message when the context(fieldname) is not needed.</param>
    /// <param name="argPattern">Regex pattern to extract the parameter from the message.</param>
    public void Add(Func<string, bool> isMatch, string message, string shortMessage, string argPattern = null)
    {
        Add(new AliasMessage(isMatch, message, shortMessage, argPattern));
    }

    /// <summary>
    /// Replace the message by an alias if needed and translate it if needed.
    /// </summary>
    /// <param name="originalMessage">The original error message.</param>
    /// <param name="shortMessage">The new message to replace the original message when the context(fieldname) is not needed.</param>
    /// <returns>The message to display.</returns>
    public string GetDisplayMessage(string originalMessage, bool shortMessage)
    {
        if (originalMessage is not null)
        {
            // We don't alias dico keys
            if (originalMessage.TranslationNeeded())
            {
                return originalMessage.CheckTranslate();
            }
            //We search for an alias
            foreach (AliasMessage alias in this)
            {
                if (alias.IsMatch(originalMessage))
                {
                    string arg = null;
                    if (alias.ArgumentPattern is not null)
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(originalMessage, alias.ArgumentPattern);
                        if (match.Success)
                        {
                            arg = match.Groups[1].Value.CheckTranslate();
                        }
                    }
                    originalMessage = shortMessage ? alias.ShortMessage : alias.Message;
                    return originalMessage.CheckTranslate(arg);
                }
            }
        }
        // No alias found, we return the orginal message
        return originalMessage;
    }

    #endregion

}
