using MessagePack;
using Microsoft.JSInterop.WebAssembly;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace System.Net.Http;


/// <summary>
/// Extensions methods
/// </summary>
public static class LagoonExtensions
{

    #region constants

    /// <summary>
    /// Name of the authenticated http client.
    /// </summary>
    internal const string AUTHENTICATED_HTTP_CLIENT_NAME = "Authenticated";
    internal const string ANONYMOUS_HTTP_CLIENT_NAME = "Anonymous";
    internal const string JSON_AUTHENTICATED_HTTP_CLIENT_NAME = "JsonAuthenticated";

    #endregion

    #region fields

    /// <summary>
    /// MessagePack option used in Lagoon
    /// </summary> 
    private static MessagePackSerializerOptions _lagoonMessagePackSerializerOptions =
        new MessagePackSerializerOptions(MessagePack.Resolvers.TypelessContractlessStandardResolver.Instance)
            .WithCompression(MessagePackCompression.Lz4BlockArray)
            .WithAllowAssemblyVersionMismatch(true)
            .WithOmitAssemblyVersion(true);

    #endregion

    #region HttpClient extensions (Authenticated / Annonymous)

    /// <summary>
    /// Create anonymous http client that don't check the authorization of resources.
    /// </summary>
    /// <param name="httpClientFactory">Http client client factory instance.</param>
    /// <returns>A new anonymous http client that don't check the authorization of resources.</returns>
    public static HttpClient CreateAnonymousClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(ANONYMOUS_HTTP_CLIENT_NAME);
    }

    /// <summary>
    /// Create authentificated http client that check the authorization of resources.
    /// </summary>
    /// <param name="httpClientFactory">Http client client factory instance.</param>
    /// <returns>A new authentificated http client that check the authorization of resources.</returns>
    public static HttpClient CreateAuthenticatedClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(AUTHENTICATED_HTTP_CLIENT_NAME);
    }

    /// <summary>
    /// Create authentificated json http client that check the authorization of resources.
    /// </summary>
    /// <param name="httpClientFactory">Http client client factory instance.</param>
    /// <returns>A new authentificated http client that check the authorization of resources.</returns>
    public static HttpClient CreateJsonAuthenticatedClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(JSON_AUTHENTICATED_HTTP_CLIENT_NAME);
    }

    #endregion

    #region HttpClient extensions (TryGetAsync / TryPostAsync / TryPutAsync / TryPatchAsync / TryDeleteAsync)

    /// <summary>
    /// Retrieve data from controlleur
    /// </summary>
    /// <typeparam name="T">Expected type result</typeparam>
    /// <param name="http">HttpClient instance</param>
    /// <param name="requestUri">Api Uri to call</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The deserialized api response</returns>
    /// <exception cref="UserException">The (potential) user exception throw by the api call</exception>
    /// <exception cref="Exception">The (potential) raw exception throw by the api call</exception>
    public static async Task<T> TryGetAsync<T>(this HttpClient http, string requestUri, CancellationToken cancellationToken = default)
    {
        // Call the request URI
        HttpResponseMessage httpResponse = await http.GetAsync(requestUri, cancellationToken);
        // Return the deserialized response
        return await httpResponse.DeserializeAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Retrieve data from controlleur
    /// </summary>
    /// <typeparam name="T">Expected type result</typeparam>
    /// <param name="http">HttpClient instance</param>
    /// <param name="requestUri">Api Uri to call</param>
    /// <param name="completionOption">Indicates if System.Net.Http.HttpClient operations should be considered completed either as soon as a response is available, or after reading the entire response message including the content.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The deserialized api response</returns>
    /// <exception cref="UserException">The (potential) user exception throw by the api call</exception>
    /// <exception cref="Exception">The (potential) raw exception throw by the api call</exception>
    public static async Task<T> TryGetAsync<T>(this HttpClient http, string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
    {
        // Call the request URI
        HttpResponseMessage httpResponse = await http.GetAsync(requestUri, completionOption, cancellationToken);
        // Return the deserialized response
        return await httpResponse.DeserializeAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Post TIn data to request URI, read response and
    ///     - return the TOut object
    ///     - return null and display ModelState error if present
    /// </summary>
    /// <typeparam name="TIn">Object type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestUri">Uri to post data to</param>
    /// <param name="model">Object to post</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    public static async Task TryPostAsync<TIn>(this HttpClient http, string requestUri, TIn model, CancellationToken cancellationToken = default)
    {
        // Post model data to resquestUri
        HttpResponseMessage response = await http.PostAsJsonAsync(requestUri, model, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // Read response content and try to deserialize the error
            await response.DeserializeAsync<object>(cancellationToken);
        }
    }

    /// <summary>
    /// Post TIn data to request URI, read response and
    ///     - return the TOut object
    ///     - return null and display ModelState error if present
    /// </summary>
    /// <typeparam name="TIn">POST Object type</typeparam>
    /// <typeparam name="TOut">Response Object type</typeparam>
    /// <param name="http">HttpClient extension method</param>
    /// <param name="requestUri">Uri to post data to</param>
    /// <param name="model">Object to post</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>Response (TOut) object if posted successfully handled, null if errors</returns>
    public static async Task<TOut> TryPostAsync<TIn, TOut>(this HttpClient http, string requestUri, TIn model, CancellationToken cancellationToken = default)
    {
        // Post model data to resquestUri
        HttpResponseMessage response = await http.PostAsJsonAsync(requestUri, model, cancellationToken);
        return await response.DeserializeAsync<TOut>(cancellationToken);
    }

    /// <summary>
    /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.
    /// </summary>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task TryDeleteAsync(this HttpClient http, string requestURI, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await http.DeleteAsync(requestURI, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // Read response content and try to deserialize the error
            await response.DeserializeAsync<object>(cancellationToken);
        }
    }

    /// <summary>
    /// Send a DELETE request to the specified Uri with a body and an optionnal cancellation token as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TIn">Object type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="model">Object data to send</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task TryDeleteAsync<TIn>(this HttpClient http, string requestURI, TIn model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        string content = JsonSerializer.Serialize(model);
        StringContent httpContent = new(content, Encoding.UTF8, "application/json");
        // Build the request
        HttpRequestMessage request = new()
        {
            Content = httpContent,
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestURI, UriKind.RelativeOrAbsolute)
        };
        // Send the request
        response = await http.SendAsync(request, cancellationToken);
        // Check response
        if (!response.IsSuccessStatusCode)
        {
            // Read response content and try to deserialize the error
            string strResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            ThrowParsedError(response.StatusCode, strResponse);
        }
    }

    /// <summary>
    /// Send a DELETE request to the specified Uri with an optionnal cancellation token as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TOut">Object response type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task<TOut> TryDeleteAsync<TOut>(this HttpClient http, string requestURI, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        // Build the request
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestURI, UriKind.RelativeOrAbsolute)
        };
        // Send the request
        response = await http.SendAsync(request, cancellationToken);
        return await response.DeserializeAsync<TOut>(cancellationToken);
    }

    /// <summary>
    /// Send a DELETE request to the specified Uri with an optionnal cancellation token as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TIn">Object content type</typeparam>
    /// <typeparam name="TOut">Object response type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="model">Object to post.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task<TOut> TryDeleteAsync<TIn, TOut>(this HttpClient http, string requestURI, TIn model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        string content = JsonSerializer.Serialize(model);
        StringContent httpContent = new(content, Encoding.UTF8, "application/json");
        // Build the request
        HttpRequestMessage request = new()
        {
            Content = httpContent,
            Method = HttpMethod.Delete,
            RequestUri = new Uri(requestURI, UriKind.RelativeOrAbsolute)
        };
        // Send the request
        response = await http.SendAsync(request, cancellationToken);
        return await response.DeserializeAsync<TOut>(cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request as an asynchronous operation to the specified Uri with the given value serialized as JSON.
    /// </summary>
    /// <typeparam name="TIn">PUT Object type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="model">Object to post with the PUT request</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task TryPutAsync<TIn>(this HttpClient http, string requestURI, TIn model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync(requestURI, model, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // Read response content and try to deserialize the error
            await response.DeserializeAsync<object>(cancellationToken);
        }
    }

    /// <summary>
    /// Sends a PUT request as an asynchronous operation to the specified Uri with the given value serialized as JSON.
    /// </summary>
    /// <typeparam name="TIn">PUT Object type</typeparam>
    /// <typeparam name="TOut">Response Object type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="model">Object to post with the PUT request</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task<TOut> TryPutAsync<TIn, TOut>(this HttpClient http, string requestURI, TIn model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await http.PutAsJsonAsync(requestURI, model, cancellationToken);
        return await response.DeserializeAsync<TOut>(cancellationToken);
    }

    /// <summary>
    /// Sends a PATCH request as an asynchronous operation to the specified Uri with the given value serialized as JSON.
    /// </summary>
    /// <typeparam name="TIn">PATCH Object type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="model">Object to post with the PUT request</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task TryPatchAsync<TIn>(this HttpClient http, string requestURI, TIn model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        string content = JsonSerializer.Serialize(model);
        StringContent httpContent = new(content, Encoding.UTF8, "application/json");
        response = await http.PatchAsync(requestURI, httpContent, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // Read response content and try to deserialize the error
            await response.DeserializeAsync<object>(cancellationToken);
        }
    }

    /// <summary>
    /// Sends a PATCH request as an asynchronous operation to the specified Uri with the given value serialized as JSON.
    /// </summary>
    /// <typeparam name="TIn">PATCH Object type</typeparam>
    /// <typeparam name="TOut">Object response type</typeparam>
    /// <param name="http">HttpClient extension</param>
    /// <param name="requestURI">The Uri the request is sent to.</param>
    /// <param name="model">Object to post with the PUT request</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive otice of cancellation.</param>
    /// <returns> The task object representing the asynchronous operation.</returns>
    public static async Task<TOut> TryPatchAsync<TIn, TOut>(this HttpClient http, string requestURI, TIn model, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;
        string content = JsonSerializer.Serialize(model);
        // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        StringContent httpContent = new(content, Encoding.UTF8, "application/json");
        response = await http.PatchAsync(requestURI, httpContent, cancellationToken);
        return await response.DeserializeAsync<TOut>(cancellationToken);
    }

    /// <summary>
    /// Deserizalize an <see cref="HttpResponseMessage"/> with the expected type (support both Json and MessagePack response format)
    /// </summary>
    /// <typeparam name="T">Expected deserialized type</typeparam>
    /// <param name="httpResponse">HttpResponseMessage for wich we want to deserialize response content</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The deserialized object</returns>
    /// <exception cref="NotSupportedException">Expect an 'application/json' or 'application/x-msgpack' content type in response</exception>
    /// <exception cref="LgRemoteException">An exception deserialized from the server response</exception>
    /// <remarks>MessagePack support deserialization of primitive type while json deserialization does not (ex: T can be string with messagepack but will throw an exception for json)</remarks>
    public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        // Read response as byte (and later convert it to text if needed)
        byte[] content = await httpResponse.Content.ReadAsByteArrayAsync(cancellationToken);
        // Check for supported Content-Type in response header (assume application/json or text/plain if no explicit application/messagepack content tpye)
        bool isMessagePack = httpResponse.Content.Headers.ContentType?.MediaType == "application/x-msgpack";
        if (httpResponse.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }
        else if (httpResponse.IsSuccessStatusCode)
        {
            // Supported content type, parse response and return the deserialized response
            return isMessagePack
                   ? content.DeserializeFromMessagePack<T>(cancellationToken)
                   : content.DeserializeFromJson<T>();
        }
        else
        {
            // Check for common error in response (like an Exception/UserException) and raise an user or generic Exception
            ThrowParsedError(httpResponse.StatusCode, isMessagePack ? JsonSerializer.Serialize(content.DeserializeFromMessagePack<object>(cancellationToken)) : Encoding.UTF8.GetString(content));
            // The code should not pass here
            throw new InvalidOperationException();
        }
    }


    /// <summary>
    /// Deserialize data with MessagePack
    /// </summary>
    /// <typeparam name="T">Expected deserialization type</typeparam>
    /// <param name="data">An UTF8 binary string</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>Deserialized data</returns>
    private static T DeserializeFromMessagePack<T>(this byte[] data, CancellationToken cancellationToken = default)
    {
        return MessagePackSerializer.Deserialize<T>(data, _lagoonMessagePackSerializerOptions, cancellationToken);
    }

    /// <summary>
    /// Deserialize data as a json string
    /// </summary>
    /// <typeparam name="T">Expected deserialization type</typeparam>
    /// <param name="data">An UTF8 binary string</param>
    /// <returns>Deserialized data</returns>
    private static T DeserializeFromJson<T>(this byte[] data)
    {
        // rq: the stream seem to be not disposed by Deserialize(), so using+var
        using MemoryStream ms = new(data);
        return JsonSerializer.Deserialize<T>(ms, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
    }

    /// <summary>
    /// Try to decode an server error response
    /// </summary>
    /// <param name="statusCode">Http response status code</param>
    /// <param name="serverResponse">Raw server response</param>
    [DoesNotReturn]
    internal static void ThrowParsedError(HttpStatusCode statusCode, string serverResponse)
    {
        if (statusCode == HttpStatusCode.BadRequest)
        {
            // Server indicate a BadRequest, response must contain a ModelState
            try
            {
                Dictionary<string, List<string>> errors = null;
                // Check the type of BadRequest returned (Standard or custom)
                if (serverResponse.StartsWith("{\"type\":\"https://tools.ietf.org/"))
                {
                    //See https://docs.microsoft.com/fr-fr/aspnet/core/web-api/?view=aspnetcore-3.1#default-badrequest-response
                    errors = serverResponse.JsonDeserialize<ValidationProblemDetailsLight>().Errors;
                }
                else
                {
                    // BadRequest response, try to deserialize model state & display error
                    errors = serverResponse.JsonDeserialize<Dictionary<string, List<string>>>();
                }
                // If we have detailled error infos
                if (errors is not null && errors.Count > 0)
                {
                    // throw underlying errors
                    throw new LgValidationException("An error occured", null, errors);
                }
                else
                {
                    // throw a generic exception
                    throw new Exception("An error occured");
                }
            }
            catch (JsonException ex)
            {
                throw new Exception("An error occured", ex);
            }
        }
        else if (statusCode == HttpStatusCode.InternalServerError)
        {
            // Internal Error (Problem()), try to deserialize the error object
            string errorMessage;
            try
            {
                // An error occured, try to deserialize the exception
                errorMessage = serverResponse.JsonDeserialize<ErrorObjectResponse>().Message;
            }
            catch
            {
                /* Unable to parse error object, raw error response will be used */
                errorMessage = serverResponse;
            }
            // Throw the underlying server exception with logged flag
            // to avoid sending this exception to the server
            // (it is a server exception and has already been logged on the server)
            throw new LgRemoteException(errorMessage);
        }
        else if (statusCode == 0)
        {
            throw new OperationCanceledException(serverResponse);
        }
        else
        {
            throw new Exception($"An error occured. Status Code: {statusCode}. Response message: {serverResponse}");
        }
    }

    #endregion

    #region ISRuntime extensions (download file)

    /// <summary>
    /// Download a file sended by an application controller
    /// </summary>
    /// <param name="http">HttpClient extension method</param>
    /// <param name="js">JSRuntime to create local url and send file to browser</param>
    /// <param name="uri">Controller uri</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [Obsolete("Use the \"App.DownloadAsync\" method.")] //13/07/2022
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task DownloadFileAsync(this HttpClient http, IJSRuntime js, string uri)
    {
        return http.DownloadFileAsync(js, uri, null);
    }

    /// <summary>
    /// Download a file sended by an application controller
    /// </summary>
    /// <param name="http">HttpClient extension method</param>
    /// <param name="js">JSRuntime to create local url and send file to browser</param>
    /// <param name="uri">Controller uri</param>
    /// <param name="onProgress">Optionnal on progress callback if you want to show download progression</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [Obsolete("Use the \"App.DownloadAsync\" method.")] //13/07/2022
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async Task DownloadFileAsync(this HttpClient http, IJSRuntime js, string uri, Func<long, long, Task> onProgress)
    {
        // Retrieve file content & ensure successfull response
        HttpResponseMessage httpResponse = await http.GetAsync(uri);
        httpResponse.EnsureSuccessStatusCode();// => TODO Utiliser la méthode TryParseError pour gérer l'éventuel throw new UserException côté controlleur
                                               // Try to parse 'content-disposition" header to retrieve filename
        string contentDisposition = httpResponse.Content.Headers.GetValues("content-disposition").FirstOrDefault();
        // Try to parse 'content-type' header to retrieve file type
        string contentType = httpResponse.Content.Headers.GetValues("content-type").FirstOrDefault();
        if (ContentDispositionHeaderValue.TryParse(contentDisposition, out ContentDispositionHeaderValue contentDispositionValue)
              && MediaTypeHeaderValue.TryParse(contentType, out MediaTypeHeaderValue mediaTypeValue))
        {
            // Read file content from response stream
            byte[] result = null;
            if (onProgress == null)
            {
                result = await httpResponse.Content.ReadAsByteArrayAsync();
                bool attachment = contentDispositionValue.DispositionType == "attachment";
                // Send file to browser
                js.DownloadRawContent(contentDispositionValue.FileNameStar ?? contentDispositionValue.FileName,
                    mediaTypeValue.MediaType, result, attachment);
            }
            else
            {
                string tempJsFileId = await StreamToJsByteArrayAsync(js, await httpResponse.Content.ReadAsStreamAsync(), onProgress);
                await CompleteBytesChunkAsync(js, tempJsFileId, contentDispositionValue.FileName, mediaTypeValue.MediaType);
#if DEBUG
                Lagoon.Helpers.Trace.ToConsole("StreamToJsByteArrayAsync complete !");
#endif
            }
            return;
        }
        else
        {
            throw new Exception("Missing 'filename' and/or 'content-type' headers");
        }
    }

    /// <summary>
    /// Read a stream and return content as a byte array
    /// </summary>
    /// <param name="js">IJSRuntime</param>
    /// <param name="input">Stream to read</param>
    /// <param name="onProgress">On progress callback</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Content as byte array</returns>
    private static async Task<string> StreamToJsByteArrayAsync(IJSRuntime js, Stream input, Func<long, long, Task> onProgress, CancellationToken cancellationToken = default)
    {
        string id = Guid.NewGuid().ToString();
        long size = input.Length;
        byte[] buffer = new byte[8192];

        long uploaded = 0;
        int length;
        while ((length = await input.ReadAsync(buffer, cancellationToken)) > 0)
        {
            if (uploaded % 250 == 0)
            {
                await onProgress?.Invoke(uploaded, size);
            }
            BytesChunkToJs(js, id, length == buffer.Length ? buffer : buffer.Take(length).ToArray(), uploaded, size);
            uploaded += length;
        }
        await onProgress?.Invoke(uploaded, size);
        input.Dispose();
        return id;
    }

    /// <summary>
    /// Send a bunch of bytes to JS. The idea is to sent a lot of bytes without having to send them in one call
    /// </summary>
    /// <param name="js">IJSRuntime</param>
    /// <param name="id">Unique identifier</param>
    /// <param name="bytes">Array of bytes</param>
    /// <param name="offset">Start offset</param>
    /// <param name="size">Total size</param>
    private static void BytesChunkToJs(IJSRuntime js, string id, byte[] bytes, long offset, long size)
    {
        if (js is WebAssemblyJSRuntime webAssemblyJSRuntime)
        {
            webAssemblyJSRuntime.InvokeUnmarshalled<string, byte[], string, bool>("Lagoon.JsFileUtils.SetBytesChunk", id, bytes, $"{offset};{size}");
        }
    }

    /// <summary>
    /// Complete a file created with <see cref="BytesChunkToJs"/> method
    /// </summary>
    /// <param name="js"></param>
    /// <param name="id"></param>
    /// <param name="filename"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    private static async Task CompleteBytesChunkAsync(IJSRuntime js, string id, string filename, string contentType)
    {
        await js.InvokeVoidAsync("Lagoon.JsFileUtils.CompleteBytesChunk", id, filename, contentType);
    }

    #endregion

    #region obsolete methods (04/04/2024)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [Obsolete("Replace \"TryGetFromJsonAsync\" by \"TryGetAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<T> TryGetFromJsonAsync<T>(this HttpClient http, string requestUri, CancellationToken? cancellationToken = null, HttpCompletionOption? completionOption = null)
    {
        throw new InvalidOperationException();
    }

    [Obsolete("Replace \"TryPostAsJsonAsync\" by \"TryPostAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task TryPostAsJsonAsync<TIn>(this HttpClient http, string requestUri, TIn model, CancellationToken? cancellationToken = null)
    {
        throw new InvalidOperationException();
    }

    [Obsolete("Replace \"TryPostAsJsonAsync\" by \"TryPostAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TOut> TryPostAsJsonAsync<TIn, TOut>(this HttpClient http, string requestUri, TIn model, CancellationToken? cancellationToken = null)
    {
        throw new InvalidOperationException();
    }

    [Obsolete("Replace \"TryPutAsJsonAsync\" by \"TryPutAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task TryPutAsJsonAsync<TIn>(this HttpClient http, string requestURI, TIn model, CancellationToken? cancellationToken = null)
    {
        throw new InvalidOperationException();
    }

    [Obsolete("Replace \"TryPutAsJsonAsync\" by \"TryPutAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TOut> TryPutAsJsonAsync<TIn, TOut>(this HttpClient http, string requestURI, TIn model, CancellationToken? cancellationToken = null)
    {
        throw new InvalidOperationException();
    }

    [Obsolete("Replace \"TryPatchAsJsonAsync\" by \"TryPatchAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task TryPatchAsJsonAsync<TIn>(this HttpClient http, string requestURI, TIn model, CancellationToken? cancellationToken = null)
    {
        throw new InvalidOperationException();
    }

    [Obsolete("Replace \"TryPatchAsJsonAsync\" by \"TryPatchAsync\" call.", true), EditorBrowsable(EditorBrowsableState.Never)]
    public static Task<TOut> TryPatchAsJsonAsync<TIn, TOut>(this HttpClient http, string requestURI, TIn model, CancellationToken? cancellationToken = null)
    {
        throw new InvalidOperationException();
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    #endregion

}
