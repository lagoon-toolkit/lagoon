using Lagoon.Server.Services.LagoonSettings;
using System.Web;

namespace Lagoon.Server.Controllers;

/// <summary>
/// GridView profile controller.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
[Route("[controller]")]
public class LgGridViewProfileController : LgControllerBase
{

    #region constants

    private const SettingParam PROFILE_SETTING = SettingParam.GridViewProfile;

    #endregion

    #region fields

    /// <summary>
    /// LagoonSettingsManager interface
    /// </summary>
    private readonly ILagoonSettingsManager _lgSettingsManager;

    #endregion

    #region constructor

    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="lgSettingsManager">Settings manager.</param>
    public LgGridViewProfileController(ILagoonSettingsManager lgSettingsManager)
    {
        _lgSettingsManager = lgSettingsManager;
    }

    #endregion

    #region methods

    /// <summary>
    /// Return the shared profiles of the given gridview
    /// </summary>
    /// <param name="gridViewId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("List/Shared/{gridViewId}")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<ProfileItem>> ListGridViewSharedProfiles(string gridViewId)
    {
        try
        {
            return Ok(GetProfileItemList(null, gridViewId).ToList());
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Return the user profiles of the given gridview
    /// </summary>
    /// <param name="gridViewId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("List/User/{gridViewId}")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<ProfileItem>> ListGridViewProfiles(string gridViewId)
    {
        try
        {
            return Ok(GetProfileItemList(ContextUserId.ToString(), gridViewId).ToList());
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Return user and shared profiles of the given gridview
    /// </summary>
    /// <param name="gridViewId">The GridView Id.</param>
    /// <returns></returns>
    [HttpGet]
    [Route("List/All/{gridViewId}")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<ProfileItem>> ListGridViewAllProfiles(string gridViewId)
    {
        try
        {
            // rq: .ToList() called since MessagePack does not support serialization of type 'SelectIPartitionIterator<GridViewProfile, ProfileItem>' 
            return Ok(GetProfileItemList(ContextUserId.ToString(), gridViewId, true).ToList());
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Return the profile item list
    /// </summary>
    /// <param name="user"></param>
    /// <param name="stateId"></param>
    /// <param name="includeShared"></param>
    /// <returns></returns>
    private IEnumerable<ProfileItem> GetProfileItemList(string user, string stateId, bool includeShared = false)
    {
        IEnumerable<GridViewProfile> profilesList =
            _lgSettingsManager.GetLgSettingFromSubParamContains<GridViewProfile>(user, PROFILE_SETTING, $"gridview-{stateId}-", includeShared);
        return profilesList.OrderBy(p => p.Id)
            .Select(x => new ProfileItem { Id = x.Id, Label = x.Label, IsSharedProfile = x.IsSharedProfile });
    }

    /// <summary>
    /// Get the profile with the given id.
    /// </summary>
    /// <param name="id">Profile id</param>
    /// <param name="shared">Shared profile ?</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Profile/{id}/{shared}")]
    [AllowAnonymous]
    public ActionResult<GridViewProfile> GetGridViewProfile(string id, bool shared)
    {
        try
        {
            string profileKey = GetProfileSubParamKey(HttpUtility.UrlDecode(id));
            string userId = shared ? null : ContextUserId.ToString();
            return Ok(_lgSettingsManager.GetLgSetting<GridViewProfile>(userId, PROFILE_SETTING, profileKey));
        }
        catch (System.Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Save profile
    /// </summary>
    /// <param name="gridViewProfile">Profile to save</param>
    [HttpPost]
    public void SaveGridViewProfile([FromBody] GridViewProfile gridViewProfile)
    {
        if (!string.IsNullOrEmpty(gridViewProfile?.Id))
        {
            string userId = gridViewProfile.IsSharedProfile ? null : ContextUserId.ToString();
            string profileKey = GetProfileSubParamKey(gridViewProfile.Id);
            GridViewProfile profile = _lgSettingsManager.GetLgSetting<GridViewProfile>(userId, PROFILE_SETTING, profileKey);
            if (profile is null)
            {
                _lgSettingsManager.AddLgSetting(gridViewProfile, userId, PROFILE_SETTING, profileKey);
            }
            else
            {
                _lgSettingsManager.SetLgSetting(gridViewProfile, userId, PROFILE_SETTING, profileKey);
            }
        }
    }

    /// <summary>
    /// Delete profile
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteGridViewProfile(string id)
    {
        try
        {
            string profileKey = GetProfileSubParamKey(HttpUtility.UrlDecode(id));
            string contextUserId = ContextUserId.ToString();
            GridViewProfile profile = _lgSettingsManager.GetLgSetting<GridViewProfile>(contextUserId, PROFILE_SETTING, profileKey);                
            profile ??= _lgSettingsManager.GetLgSetting<GridViewProfile>((string)null, PROFILE_SETTING, profileKey);
            string userId = profile.IsSharedProfile ? null : contextUserId;
            if (profile is not null)
            {
                _lgSettingsManager.DeleteLgSetting(userId, PROFILE_SETTING, profileKey);
                return Ok();
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(ex);
        }
    }

    /// <summary>
    /// Get the sub param key for a profile.
    /// </summary>
    /// <param name="profileId">The profile id.</param>
    /// <returns>The sub param key for a profile.</returns>
    private static string GetProfileSubParamKey(string profileId)
    {
        // Compatibility with the old id format. Ex: "https://localhost:44396/gridview-GridViewTest4-profile-2"
        // The new format is "gridview-{ProfileId}", ProfileId -> "{StateId}-{Index}"
        return profileId.Contains("/gridview-") ? profileId : $"gridview-{profileId}";
    }

    #endregion

}
