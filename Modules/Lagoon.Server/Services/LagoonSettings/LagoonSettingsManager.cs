using Lagoon.Model.Context;
using Lagoon.Model.Models;

namespace Lagoon.Server.Services.LagoonSettings;


/// <summary>
/// Lagoon settings manager with user id as string.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
public class LagoonSettingsManager<TUser> : LagoonSettingsManagerBase<LagoonSettings<TUser>, TUser, string>
    where TUser : ILgIdentityUser
{

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="db">The database context.</param>
    public LagoonSettingsManager(ILgApplicationDbContext db)
        : base(db)
    { }

    ///<inheritdoc/>
    protected override void SetSettingUserId(LagoonSettings<TUser> setting, string userId)
    {
        setting.UserId = userId;
    }

    ///<inheritdoc/>
    protected override Func<LagoonSettings<TUser>, bool> GetEqualCondition(string userId, SettingParam param, string subParam)
    {
        if (userId == default)
        {
            userId = null;
        }
        return x => x.Param == param && x.UserId == userId && x.SubParam == subParam;
    }

    ///<inheritdoc/>
    protected override Func<LagoonSettings<TUser>, bool> GetContainsCondition(string userId, SettingParam param, string subParam, bool includeShared)
    {
        return string.IsNullOrEmpty(userId)
            ? includeShared
                ? x => x.Param == param && x.UserId == null && x.SubParam == subParam
                : x => false
            : includeShared
                ? x => x.Param == param && (x.UserId == null || x.UserId == userId) && x.SubParam.Contains(subParam)
                : x => x.Param == param && x.UserId == userId && x.SubParam.Contains(subParam);
    }

    ///<inheritdoc/>
    protected override IEnumerable<string> SelectUsersDistinct(IQueryable<LagoonSettings<TUser>> settings)
    {
        return settings.Select(f => f.UserId).Distinct();
    }

}
