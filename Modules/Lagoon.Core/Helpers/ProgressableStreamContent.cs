using System.Diagnostics.Contracts;
using System.Net;
using System.Threading;
using System;

namespace Lagoon.Helpers;

/// <summary>
/// Defines a progressable stream content.
/// </summary>
public class ProgressableStreamContent : HttpContent
{

    #region fields

    /// <summary>
    /// Default buffer size
    /// </summary>
    private const int DefaultBufferSize = 16384;

    /// <summary>
    /// The stream to Post
    /// </summary>
    private readonly Stream _streamContent;

    /// <summary>
    /// Actual buffer size
    /// </summary>
    private readonly int _bufferSize;

    /// <summary>
    /// The on progress callback (sended, totalSended, totalToSend).
    /// </summary>
    private readonly Func<long, long, long, Task> _onProgress;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressableStreamContent"/> class for observable stream progression.
    /// </summary>
    /// <param name="content">Stream to send</param>
    /// <param name="onProgress">On progress callback</param>
    public ProgressableStreamContent(Stream content, Func<long, long, long, Task> onProgress) : this(content, onProgress, DefaultBufferSize) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressableStreamContent"/> class for observable stream progression.
    /// </summary>
    /// <param name="content">Stream to send</param>
    /// <param name="onProgress">On progress callback</param>
    /// <param name="bufferSize">Read buffer size used to read <paramref name="content"/> stream</param>
    public ProgressableStreamContent(Stream content, Func<long, long, long, Task> onProgress, int bufferSize)
    {
        if (bufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }
        _streamContent = content ?? throw new ArgumentNullException(nameof(content));
        _bufferSize = bufferSize;
        _onProgress = onProgress;
    }

    #endregion

    #region methods

    /// <inheritdoc />
    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        Contract.Assert(stream != null);
        byte[] buffer = new byte[_bufferSize];
        long size = _streamContent.Length;
        long uploaded = 0;
        int length;
        int reported = 0;
        while ((length = await _streamContent.ReadAsync(buffer, CancellationToken.None)) > 0)
        {
            uploaded += length;
            reported += length;
            // Avoid to report progression to many time
            if (uploaded % 250 == 0)
            {
                await _onProgress?.Invoke(reported, uploaded, size);
                reported = 0;
            }
            // Write data to the output stream
            await stream.WriteAsync(buffer);
        }
        // Report last progression
        await _onProgress?.Invoke(reported, uploaded, size);
    }

    /// <inheritdoc />
    protected override bool TryComputeLength(out long length)
    {
        length = _streamContent.Length;
        return true;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Console.WriteLine($"ProgressableStreamContent ===> Disposing ");
            _streamContent.Dispose();
        }
        Console.WriteLine($"ProgressableStreamContent ===> Disposing but false");
        base.Dispose(disposing);
    }

    #endregion

}