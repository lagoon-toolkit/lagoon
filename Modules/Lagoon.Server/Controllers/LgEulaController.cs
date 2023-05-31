using Lagoon.Shared.Model;
using Lagoon.Shared;
using Lagoon.Internal;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Controller used to send EULA data.
/// Not intended to be accessed directly. To add/update eula use ILgEulaManager service
/// </summary>
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route(Routes.EULA_ROUTE)]
[AllowAnonymous]
public class LgEulaController : LgControllerBase
{

    /// <summary>
    /// LgEulaManager interface
    /// </summary>
    private readonly ILgEulaManager _eulaManager;


    /// <summary>
    /// Initialisation
    /// </summary>
    /// <param name="eulaManager"></param>
    public LgEulaController(ILgEulaManager eulaManager)
    {
        _eulaManager = eulaManager;
    }

    /// <summary>
    /// Return the list of configured eula (or a single entry with the last date update if there is no version change)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get(string version)
    {
        try
        {
            if (_eulaManager.GetEulaVersion() != version)
            {
                var x = _eulaManager.GetAllEula();
                return Ok(x);
            }
            else
            {
                return Ok(new[] { new Eula() { Id = Eula.VersionKey, Value = version } });
            }
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Get last version
    /// </summary>                     
    /// <returns></returns>
    [HttpGet(Routes.EULA_VERSION)]
    [AllowAnonymous]
    public IActionResult LastVersion()
    {
        try
        {
            return Ok(_eulaManager.GetEulaVersion());
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Add or update eula
    /// </summary>        
    /// <param name="eula">Eula content to add or update</param>        
    /// <param name="updateVersion">Indicate if the version number must be increase</param>
    /// <returns></returns>
    [HttpPost(Routes.EULA_UPDATE)]
    [Authorize(Policy = Policies.EulaEditor)]
    public async Task<IActionResult> SetAsync(Eula eula, bool updateVersion = true)
    {
        try
        {
            await _eulaManager.SetEula(eula.Id, eula.Value, updateVersion);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Update eula version
    /// </summary>                     
    /// <returns></returns>
    [HttpGet(Routes.EULA_VERSION_UPDATE)]
    [Authorize(Policy = Policies.EulaEditor)]
    public async Task<IActionResult> SetVersionAsync()
    {
        try
        {
            await _eulaManager.ForceEulaRevalidation();
            return Ok(_eulaManager.GetEulaVersion());
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

}
