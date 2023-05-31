namespace Lagoon.Server.Application.EulaManager;


/// <summary>
/// Used to deserialize 'Lagoon:EulaFiles' appsettings node in a typed collection
/// </summary>
internal class EulaFile
{

    /// <summary>
    /// Get or set a language key (fr, en, ...)
    /// </summary>
    public string LanguageKey { get; set; }

    /// <summary>
    /// Get or set the path to the file which contain EULA text
    /// </summary>
    public string FilePath { get; set; }

}
