namespace Lagoon.Console.Application;

/// <summary>
/// Exception to exit from the console application with an exit code different from zero.
/// </summary>
public class CriticalException : Exception
{

    /// <summary>
    /// The console exit code.
    /// </summary>
    public int ExitCode { get; }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="message">The description of the exception.</param>
    /// <param name="exitCode">The console exit code.</param>
    public CriticalException(string message, int exitCode = 1)
        : base(message)
    {
        if (exitCode == 0)
        {
            throw new ArgumentException($"The {nameof(exitCode)} can't be zero.");
        }
        ExitCode = exitCode;
    }
}
