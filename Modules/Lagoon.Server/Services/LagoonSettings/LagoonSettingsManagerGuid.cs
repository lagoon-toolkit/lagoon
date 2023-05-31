using Lagoon.Model.Context;
using Lagoon.Model.Models;

namespace Lagoon.Server.Services.LagoonSettings;


/// <summary>
/// Lagoon settings manager with generic type for user id (Guid, int, ...).
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public class LagoonSettingsManagerGuid<TUser> : LagoonSettingsManagerBase<LagoonSettingsGuid<TUser>, TUser, Guid>
    where TUser : ILgIdentityUser
{

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="db">The database context.</param>
    public LagoonSettingsManagerGuid(ILgApplicationDbContext db)
        : base(db)
    { }

    ///<inheritdoc/>
    protected override void SetSettingUserId(LagoonSettingsGuid<TUser> setting, string userId)
    {
        setting.UserId = ParseUserId(userId);
    }

    ///<inheritdoc/>
    protected override Func<LagoonSettingsGuid<TUser>, bool> GetEqualCondition(string userId, SettingParam param, string subParam)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return x => x.Param == param && x.UserId == null && x.SubParam == subParam;
        }
        else
        {
            Guid? key = ParseUserId(userId);
            return x => x.Param == param && x.UserId == key && x.SubParam == subParam;
        }
    }

    ///<inheritdoc/>
    protected override Func<LagoonSettingsGuid<TUser>, bool> GetContainsCondition(string userId, SettingParam param, string subParam, bool includeShared)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return includeShared
                ? x => x.Param == param && x.UserId == null && x.SubParam == subParam
                : x => false;
        }
        else
        {
            Guid? key = ParseUserId(userId);
            return includeShared
                ? x => x.Param == param && (x.UserId == null || x.UserId == key) && x.SubParam.Contains(subParam)
                : x => x.Param == param && x.UserId == key && x.SubParam.Contains(subParam);
        }
    }

    /// <summary>
    /// Convert the string to right db type.
    /// </summary>
    /// <param name="userId">The user Id.</param>
    /// <returns>The converted value.</returns>
    private static Guid? ParseUserId(string userId)
    {
        return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
    }

    ///<inheritdoc/>
    protected override IEnumerable<string> SelectUsersDistinct(IQueryable<LagoonSettingsGuid<TUser>> settings)
    {
        return settings.Select(f => f.UserId).Distinct().Select(u => u.ToString());
    }

}
