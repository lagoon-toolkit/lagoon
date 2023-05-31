using Lagoon.UI.Application;

namespace Lagoon.UI.Components;

/// <summary>
/// Lagoon extension methods.
/// </summary>
public static class LagoonExtensions
{

    /// <summary>
    /// Add a new client service notification
    /// </summary>
    /// <typeparam name="T">The Notification ViewModel type.</typeparam>
    /// <param name="services">The services registration collection.</param>
    /// <returns>The services registration collection.</returns>
    public static IServiceCollection AddNotification<T>(this IServiceCollection services) where T : NotificationVmBase
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        LgApplication app = serviceProvider.GetRequiredService<LgApplication>();
        using (var scope = serviceProvider.CreateScope())
        {
            INotificationService previous = scope.ServiceProvider.GetService<INotificationService>();
            if (previous is not null)
            {
                throw new Exception($"The method \"{nameof(AddNotification)}\" can be called only once.");
            }
        }
        services.AddScoped<INotificationService<T>, ClientNotificationService<T>>();
        services.AddScoped<INotificationService>((s) => s.GetService<INotificationService<T>>());
        ClientNotificationService<T>.RegisterToSignoutManager(app);
        return services;
    }

}
