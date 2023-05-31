namespace Lagoon.Server.Services.LagoonSettings;

/// <summary>
///  Lagoon settings manager
/// </summary>
public interface ILagoonSettingsManager
{

    /// <summary>
    /// Get lagoon settings from parameters
    /// </summary>
    /// <typeparam name="TOut">Return type</typeparam>
    /// <param name="userId">User Id</param>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    /// <returns></returns>
    TOut GetLgSetting<TOut>(string userId, SettingParam param, string subParam = null);

    /// <summary>
    /// Get lagoon settings from parameters
    /// </summary>
    /// <typeparam name="TOut">Return type</typeparam>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    /// <returns></returns>
    TOut GetLgSetting<TOut>(SettingParam param, string subParam);

    /// <summary>
    /// Get lagoon settings list from parameters with contains on sub parameter
    /// </summary>
    /// <typeparam name="TOut">Return type</typeparam>
    /// <param name="userId">User Id</param>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    /// <param name="includeShared">Include value where user is null.</param>
    /// <returns>The settings.</returns>
    IEnumerable<TOut> GetLgSettingFromSubParamContains<TOut>(string userId, SettingParam param, string subParam, bool includeShared = false);

    /// <summary>
    /// Return the list of user id which have a specific param and optionnal subparam
    /// </summary>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    /// <returns></returns>
    IEnumerable<string> GetUsersBySettingParam(SettingParam param, string subParam = null);

    /// <summary>
    /// Save new lagoon setting
    /// </summary>
    /// <typeparam name="TIn">Type</typeparam>
    /// <param name="model">Model to parse as Json</param>
    /// <param name="userId">User Id</param>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    void AddLgSetting<TIn>(TIn model, string userId, SettingParam param, string subParam = null);

    /// <summary>
    /// Update lagoon setting
    /// </summary>
    /// <typeparam name="TIn">Type</typeparam>
    /// <param name="model">Model to parse as Json</param>
    /// <param name="userId">User Id</param>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    void SetLgSetting<TIn>(TIn model, string userId, SettingParam param, string subParam = null);

    /// <summary>
    /// Delete lagoon setting
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="param">Parameter</param>
    /// <param name="subParam">Sub parameter</param>
    void DeleteLgSetting(string userId, SettingParam param, string subParam = null);

}
