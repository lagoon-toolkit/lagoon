namespace Lagoon.Helpers;

/// <summary>
/// The ControlChars module contains constants used as control characters. These constants can be used anywhere in your code.
/// </summary>
public static class ControlChars
{

    /// <summary>
    /// Represents a backspace character.
    /// </summary>
    public const char Back = '\b';

    /// <summary>
    /// Represents a carriage return character.
    /// </summary>
    public const char Cr = '\r';

    /// <summary>
    /// Represents a carriage return/linefeed character combination.
    /// </summary>
    public const string CrLf = "\r\n";

    /// <summary>
    /// Represents a form feed character for print functions.
    /// </summary>
    public const char FormFeed = '\f';

    /// <summary>
    /// Represents a line feed character.
    /// </summary>
    public const char Lf = '\n';

    /// <summary>
    /// Represents a new line character for current system.
    /// </summary>
    [System.Obsolete("Use ")]
    public static string NewLine => System.Environment.NewLine;

    /// <summary>
    /// Represents a null character.
    /// </summary>
    public const char NullChar = '\0';

    /// <summary>
    /// Represents a double-quote character.
    /// </summary>
    public const char Quote = '"';

    /// <summary>
    /// Represents a tab character.
    /// </summary>
    public const char Tab = '\t';

    /// <summary>
    /// Represents a vertical tab character.
    /// </summary>
    public const char VerticalTab = '\v';


}
