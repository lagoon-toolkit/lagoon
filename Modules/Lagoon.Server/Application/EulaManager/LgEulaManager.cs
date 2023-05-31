using Lagoon.Model.Context;
using Lagoon.Server.Application.EulaManager;
using Lagoon.Shared.Model;

namespace Lagoon.Server.Application;


/// <summary>
/// Service used to manage eula data
/// </summary>
public class LgEulaManager : ILgEulaManager
{

    #region fields

    // Application DB context (eula stored in DB)
    private ILgApplicationDbContext _db;

    // Last eula file modification
    internal static DateTime? _lastEulaFileModification = null;

    /// <summary>
    /// Last EULA update ticks (used to avoid requesting DB if no eula changes)
    /// </summary>
    internal static string EulaVersion = default;

    /// <summary>
    /// List of configured eula file in appsettings.json
    /// </summary>
    private List<EulaFile> _eulaFiles = new();

    /// <summary>
    /// Application logger
    /// </summary>
    private ILogger<LgEulaManager> _logger;

    /// <summary>
    /// Help to check if we are in Databse or File mode
    /// </summary>
    private EulaMode Mode => _eulaFiles.Count > 0 ? EulaMode.File : EulaMode.DataBase;

    #endregion

    #region initialisation

    /// <summary>
    /// Service used to manage EULA data
    /// </summary>
    /// <param name="db">Database context</param>
    /// <param name="config">Application configuration settings</param>
    /// <param name="logger">Application logger</param>
    public LgEulaManager(ILgApplicationDbContext db, IConfiguration config, ILogger<LgEulaManager> logger)
    {
        _db = db;
        _logger = logger;
        // Update options with the configuration if the section exists
        config.GetSection("Lagoon:EulaFiles").Bind(_eulaFiles);
    }

    #endregion

    #region E.U.L.A management

    /// <summary>
    /// Return the list of configured eula
    /// </summary>
    public List<Eula> GetAllEula()
    {
        if (Mode == EulaMode.DataBase)
        {
            // Return eulas from DB
            return _db.Eulas.ToList();
        }
        else
        {
            // Build eula result with eula files content
            List<Eula> _eulas = new();
            Eula eulaVersion = new() { Id = Eula.VersionKey };
            _eulas.Add(eulaVersion);
            long lastVersionNumber = 0;
            foreach (EulaFile o in _eulaFiles)
            {
                if (System.IO.File.Exists(o.FilePath))
                {
                    string eulaContent = System.IO.File.ReadAllText(o.FilePath);
                    _eulas.Add(new Eula()
                    {
                        Id = o.LanguageKey,
                        Value = eulaContent
                    });
                    long fileTick = new System.IO.FileInfo(o.FilePath).LastWriteTime.Ticks;
                    lastVersionNumber = lastVersionNumber < fileTick ? fileTick : lastVersionNumber;
                }
            }
            eulaVersion.Value = lastVersionNumber.ToString();
            return _eulas;
        }
    }

    /// <summary>
    /// Create or update the EULA text for the given language key
    /// </summary>
    /// <param name="languageKey">Language key to create or update</param>
    /// <param name="eula">EULA text</param>
    /// <param name="updateVersion"><c>true</c> by default, users must revalidate eula. if <c>false</c> eula last modification date will not be updated</param>
    public Task SetEula(string languageKey, string eula, bool updateVersion = true)
    {
        // Eula modification is only available in server mode
        EnsureDbMode(nameof(SetEula));
        Eula eulaDB = _db.Eulas.Where(x => x.Id == languageKey).FirstOrDefault();
        if (eulaDB == null)
        {
            // Create new eula entry
            _db.Eulas.Add(new Eula()
            {
                Id = languageKey,
                Value = eula
            });
        }
        else
        {
            // Update existing eula entry
            eulaDB.Value = eula;
        }
        if (updateVersion)
        {
            // Update the EULA modification date 
            Eula eulaVersionDB = _db.Eulas.Where(x => x.Id == Eula.VersionKey).FirstOrDefault();
            if (eulaVersionDB is null)
            {
                // create a new entry
                eulaVersionDB = new Eula
                {
                    Id = Eula.VersionKey
                };
                _db.Eulas.Add(eulaVersionDB);
            }
            // update an existing entry
            eulaVersionDB.Value = DateTime.Now.Ticks.ToString();
            EulaVersion = eulaVersionDB.Value;
        }
        // Save changes
        return _db.SaveChangesAsync();
    }

    /// <summary>
    /// Force user to re-validate eula
    /// </summary>
    public async Task ForceEulaRevalidation()
    {
        // Force eula revalidation is only available in server mode
        EnsureDbMode(nameof(ForceEulaRevalidation));
        // Update the EULA modification date 
        Eula eulaVersion = _db.Eulas.Where(x => x.Id == Eula.VersionKey).FirstOrDefault();
        if (eulaVersion == null)
        {
            eulaVersion = new Eula
            {
                Id = Eula.VersionKey,
                Value = DateTime.Now.Ticks.ToString()
            };
            _db.Eulas.Add(eulaVersion);
        }
        else
        {
            eulaVersion.Value = DateTime.Now.Ticks.ToString();
        }
        // Save changes
        await _db.SaveChangesAsync();
        EulaVersion = eulaVersion.Value;
    }

    /// <summary>
    /// Return the last update date for eula
    /// </summary>
    public string GetEulaVersion()
    {
        if (EulaVersion == null && Mode == EulaMode.DataBase)
        {
            // DB Mode
            Eula eulaDateDB = _db.Eulas.Where(x => x.Id == Eula.VersionKey).FirstOrDefault();
            if (eulaDateDB != null)
            {
                EulaVersion = eulaDateDB.Value;
            }
        }
        else if (Mode == EulaMode.File)
        {
            // File Mode
            long version = 0;
            foreach (EulaFile o in _eulaFiles)
            {
                if (System.IO.File.Exists(o.FilePath))
                {
                    long ticks = new System.IO.FileInfo(o.FilePath).LastWriteTime.Ticks;
                    if (ticks > version)
                    {
                        version = ticks;
                    }
                }
                EulaVersion = version.ToString();
            }
        }
        return EulaVersion;
    }

    /// <summary>
    /// Throw an exception if we are in db mode (no eula file configured)
    /// </summary>
    /// <param name="caller">Caller name</param>
    private void EnsureDbMode(string caller)
    {
        if (_eulaFiles.Count > 0)
        {
            throw new Exception($"LgEulaManager - '{caller}' is not available for EULA files mode");
        }
    }

    #endregion

    private enum EulaMode
    {
        File = 1,
        DataBase = 2
    }

}
