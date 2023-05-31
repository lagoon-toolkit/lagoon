using Lagoon.Internal;
using Lagoon.Model.Collation;
using Lagoon.Model.Context;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Shared configuration controller.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
[Route(Routes.CONFIGURATION_ROUTE)]
[AllowAnonymous]
public class LgConfigurationController : LgControllerBase
{

    /// <summary>
    /// To access application configuration file
    /// </summary>
    private IConfiguration _config;
    private readonly IWithCollation _db;

    /// <summary>
    /// Initialisation.
    /// </summary>
    /// <param name="config">Configuration accessor.</param>
    /// <param name="db">The application DbContext.</param>
    public LgConfigurationController(IConfiguration config, ILgApplicationDbContext db)
    {
        _config = config;
        _db = db as IWithCollation;
    }

    /// <summary>
    /// Return the 'App' node from the appsettings[.development].json
    /// </summary>
    /// <returns>A json string representing the app node</returns>
    [HttpGet]
    public IActionResult Get()
    {
        // 
        string lagoon = _db?.CollationType is null
            ? ""
            : $"\"Lagoon\":{{\"Collation\":{(int)_db.CollationType.Value}}},";
        // Serialize the App section with a custom converter
        string json = JsonSerializer.Serialize(_config.GetSection("App"), new JsonSerializerOptions
        {
            Converters = { new ConfigurationConverter() },
            WriteIndented = false,        
        });
        json = "{" + lagoon + json + "}";
        return Ok(json);
    }
}

/// <summary>
/// JSon converter used to serialize an IConfigurationSection to JSON string
/// </summary>
internal class ConfigurationConverter : JsonConverter<IConfigurationSection>
{

    /// <summary>
    /// IConfigurationSection deserialization
    /// </summary>
    public override IConfigurationSection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // We don't use deserialisation
        return null;
    }

    /// <summary>
    /// IConfigurationSection serialization
    /// </summary>
    public override void Write(Utf8JsonWriter writer, IConfigurationSection value, JsonSerializerOptions options)
    {
        if (value is IConfigurationSection section)
        {
            if (section.Value is null)
            {
                writer.WriteStartObject(section.Key);
            }
            else
            {
                writer.WriteString(section.Key, section.Value);
                return;
            }
        }
        else
        {
            writer.WriteStartObject();
        }
        foreach (IConfigurationSection child in value.GetChildren())
        {
            Write(writer, child, options);
        }
        writer.WriteEndObject();
    }
}
