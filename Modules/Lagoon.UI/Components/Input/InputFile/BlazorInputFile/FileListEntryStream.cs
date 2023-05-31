namespace Lagoon.UI.Internal.BlazorInputFile;

/// <summary>
/// TODO: When ReadAsync is called, don't just fetch the segment of data that's being requested.
/// That will be very slow, as you may be doing a separate round-trip for each 1KB or so of data.
/// Instead, have a larger buffer whose size == SignalR.MaxMessageSize and populate that. Then
/// many of the ReadAsync calls can return immediately with already-loaded data.
///
/// This is still not as fast as allowing the client to send as much data as it wants, and using
/// TCP to apply backpressure. In the future we could achieve something closer to that by having
/// an even larger buffer, and telling the client to send N messages in parallel. The ReadAsync
/// calls would return whenever their portion of the buffer was populated. This is much more
/// complicated to implement.
/// </summary>
public abstract class FileListEntryStream : Stream
{

    #region fields

    /// <summary>
    /// JSRuntime to retrieve data from JS
    /// </summary>
    protected readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Reference to the HTMLInputElement (wich hold files data)
    /// </summary>
    protected readonly ElementReference _inputFileElement;

    /// <summary>
    /// A file entry
    /// </summary>
    protected readonly FileListEntryImpl _file;

    /// <summary>
    /// Stream position
    /// </summary>
    private long _position;

    #endregion

    #region initialisation

    /// <summary>
    /// Initialise a new <see cref="FileListEntryStream"/>
    /// </summary>
    /// <param name="jsRuntime">JSRuntime</param>
    /// <param name="inputFileElement">HTMLInputElement reference</param>
    /// <param name="file">File entry</param>
    public FileListEntryStream(IJSRuntime jsRuntime, ElementReference inputFileElement, FileListEntryImpl file)
    {
        _jsRuntime = jsRuntime;
        _inputFileElement = inputFileElement;
        _file = file;
    }

    #endregion

    #region Stream override

    /// <summary>
    /// Is the stream readable
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// Is the stream seekable
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// Is the stream writable
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    /// Stream length
    /// </summary>
    public override long Length => _file.Size;

    /// <summary>
    /// Current stream position
    /// </summary>
    public override long Position
    {
        get => _position;
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Not supported for this stream
    /// </summary>
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Not supported for this stream
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Synchronous reads are not supported");
    }

    /// <summary>
    /// Not supported for this stream
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Not supported for this stream
    /// </summary>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Not supported for this stream
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Read a part of the stream
    /// </summary>
    /// <param name="buffer">Data to read</param>
    /// <param name="offset">Start offset</param>
    /// <param name="count">Number of bytes readed</param>
    /// <param name="cancellationToken">A CancellationToken to interrup the process</param>
    /// <returns></returns>
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var maxBytesToRead = (int)Math.Min(count, Length - Position);
        if (maxBytesToRead == 0)
        {
            return 0;
        }

        var actualBytesRead = await CopyFileDataIntoBuffer(_position, buffer, offset, maxBytesToRead, cancellationToken);
        _position += actualBytesRead;
        _file.RaiseOnDataRead();
        return actualBytesRead;
    } 

    #endregion

    /// <summary>
    /// Copy data
    /// </summary>
    /// <param name="sourceOffset">Source from</param>
    /// <param name="destination">Destination array</param>
    /// <param name="destinationOffset">Destination offset</param>
    /// <param name="maxBytes">Byte to retrives</param>
    /// <param name="cancellationToken">CancellationToken</param>
    protected abstract Task<int> CopyFileDataIntoBuffer(long sourceOffset, byte[] destination, int destinationOffset, int maxBytes, CancellationToken cancellationToken);

}
