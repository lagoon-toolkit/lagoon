using Lagoon.Services;

namespace Microsoft.Extensions.DependencyInjection;


/// <summary>
/// Extension methods class.
/// </summary>
public static class LagoonExtensions
{

    #region Email / SMTP

    /// <summary>
    /// Register a service to send email
    /// </summary>
    /// <param name="services">IServiceCollection extension method</param>
    /// <returns>IServiceCollection to chain methods</returns>
    public static IServiceCollection AddSmtp(this IServiceCollection services)
    {
        return services.AddTransient<ISmtp, EmailSender>(i =>
        {
            IConfiguration config = i.GetRequiredService<IConfiguration>();
            return new EmailSender(
                config["Lagoon:Smtp:Host"],
                config.GetValue("Lagoon:Smtp:Port", 25),
                config.GetValue("Lagoon:Smtp:EnableSSL", false),
                config["Lagoon:Smtp:UserName"],
                config["Lagoon:Smtp:Password"],
                config["Lagoon:Smtp:Sender"]
            );
        });
    }

    #endregion

}
