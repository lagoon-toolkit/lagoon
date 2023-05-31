using System.DirectoryServices.AccountManagement;

namespace Lagoon.Services;


[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class ActiveDirectoryService
{
    #region Fields 

    private readonly PrincipalContext _principalContext;
    private readonly ILogger<ActiveDirectoryService> _logger;

    #endregion Fields 

    public ActiveDirectoryService(string activeDirectoryName, ILogger<ActiveDirectoryService> logger)
    {
        _principalContext = new PrincipalContext(ContextType.Domain, activeDirectoryName);
        _logger = logger;
    }

    #region Public Methods

    /// <summary>
    /// Get AD groups for user
    /// </summary>
    /// <param name="user">user</param>
    /// <returns></returns>
    public Task<List<string>> GetAdGroupsForUserAsync(string user)
    {
        return GetUserAdGroups(user);
    }

    /// <summary>
    /// return true if user is in the AD group
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public async Task<bool> IsUserInGroupAsync(string userName, string group)
    {
        var groups = await GetUserAdGroups(userName);
        return groups.Contains(group);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Get AD groups for user
    /// </summary>
    /// <param name="user">user</param>
    /// <returns></returns>
    private Task<List<string>> GetUserAdGroups(string userName)
    {
        return Task.Run(() =>
        {
            List<string> result = new();
            UserPrincipal user = null;
            user = UserPrincipal.FindByIdentity(_principalContext, userName);
            if (user == null)
                return result;

            try
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
                foreach (var g in groups)
                {
                    result.Add(g.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(0, ex.GetBaseException(), "Exception occurred.");
            }
            return result;
        });
    }


    #endregion Private Methods
}
