using Lagoon.Console.Commands;
using Microsoft.Extensions.Hosting;

namespace Lagoon.Console.Application;

/// <summary>
/// Lagoon console application builder.
/// </summary>
public static class LagoonConsoleApplication
{
    /// <summary>
    /// Excute the configured application.
    /// </summary>
    /// <typeparam name="TApplication">The "Main" class.</typeparam>
    /// <param name="args">The console arguments.</param>
    public static int Run<TApplication>(string[] args) where TApplication : LgApplication, new()
    {
        int result = 0;
        RunAsync<TApplication>(args).ContinueWith(t => result = t.Result).Wait();
        return result;
    }

    /// <summary>
    /// Initialize a host for dependency injection.
    /// </summary>
    /// <typeparam name="TApplication">The "Main" class.</typeparam>
    /// <param name="args">The console arguments.</param>
    private static async Task<int> RunAsync<TApplication>(string[] args) where TApplication : LgApplication, new()
    {
        using (TApplication main = new())
        {
            int exitCode = 0;
            StreamWriter output = null;
            try
            {
                // Detect the file path to write the console output
                string filename = TryGetOutputFilePath(args);
                if (filename is not null)
                {
                    // Creating a text file to write the console outputs
                    output = new(filename, false, System.Text.Encoding.UTF8);
                    System.Console.SetOut(output);
                    System.Console.SetError(output);
                }
                // Create the Host
                IHostBuilder builder = Host.CreateDefaultBuilder(args);
                builder.ConfigureServices(main.BuildHost);
                builder.ConfigureLogging(main.BuildLogging);
                builder.UseCommandLineApplication<TApplication>(args, cmdapp =>
                {
                    cmdapp.ModelFactory = () => main;
                    main.ConfigureCommandLineApplication(cmdapp);
                });
                // Build and run the host
                using (IHost host = builder.Build())
                {
                    main.ConfigureHost(host);
                    exitCode = await host.RunCommandLineApplicationAsync();
                }
            }
            catch (Exception ex)
            {
                ConsoleEx.WriteError($"Error: {ex.Message}");
                main.TraceCriticalException(ex);
                exitCode = ex is CriticalException fex ? fex.ExitCode : -130479;
            }
            finally
            {
                // Close the file output if openned
                output?.Dispose(); 
            }
            return exitCode;
        }
    }

    /// <summary>
    /// Return the output filepath
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static string TryGetOutputFilePath(string[] args)
    {
        string filePath = null;
        // Search the filepath in the command line arguments
        string previousArg = string.Empty;
        foreach (string arg in args)
        {
            if (previousArg.Equals(SpecialOptionsConvention.OUT_OPTION, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(arg) && !arg.StartsWith("-"))
                {
                    filePath = arg;
                }
                break;
            }
            previousArg = arg;
        }
        if (filePath is null)
        {
            return null;
        }
        // Backup the old file if exists
        string backup = $"{filePath}.old";
        if (File.Exists(backup))
        {
            File.Delete(backup);
        }
        if (File.Exists(filePath))
        {
            File.Move(filePath, backup);
        }
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        return filePath;
    }

}
