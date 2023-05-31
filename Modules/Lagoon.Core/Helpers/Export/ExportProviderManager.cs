using System.Collections;

namespace Lagoon.Helpers;

/// <summary>
/// Classe to manage export provider collection.
/// </summary>
public class ExportProviderManager : IEnumerable<IExportProvider>
{
    #region fields

    private readonly Dictionary<string, IExportProvider> _providers;

    #endregion

    #region constructors

    /// <summary>
    /// Instanciate a new export provider manager.
    /// </summary>
    public ExportProviderManager()
    {
        _providers = new Dictionary<string, IExportProvider>();
    }

    /// <summary>
    /// Instanciate a new export provider manager.
    /// </summary>
    /// <param name="exportProviderManager">The source export provider.</param>
    public ExportProviderManager(ExportProviderManager exportProviderManager)
    {
        _providers = new Dictionary<string, IExportProvider>(exportProviderManager._providers);
    }

    /// <summary>
    /// Instanciate a new export provider manager with the specified providers.
    /// </summary>
    public ExportProviderManager(params IExportProvider[] providers) : this()
    {
        foreach (IExportProvider provider in providers)
        {
            Register(provider);
        }
    }

    #endregion

    #region properties

    /// <summary>
    /// Get the number of export providers.
    /// </summary>
    public int Count => _providers.Count;

    #endregion

    #region methods

    /// <summary>
    /// Clear the providers collection.
    /// </summary>
    public void Clear()
    {
        _providers.Clear();
    }

    /// <summary>
    /// Add or replace an export provider. The provider id is unique in the collection.
    /// </summary>
    public void Register<TExportProvider>() where TExportProvider : IExportProvider, new()
    {
        Register(new TExportProvider());
    }

    /// <summary>
    /// Add or replace an export provider. The provider id is unique in the collection.
    /// </summary>
    /// <param name="provider">The new provider.</param>
    public void Register(IExportProvider provider)
    {
        string id = provider.Id.ToLowerInvariant();
        if (!_providers.TryAdd(id, provider))
        {
            _providers[id] = provider;
        }
    }

    /// <summary>
    /// Remove the provider id.
    /// </summary>
    /// <param name="providerIdList">Provider Id list separated by comma.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public void Remove(string providerIdList)
    {
        if (providerIdList is not null)
        {
            foreach (string providerId in providerIdList.ToLowerInvariant().Split('.'))
            {
                _providers.Remove(providerId.Trim());
            }
        }
    }

    /// <summary>
    /// Return the registred export provider corresponding to the the requested id.
    /// </summary>
    /// <param name="providerId">The provider Id.</param>
    /// <returns></returns>
    public IExportProvider GetProvider(string providerId)
    {
        return _providers[providerId.ToLowerInvariant()];
    }

    ///<inheritdoc/>
    public IEnumerator<IExportProvider> GetEnumerator()
    {
        return _providers.Values.GetEnumerator();
    }

    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

}
