namespace TemplateLagoonWeb.Client;

/// <summary>
/// The entry point of the application.
/// </summary>
public class Program
{
    /// <summary>
    /// Application startup.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static Task Main(string[] args)
    {
        return LagoonWebAssemblyApplication.RunAsync<Main>(args);
    }

}
