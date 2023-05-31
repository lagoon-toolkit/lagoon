using Lagoon.Shared.Model;

namespace Lagoon.Server.Application;


/// <summary>
/// Interface for LgEulaManager component used to get/set eula data
/// </summary>
public interface ILgEulaManager
{

    /// <summary>
    /// Return the list of configured eula
    /// </summary>
    List<Eula> GetAllEula();

    /// <summary>
    /// Create or update the EULA text for the given language key
    /// </summary>
    /// <param name="languageKey">Language key to create or update</param>
    /// <param name="eula">EULA text</param>
    /// <param name="updateVersion"><c>true</c> by default, users must revalidate eula. if <c>false</c> eula last modification version will not be updated</param>
    Task SetEula(string languageKey, string eula, bool updateVersion = true);

    /// <summary>
    /// Return the last update date for eula
    /// </summary>
    string GetEulaVersion();

    /// <summary>
    /// Force eula revalidation (last update date will invalid accepted eula made by users)
    /// </summary>
    Task ForceEulaRevalidation();

}
