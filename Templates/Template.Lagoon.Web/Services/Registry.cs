namespace TemplateLagoonWeb.Services;

/// <summary>
/// Services defined in this application.
/// </summary>
public static class Registry
{

    /// <summary>
    /// Register services for dependency injection.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterServices(IServiceCollection services)
    {
        // ** Technical services 
        // Register the service to send mails
//        services.AddSmtp();
        // ** Business services
//        services.AddScoped<FooService>();
    }

}
