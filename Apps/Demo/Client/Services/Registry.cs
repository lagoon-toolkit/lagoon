using Demo.Shared.ViewModels;

namespace Demo.Client.Services;

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
        services.AddScoped<NavigationModeService>();
        services.AddNotification<NotificationVm>();
    }

}
