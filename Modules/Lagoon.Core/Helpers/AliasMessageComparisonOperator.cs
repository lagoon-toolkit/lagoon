namespace Lagoon.Helpers;

/// <summary>
/// Method to detect the use of the alias.
/// </summary>
public enum AliasMessageComparisonOperator
{
    /// <summary>
    /// The original message match exactly the searched text.
    /// </summary>
    Equals,

    /// <summary>
    /// The original message starts with the searched text.
    /// </summary>
    StartsWith,

    /// <summary>
    /// The original message contains with the searched text.
    /// </summary>
    Contains,

    /// <summary>
    /// The original message ends with the searched text.
    /// </summary>
    EndsWidth,

    /// <summary>
    /// The orginal message response to a format.
    /// </summary>
    FromFormat
}
