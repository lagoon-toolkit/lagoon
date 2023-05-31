namespace Demo.Server;

/// <summary>
/// The entry point of the application.
/// </summary>
public class Program
{

    /// <summary>
    /// Application startup.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static void Main(string[] args)
    {
        LagoonWebApplication.Run<Main>(args);
    }

}
