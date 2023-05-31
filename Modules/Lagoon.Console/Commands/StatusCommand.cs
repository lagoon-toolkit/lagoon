using Lagoon.Console.Application;
using McMaster.Extensions.CommandLineUtils;

namespace Lagoon.Console.Commands;

/// <summary>
/// Command "status" to show the status of the application's resources.
/// </summary>
[Command(Name = "status", Description = "Show the status of the application's resources.")]
public class StatusCommand
{

    /// <summary>
    /// Show the status of the application's resources.
    /// </summary>
    /// <param name="app">The main application</param>
    /// <returns>0 if everything is ok.</returns>
    /// <remarks>Override the "ShowStatus" of "Main.cs" to customize.</remarks>
#pragma warning disable CA1822 // Mark members as static
    public int OnExecute(ILgApplication app)
#pragma warning restore CA1822 // Mark members as static
    {
        return ((LgApplication)app).ShowStatus();
    }

}
