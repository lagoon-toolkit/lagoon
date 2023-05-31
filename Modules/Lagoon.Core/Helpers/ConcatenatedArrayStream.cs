namespace Lagoon.Core.Helpers;

/// <summary>
/// A stream to read a series of bytes arrays as one.
/// </summary>
public class ConcatenatedArrayStream : Stream
{

    #region fields

    private List<byte[]> _arrays;
    private int _currentArray;
    private long _currentPosition;
    private long _length;
    private long _position;

    #endregion

    #region properties

    /// <summary>
    /// Gets a value indicating whether the current stream supports reading.
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking.
    /// </summary>
    public override bool CanSeek => true;

    /// <summary>
    /// Gets a value indicating whether the current stream supports writing.
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    /// Gets the length in bytes of the stream.
    /// </summary>
    public override long Length => _length;

    /// <summary>
    /// Gets or sets the position within the current stream.
    /// </summary>
    public override long Position { get => _position; set => SetPosition(value); }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="arrays">An ordered list of array to concatenate.</param>
    public ConcatenatedArrayStream(params byte[][] arrays)
        : this((IEnumerable<byte[]>)arrays) { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="arrays">An ordered list of array to concatenate.</param>
    public ConcatenatedArrayStream(IEnumerable<byte[]> arrays)
    {
        _arrays = new();
        foreach (byte[] array in arrays)
        {
            if (array is not null && array.Length > 0)
            {
                _arrays.Add(array);
                _length += array.Length;
            }
        }
        _currentArray = _arrays.Count == 0 ? -1 : 0;
    }

    #endregion

    #region methods

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream does not support writing.</exception>
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
    /// byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number
    /// of bytes requested if that many bytes are not currently available, or zero (0)
    /// if the end of the stream has been reached.
    /// </returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        int result = 0;
        byte[] array;
        long left;
        long toCopy;
        while (count > 0 && _currentArray != -1)
        {
            array = _arrays[_currentArray];
            left = array.LongLength - _currentPosition;
            toCopy = Math.Min(count, left);
            Array.Copy(array, _currentPosition, buffer, offset, toCopy);
            result += (int)toCopy;
            if (count > left)
            {
                if (_currentArray == _arrays.Count - 1)
                {
                    _currentArray = -1;
                }
                else
                {
                    _currentArray++;
                    _currentPosition = 0;
                    count -= (int)left;
                    offset += (int)toCopy;
                }
            }
            else
            {
                _currentPosition += toCopy;
                break;
            }
        }
        _position += result;
        return result;
    }

    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
    /// <returns>The new position within the current stream.</returns>  
    public override long Seek(long offset, SeekOrigin origin)
    {
        Position = origin switch
        {
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => _length - offset,
            _ => offset
        };
        return Position;
    }

    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="value">The desired length of the current stream in bytes.</param>
    /// <exception cref="NotSupportedException">The stream does not support writing.</exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Update the current position.
    /// </summary>
    /// <param name="position">The new position.</param>
    private void SetPosition(long position)
    {
        _position = position;
        _currentArray = -1;
        for (int i = 0; i < _arrays.Count; i++)
        {
            if (position < _arrays[i].LongLength)
            {
                _currentArray = i;
                _currentPosition = position;
                break;
            }
            position -= _arrays[i].LongLength;
        }
    }

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position
    /// within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">This method copies count bytes from buffer to the current stream.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
    /// <param name="count">The number of bytes to be written to the current stream.</param>
    /// <exception cref="NotSupportedException">The stream does not support writing.</exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    #endregion

}
