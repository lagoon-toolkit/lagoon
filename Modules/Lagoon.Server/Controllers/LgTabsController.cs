using Lagoon.Internal;
using Lagoon.Server.Services.LagoonSettings;

namespace Lagoon.Server.Controllers;


/// <summary>
/// Controller used to send EULA data.
/// Not intended to be accessed directly. To add/update eula use ILgEulaManager service
/// </summary>
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route(Routes.TABS_ROUTE)]
[Authorize]
public class LgTabsController : LgControllerBase
{
    /// <summary>
    /// LagoonSettingsManager interface
    /// </summary>
    private readonly ILagoonSettingsManager _lgSettingsManager;

    /// <summary>
    /// Initialisation
    /// </summary>
    /// <param name="lgSettingsManager">LagoonSettingsManager interface</param>
    public LgTabsController(ILagoonSettingsManager lgSettingsManager)
    {
        _lgSettingsManager = lgSettingsManager;
    }

    /// <summary>
    /// Return the list of saved tabs uri for the connected user
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        try
        {
            List<Tab> tabs = _lgSettingsManager.GetLgSetting<List<Tab>>(ContextUserId.ToString(), SettingParam.Tab);
            return Ok(tabs);
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Save the list of tabs uri for the connected user
    /// </summary>
    /// <param name="tabs"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post(IEnumerable<Tab> tabs)
    {
        try
        {
            IEnumerable<Tab> existingTabs = _lgSettingsManager.GetLgSetting<IEnumerable<Tab>>(ContextUserId.ToString(), SettingParam.Tab);
            if (existingTabs is not null)
            {
                // Remove existing list
                _lgSettingsManager.DeleteLgSetting(ContextUserId.ToString(), SettingParam.Tab);
            }
            // Init list of tab for the user
            _lgSettingsManager.AddLgSetting<IEnumerable<Tab>>(tabs, ContextUserId.ToString(), SettingParam.Tab);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Save the list of tabs uri for the connected user
    /// </summary>
    /// <param name="tabs"></param>
    /// <returns></returns>
    [HttpDelete]
    [AllowAnonymous]
    public IActionResult Delete(IEnumerable<Tab> tabs)
    {
        try
        {
            IEnumerable<Tab> existingTabs = _lgSettingsManager.GetLgSetting<IEnumerable<Tab>>(ContextUserId.ToString(), SettingParam.Tab);
            if (existingTabs is not null)
            {
                // Remove existing list
                _lgSettingsManager.DeleteLgSetting(ContextUserId.ToString(), SettingParam.Tab);
            }
            // Init list of tab for the user
            _lgSettingsManager.AddLgSetting<IEnumerable<Tab>>(tabs, ContextUserId.ToString(), SettingParam.Tab);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

}
