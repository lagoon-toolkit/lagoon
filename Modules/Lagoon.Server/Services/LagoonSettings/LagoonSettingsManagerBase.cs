using Lagoon.Model.Context;
using Lagoon.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Lagoon.Server.Services.LagoonSettings;


/// <summary>
/// Lagoon settings manager.
/// </summary>
/// <typeparam name="TLagoonSettings">The LagoonSettings entity.</typeparam>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TKey">The type of the foreign key for users.</typeparam>
public abstract class LagoonSettingsManagerBase<TLagoonSettings, TUser, TKey> : ILagoonSettingsManager
    where TLagoonSettings : Model.Models.LagoonSettingsBase, new()
    where TUser : ILgIdentityUser
    where TKey : IEquatable<TKey>
{
    #region fields

    // Application DB context (user ui params stored in DB)
    private DbContext _db;
    private DbSet<TLagoonSettings> _dbSet;

    #endregion

    #region initialisation

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="db">The database context.</param>
    public LagoonSettingsManagerBase(ILgApplicationDbContext db)
    {
        _db = (DbContext)db;
        _dbSet = db.Set<TLagoonSettings>();
    }

    #endregion

    #region Getters

    ///<inheritdoc/>
    public TOut GetLgSetting<TOut>(string userId, SettingParam param, string subParam = null)
    {
        string json = GetJsonWhere(GetEqualCondition(userId, param, subParam)).FirstOrDefault();
        return !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<TOut>(json) : default;
    }

    ///<inheritdoc/>
    public TOut GetLgSetting<TOut>(SettingParam param, string subParam)
    {
        string data = _dbSet.Where(x => x.Param == param && x.SubParam == subParam).Select(f => f.Json).FirstOrDefault();
        return !string.IsNullOrEmpty(data) ? JsonSerializer.Deserialize<TOut>(data) : default;
    }

    ///<inheritdoc/>
    public IEnumerable<TOut> GetLgSettingFromSubParamContains<TOut>(string userId, SettingParam param, string subParam, bool includeShared = false)
    {
        foreach (string json in GetJsonWhere(GetContainsCondition(userId, param, subParam, includeShared)))
        {
            yield return JsonSerializer.Deserialize<TOut>(json);
        }
    }

    /// <summary>
    /// Return the list of user id which have a specific param and optionnal subparam
    /// </summary>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    /// <returns></returns>
    public IEnumerable<string> GetUsersBySettingParam(SettingParam param, string subParam = null)
    {
        return SelectUsersDistinct(_dbSet.Where(x => x.Param == param && (subParam == null || x.SubParam == subParam)));
    }

    /// <summary>
    /// Return a distinct user id list as string.
    /// </summary>
    /// <param name="settings">A settings query.</param>
    /// <returns>A distinct user id list as string</returns>
    protected abstract IEnumerable<string> SelectUsersDistinct(IQueryable<TLagoonSettings> settings);

    /// <summary>
    /// Return enumerable of json data
    /// </summary>
    /// <param name="predicate">The condition.</param>
    /// <returns>The Json values corresponding to the condition.</returns>
    private IEnumerable<string> GetJsonWhere(Func<TLagoonSettings, bool> predicate)
    {
        return _dbSet.Where(predicate).Select(f => f.Json);
    }

    /// <summary>
    /// Get the condition to get the setting entry.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="param">The parameter ID.</param>
    /// <param name="subParam">The parameter sub key.</param>
    /// <returns></returns>
    protected abstract Func<TLagoonSettings, bool> GetEqualCondition(string userId, SettingParam param, string subParam);

    /// <summary>
    /// Get the condition to get setting entries.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="param">The parameter ID.</param>
    /// <param name="subParam">The parameter sub key.</param>
    /// <param name="includeShared">Include value where user is null.</param>
    /// <returns>The filter function.</returns>
    protected abstract Func<TLagoonSettings, bool> GetContainsCondition(string userId, SettingParam param, string subParam, bool includeShared);

    #endregion

    #region Setters

    ///<inheritdoc/>
    public void AddLgSetting<TIn>(TIn model, string userId, SettingParam param, string subParam = null)
    {
        string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
        TLagoonSettings setting = new()
        {
            Param = param,
            SubParam = subParam,
            Json = json
        };
        SetSettingUserId(setting, userId);
        _dbSet.Add(setting);
        _db.SaveChanges();
    }

    /// <summary>
    /// Assign the setting to a user.
    /// </summary>
    /// <param name="setting">The setting.</param>
    /// <param name="userId">The userId.</param>
    protected abstract void SetSettingUserId(TLagoonSettings setting, string userId);

    ///<inheritdoc/>
    public void SetLgSetting<TIn>(TIn model, string userId, SettingParam param, string subParam = null)
    {
        string json = JsonSerializer.Serialize(model, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
        TLagoonSettings settings = _dbSet.Where(GetEqualCondition(userId, param, subParam)).FirstOrDefault();
        settings.Json = json;
        _dbSet.Update(settings);
        _db.SaveChanges();
    }
    #endregion

    #region Delete

    ///<inheritdoc/>
    public void DeleteLgSetting(string userId, SettingParam param, string subParam = null)
    {
        _dbSet.RemoveRange(_dbSet.Where(GetEqualCondition(userId, param, subParam)));
        _db.SaveChanges();
    }

    #endregion
}
