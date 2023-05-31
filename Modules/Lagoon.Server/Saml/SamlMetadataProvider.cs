using System.Collections.Concurrent;

namespace Lagoon.Server.Saml;


internal class SamlMetadataProvider
{

    #region constants

    /// <summary>
    /// The name of the HTTP client.
    /// </summary>
    public const string HTTP_CLIENT_NAME = "LagoonSAML2";

    #endregion

    #region fields

    /// <summary>
    /// The HTTP client factory.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// The IdP metadata cache.
    /// </summary>
    private readonly ConcurrentDictionary<string, IdpMetadata> _metadata = new();

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public SamlMetadataProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    #endregion

    #region methods

    /// <summary>
    /// Get the identity provider metadata.
    /// </summary>
    /// <param name="location">The location of the identity provider metadata.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IdpMetadata> GetIdpMetadataAsync(string location)
    {
        if (!_metadata.TryGetValue(HTTP_CLIENT_NAME, out IdpMetadata metadata))
        {
            Uri uri = new(location);
            string content;
            if (uri.IsFile)
            {
                content = await File.ReadAllTextAsync(uri.LocalPath);
            }
            else
            {
                HttpRequestMessage request = new(HttpMethod.Get, location)
                {
                    Headers = { { "Accept", "*/*" } }
                };
                HttpClient client = _httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
                HttpResponseMessage response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"We can't download the identity provider metadata from \"{location}\".");
                }
                content = await response.Content.ReadAsStringAsync();
            }
            metadata = IdpMetadata.FromString(content);
            _metadata.TryAdd(location, metadata);
        }
        return metadata;
    }

    #endregion

}
