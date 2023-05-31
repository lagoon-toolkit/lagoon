using Lagoon.Hubs;
using Lagoon.Server;
using Lagoon.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using System.Web;
using AuthenticationOptions = Lagoon.Server.Application.AuthenticationOptions;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods class.
/// </summary>
public static class LagoonExtensions
{

    #region UseLagoon extensions

    /// <summary>
    /// Initialize the application with the Main.cs file.
    /// </summary>
    /// <typeparam name="TApp">The application "Main" class.</typeparam>
    /// <param name="hostBuilder">The IHost builder.</param>
    /// <returns>The IHost builder.</returns>
    public static IHostBuilder UseLagoon<TApp>(this IHostBuilder hostBuilder)
        where TApp : ILgApplication, new()
    {
        hostBuilder.ConfigureWebHostDefaults(builder => builder.UseLagoon<TApp>());
        return hostBuilder;
    }

    /// <summary>
    /// Initialize the application with the Main.cs file.
    /// </summary>
    /// <typeparam name="TApp">The application "Main" class.</typeparam>
    /// <param name="builder">The IWebHost builder.</param>
    /// <returns>The IWebHost builder.</returns>
    public static IWebHostBuilder UseLagoon<TApp>(this IWebHostBuilder builder)
        where TApp : ILgApplication, new()
    {
        // Register the configurator
        builder.UseStartup<LagoonWebApplicationStartup<TApp>>();
        // Specify the good application key; else the IWebHostEnvironment is not properly loaded and static files aren't resolved
        string startupAssemblyName = typeof(TApp).Assembly.GetName().Name;
        builder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName);
        return builder;
    }

    #endregion

    #region Notifications

    /// <summary>
    /// Register a service to activate notifications
    /// </summary>
    /// <param name="services">IServiceCollection extension method</param>
    /// <returns>IServiceCollection to chain methods</returns>
    public static IServiceCollection AddNotification<TNotificationManager>(this IServiceCollection services)
    {
        services.AddSignalR().AddMessagePackProtocol();

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        // Add access to LgNotificationManager
        Type notificationManagerType = typeof(TNotificationManager);
        Type notificationManagerInterfaceType = notificationManagerType.GetInterface("ILgNotificationManager`2") ?? throw new Exception($"The type \"{notificationManagerType}\" don't implement \"ILgNotificationManager\".");
        services.AddScoped(notificationManagerInterfaceType, notificationManagerType);
        return services;
    }

    #endregion

    #region Background Tasks

    /// <summary>
    /// Register a service to activate background tasks
    /// </summary>
    /// <param name="services">IServiceCollection extension method</param>
    /// <returns>IServiceCollection to chain methods</returns>
    public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
    {
        services.AddSignalR().AddMessagePackProtocol();

        services.AddSingleton<BackgroundTaskService>();
        return services;
    }

    #endregion

    #region IEndpointRouteBuilder

    /// <summary>
    /// Add Lagoon pre-required handlers
    /// </summary>
    /// <param name="endpoints">A contract for a route builder in an application.</param>
    /// <returns>The endpoints.</returns>
    public static IEndpointRouteBuilder MapLagoonNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>("hubs/notifhub").RequireAuthorization();
        return endpoints;
    }

    /// <summary>
    /// Add Lagoon pre-required handlers
    /// </summary>
    /// <param name="endpoints">A contract for a route builder in an application.</param>
    /// <returns>The endpoints.</returns>
    public static IEndpointRouteBuilder MapLagoonRemoteTasks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<BackgroundTaskHub>("hubs/remoteTaskHub");
        return endpoints;
    }

    /// <summary>
    /// Add handler to return HTTP 404 when api is not found.
    /// </summary>
    /// <param name="endpoints">A contract for a route builder in an application.</param>
    /// <param name="path">The root path of API requests.</param>
    /// <returns>The endpoints.</returns>
    public static IEndpointRouteBuilder MapApiNotFound(this IEndpointRouteBuilder endpoints, string path = "api/")
    {
        endpoints.MapFallback($"{path}{{**slug}}", context =>
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        });
        return endpoints;
    }

    /// <summary>
    /// Add handler to redirect to "/~/index.{EnvironmentName}.html" if exists, else to "/~/index.html".
    /// If <paramref name="allowAnonymous"/> is false and the user is not connected, the user is redirected to the authentication page.
    /// </summary>
    /// <param name="endpoints">The endpoints.</param>
    /// <param name="allowAnonymous">Allow the user to load the WebAssembly application, even if he's not authenticated.</param>
    /// <returns>The endpoints.</returns>
    public static IEndpointConventionBuilder MapLagoonFallback(this IEndpointRouteBuilder endpoints, bool allowAnonymous = false)
    {
        ArgumentNullException.ThrowIfNull(endpoints, nameof(endpoints));
        return endpoints
            // Throw 404 for request to non existent /api/* endpoints
            .MapApiNotFound()
            // Send the content of the index.html
            .MapFallback(CreateFallbackRequestDelegate(endpoints, allowAnonymous));
    }

    /// <summary>
    /// Delete method called by "MapAuthenticatedFallbackToFile".
    /// </summary>
    /// <param name="endpoints">The endpoints.</param>
    /// <param name="allowAnonymous">Allow the user to load the WebAssembly application, even if he's not authenticated.</param>
    /// <returns>The request delegate.</returns>
    private static RequestDelegate CreateFallbackRequestDelegate(IEndpointRouteBuilder endpoints, bool allowAnonymous = false)
    {
        IApplicationBuilder appBuilder = endpoints.CreateApplicationBuilder();
        appBuilder.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/Identity", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 500;
                throw new Exception("Identity routes are not registred !");
                //                    return;
            }
            if (allowAnonymous || (await context.AuthenticateAsync(IdentityConstants.ApplicationScheme)).Succeeded)
            {
                // Get the generated "index.html" path
                ILgApplication main = appBuilder.ApplicationServices.GetRequiredService<ILgApplication>();
                context.Request.Path = main.GetIndexPath();
                // Set endpoint to null so the static files middleware will handle the request.
                context.SetEndpoint(null);
                await next();
            }
            else
            {
                AuthenticationOptions authOptions = appBuilder.ApplicationServices.GetRequiredService<AuthenticationOptions>();
                string loginUrl = authOptions?.LoginUrl;
                if (string.IsNullOrEmpty(loginUrl))
                {
#if DEBUG //TOCLEAN
                    Lagoon.Helpers.Trace.ToConsole($"authOptions is null : {authOptions is null}");
#endif
                    context.Response.StatusCode = 401;
                }
                else
                {
                    // Add the URL to return to after the authentication
                    string returnUrl = $"{context.Request.Path}{context.Request.QueryString.Value}";
                    if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "/")
                    {
                        loginUrl += loginUrl.Contains('?') ? '&' : '?' + "returnUrl=" + HttpUtility.UrlEncode($"{context.Request.PathBase}{returnUrl}");
                    }
                    // Redirect to the login page
                    context.Response.Redirect($"{context.Request.PathBase}{loginUrl}");
                }
            }
        });
        appBuilder.UseStaticFiles();
        return appBuilder.Build();
    }

    #endregion

}