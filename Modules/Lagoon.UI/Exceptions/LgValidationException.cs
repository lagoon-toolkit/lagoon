namespace Lagoon.UI.Helpers;

/// <summary>
/// Used to handle form validation exception
/// </summary>
public class LgValidationException : UserException
{

    /// <summary>
    /// Get or set the list of erros (key shoulb be a property name)
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; set; } 

    /// <summary>
    /// Initialize a new <see cref="LgValidationException"/>
    /// </summary>
    public LgValidationException()
    { }

    /// <summary>
    /// Initialize a new <see cref="LgValidationException"/>
    /// </summary>
    /// <param name="message">Message</param>
    public LgValidationException(string message) : base(message)
    { }

    /// <summary>
    /// Initialize a new <see cref="LgValidationException"/>
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="inner">Exception</param>
    public LgValidationException(string message, Exception inner) : base(message, inner)
    { }

    /// <summary>
    /// Initialize a new <see cref="LgValidationException"/>
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="inner">Exception</param>
    /// <param name="errors">Errors</param>
    public LgValidationException(string message, Exception inner, Dictionary<string, List<string>> errors) : base(message, inner)
    {
        Errors = errors;
    }

}
