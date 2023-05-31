using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods class.
/// </summary>
public static class LagoonExtensions
{

    /// <summary>
    /// Add swagger integration
    /// </summary>
    /// <param name="services">IServiceCollection extension method</param>
    /// <param name="assemblyName">Application assembly name</param>
    /// <param name="useXmlDocumentation">If true, xml documentation will be used to show method and parameters comments</param>
    public static void AddSwagger(this IServiceCollection services, string assemblyName, bool useXmlDocumentation = false)
    {
        // Register the Swagger generator, defining 1 or more Swagger documents
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo() { Title = $"{assemblyName} - Web Api Definitions", Version = "v1" });
            if (!string.IsNullOrEmpty(assemblyName) && useXmlDocumentation)
            {
                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{assemblyName}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, true);
                }
            }

            // Add access for swagger ui
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {

                        AuthorizationUrl = new Uri($"/connect/authorize", UriKind.Relative),
                        TokenUrl = new Uri($"/connect/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            //{$"{assemblyName}API", $"{assemblyName}API"}
                        }
                    }
                }
            });
            // To allow discovery of controller protected whith Authorize annotation
            c.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }

}

/// <summary>
/// Authorize filter for Swagger UI
/// </summary>
public class AuthorizeCheckOperationFilter : IOperationFilter
{

    /// <summary>
    /// Handle unauthorized status code
    /// </summary>
    /// <param name="operation">Operation response</param>
    /// <param name="context">Requestion context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool hasAuthorize =
          context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
          || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        }
                    ] = new[] {"swagger"}
                }
            };
        }
    }
}
