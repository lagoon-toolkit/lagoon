using Lagoon.PDF.Services;

namespace Lagoon.Helpers;

/// <summary>
/// Extension method to add Docx services
/// </summary>
public static class Extensions
{

    #region IServiceCollection extensions

    /// <summary>
    /// Add PDF generation service
    /// </summary>
    /// <remarks>
    /// The ChromiumPath key in Lagoon section must be defined in appsettings.json
    /// </remarks>
    /// <param name="services">Extension method for IServiceCollection</param>
    /// <returns>IServiceCollection for chaining</returns>
    public static IServiceCollection AddPDF(this IServiceCollection services, IConfiguration config)
    {
        string chromiumPath = config["Lagoon:ChromiumPath"];
        if (string.IsNullOrWhiteSpace(chromiumPath)) throw new InvalidOperationException("The key 'ChromiumPath' must be set in appsettings.json under 'Lagoon' section. ");
        return services.AddScoped<IPuppeteerSharp, PDF.Services.PuppeteerSharp>(service => new PDF.Services.PuppeteerSharp(chromiumPath));
    }

    #endregion

}
