namespace Lagoon.Model.Models;


/// <summary>
/// Lagoon settings table.
/// </summary>
public abstract class LagoonSettingsBase
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Paramter
    /// </summary>
    public SettingParam Param { get; set; }

    /// <summary>
    /// Sub parameter
    /// </summary>
    public string SubParam { get; set; }

    /// <summary>
    /// Json (data)
    /// </summary>
    public string Json { get; set; }

}
