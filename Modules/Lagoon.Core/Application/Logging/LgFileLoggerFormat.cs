namespace Lagoon.Core.Application.Logging;

/// <summary>
/// 
/// </summary>
public enum LgFileLoggerFormat
{

    /// <summary>
    /// A sequence of JSON object thats support '\n' inside the object declaration.
    /// (The object is ended when a line ends with "}\n".
    /// </summary>
    /// <remarks>The "\r\n" is not supported for line breaks.</remarks>
    IndentedJsonLines,

    /// <summary>
    /// The JSON lines format : Each line is a valid JSON value (The line ends with the '\n' char).
    /// <see href="https://jsonlines.org/" />
    /// </summary>
    JsonLines

}
