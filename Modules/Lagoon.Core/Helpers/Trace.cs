namespace Lagoon.Helpers;


/// <summary>
/// Simplified access for trace management
/// </summary>
public class Trace
{

    /// <summary>
    /// Trace debug informations to the console.
    /// </summary>
    /// <param name="message">Message to show.</param>
    /// <param name="filePath">File name set by the compiler (leave empty).</param>
    /// <param name="memberName">Method name set by the compiler (leave empty).</param>
    public static void ToConsole(string message,
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
    {
        Console.WriteLine($"[{System.IO.Path.GetFileNameWithoutExtension(filePath).Replace('.', '-')}.{memberName}] {message}");
    }

    /// <summary>
    /// Trace debug informations to the console.
    /// </summary>
    /// <param name="caller">Instance calling the trace. Set "this" here.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="memberName">Method name set by the compiler (leave empty).</param>
    public static void ToConsole(object caller, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
    {
        Console.WriteLine($"[{caller.GetType().FriendlyName()}.{memberName}] {message}");
    }

}
