namespace Lagoon.Helpers;

/// <summary>
/// Informations about the progression.
/// </summary>
public class Progress
{
    #region fields

    private bool _isIndeterminate;
    private bool _isEnded;
    private int _position;
    private string _message;
    private int _startPosition;
    private int _endPosition;

    #endregion

    #region properties

    /// <summary>
    /// Indicate if the current position is indeterminate.
    /// </summary>
    public bool IsIndeterminate => _isIndeterminate;

    /// <summary>
    /// Indicate if the progression is over
    /// </summary>
    public bool IsEnded => _isEnded;

    /// <summary>
    /// Start position.
    /// </summary>
    public int StartPosition => _startPosition;

    /// <summary>
    /// End position.
    /// </summary>
    public int EndPosition => _endPosition;

    /// <summary>
    /// Current position.
    /// </summary>
    public int Position => _position;

    /// <summary>
    /// Progression description.
    /// </summary>
    public string Message => _message;

    #endregion

    #region events

    /// <summary>
    /// Event raised when a new progression is reported.
    /// </summary>
    public event Action<Progress> OnProgress;

    /// <summary>
    /// Event raised when the progression is over.
    /// </summary>
    public event Action<Progress> OnEnd;

    /// <summary>
    /// Event raised when the progression is reset.
    /// </summary>
    public event Action<Progress> OnReset;

    #endregion

    #region constructors

    /// <summary>
    /// Initialise a new indeterminated progression state.
    /// </summary>
    /// <param name="message">Initial message.</param>
    public Progress(string message = null)
    {
        Reset(0, 0, message);
    }

    /// <summary>
    /// Initialise a new progression state starting from 0.
    /// </summary>
    /// <param name="endPosition">End position.</param>
    /// <param name="message">Initial message.</param>
    public Progress(int endPosition, string message = null)
    {
        Reset(endPosition, message);
    }
    /// <summary>
    /// Initialise a new progression state.
    /// </summary>
    /// <param name="startPosition">Start position.</param>
    /// <param name="endPosition">End position.</param>
    /// <param name="message">Initial message.</param>
    public Progress(int startPosition, int endPosition, string message = null)
    {
        Reset(startPosition, endPosition, message);
    }

    #endregion

    #region "Reset" methods

    /// <summary>
    /// Reset the current progression.
    /// </summary>
    /// <param name="message">Initial message.</param>
    public void Reset(string message = null)
    {
        Reset(_startPosition, _endPosition, message);
    }

    /// <summary>
    /// Initialise a new progression state starting from 0.
    /// </summary>
    /// <param name="endPosition">End position.</param>
    /// <param name="message">Initial message.</param>
    public void Reset(int endPosition, string message = null)
    {
        Reset(0, endPosition, message);
    }

    /// <summary>
    /// Initialise a new progression state.
    /// </summary>
    /// <param name="startPosition">Start position.</param>
    /// <param name="endPosition">End position.</param>
    /// <param name="message">Initial message.</param>
    public void Reset(int startPosition, int endPosition, string message = null)
    {

        Reset(startPosition, endPosition, true, message);
        OnResetting();
        OnReset?.Invoke(this);
        OnProgress?.Invoke(this);
    }

    /// <summary>
    /// Reset the progression parameters.
    /// </summary>
    /// <param name="startPosition">Start position.</param>
    /// <param name="endPosition">End position.</param>
    /// <param name="updateMessage">Update the message.</param>
    /// <param name="message">Initial message.</param>
    internal void Reset(int startPosition, int endPosition, bool updateMessage, string message = null)
    {
        _isEnded = false;
        _isIndeterminate = startPosition >= endPosition;
        _startPosition = startPosition;
        _endPosition = endPosition;
        _position = startPosition;
        if (updateMessage)
        {
            _message = message;
        }
    }

    /// <summary>
    /// The progression is resetting.
    /// </summary>
    protected virtual void OnResetting()
    {
    }

    #endregion

    #region "Increment" methods

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    public void Increment()
    {
        Increment(1, null, false);
    }

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="message">The new progression message.</param>
    public void Increment(string message)
    {
        Increment(1, message, true);
    }

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="value">Value to add to the current position.</param>
    public void Increment(int value)
    {
        Increment(value, null, false);
    }

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="value">Value to add to the current position.</param>
    /// <param name="message">The new progression message.</param>
    public void Increment(int value, string message)
    {
        Increment(value, message, true);
    }

    /// <summary>
    /// Increment current position
    /// </summary>
    /// <param name="value">Value to add to the current position.</param>
    /// <param name="message">The new progression message if <paramref name="updateMessage"/> is <c>true</c>.</param>
    /// <param name="updateMessage">Indicate if the message must be updated or ignored.</param>
    /// <param name="autoEnd">Indicate if the "End" state is set when the position reach the "EndPosition".</param>
    internal void Increment(int value, string message, bool updateMessage, bool autoEnd = true)
    {
        Report(_position + value, message, true, updateMessage, autoEnd);
    }

    #endregion

    #region "Report" methods

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="position">The new progression position.</param>
    public void Report(int position)
    {
        Report(position, default, true, false);
    }

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="message">The new progression message.</param>
    public void Report(string message)
    {
        Report(default, message, false, true);
    }

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="position">The new progression position.</param>
    /// <param name="message">The new progression message.</param>
    public void Report(int position, string message)
    {
        Report(position, message, true, true);
    }

    /// <summary>
    /// Indicate the new running progression state.
    /// </summary>
    /// <param name="position">The new progression position if <paramref name="updatePosition"/> is <c>true</c>.</param>
    /// <param name="message">The new progression message if <paramref name="updateMessage"/> is <c>true</c>.</param>
    /// <param name="updatePosition">Indicate if the position must be updated or ignored.</param>
    /// <param name="updateMessage">Indicate if the message must be updated or ignored.</param>
    /// <param name="autoEnd">Indicate if the "End" state is set when the position reach the "EndPosition".</param>
    protected virtual void Report(int position, string message, bool updatePosition, bool updateMessage, bool autoEnd = true)
    {
        if (updateMessage)
        {
            _message = message;
        }
        if (updatePosition && position != _position && !IsIndeterminate)
        {
            _position = position;
            if (autoEnd && _position >= EndPosition)
            {
                Stop(true);
            }
            else
            {
                OnProgress?.Invoke(this);
            }
        }
        else if (updateMessage)
        {
            OnProgress?.Invoke(this);
        }
    }

    #endregion

    #region "ReportEnd" methods

    /// <summary>
    /// Indicates that the process is finished.
    /// </summary>
    public void ReportEnd()
    {
        ReportEnd(default, false);
    }

    /// <summary>
    /// Indicates that the process is finished.
    /// </summary>
    public void ReportEnd(string message)
    {
        ReportEnd(message, true);
    }


    /// <summary>
    /// Indicates that the process is finished.
    /// </summary>
    /// <param name="message">The new progression message if <paramref name="updateMessage"/> is <c>true</c>.</param>
    /// <param name="updateMessage">Indicate if the message must be updated or ignored.</param>
    private void ReportEnd(string message, bool updateMessage)
    {
        if (updateMessage)
        {
            _message = message;
        }
        Stop(false);
    }

    /// <summary>
    /// Indicate the process is done.
    /// </summary>
    /// <param name="raiseOnProgress"></param>
    private void Stop(bool raiseOnProgress)
    {
        _isEnded = true;
        OnEnding();
        if (raiseOnProgress)
        {
            OnProgress?.Invoke(this);
        }
        OnEnd?.Invoke(this);
    }

    /// <summary>
    /// The progression is over.
    /// </summary>
    protected virtual void OnEnding()
    {
    }

    #endregion

    #region "OpenSubProgression" methods

    /// <summary>
    /// Open a new sub indeterminate progression .
    /// </summary>
    /// <param name="subPartLength">Length of the sub-progression inside the main progression.</param>
    /// <param name="ignoreMessages">Ignore the sub progress report messages.</param>
    /// <returns>The new sub progress instance.</returns>
    public Progress OpenSubProgress(int subPartLength, bool ignoreMessages = false)
    {
        return new SubProgress(this, subPartLength, 0, 0, ignoreMessages);
    }

    /// <summary>
    /// Open a new sub indeterminate progression .
    /// </summary>
    /// <param name="subPartLength">Length of the sub-progression inside the main progression.</param>
    /// <param name="message">Initial message.</param>
    /// <param name="ignoreMessages">Ignore the sub progress report messages.</param>
    /// <returns>The new sub progress instance.</returns>
    public Progress OpenSubProgress(int subPartLength, string message, bool ignoreMessages = false)
    {
        return new SubProgress(this, subPartLength, 0, 0, ignoreMessages, true, message);
    }


    /// <summary>
    /// Open a new sub progression
    /// </summary>
    /// <param name="subPartLength">Length of the sub-progression inside the main progression.</param>
    /// <param name="startPosition">Start position for the sub progression.</param>
    /// <param name="endPosition">End position for the sub progression.</param>
    /// <param name="ignoreMessages">Ignore the sub progress report messages.</param>
    /// <returns>The new sub progress instance.</returns>
    public Progress OpenSubProgress(int subPartLength, int startPosition, int endPosition, bool ignoreMessages = false)
    {
        return new SubProgress(this, subPartLength, startPosition, endPosition, ignoreMessages);
    }

    /// <summary>
    /// Open a new sub progression
    /// </summary>
    /// <param name="subPartLength">Length of the sub-progression inside the main progression.</param>
    /// <param name="startPosition">Start position for the sub progression.</param>
    /// <param name="endPosition">End position for the sub progression.</param>
    /// <param name="message">Initial message.</param>
    /// <param name="ignoreMessages">Ignore the sub progress report messages.</param>
    /// <returns>The new sub progress instance.</returns>
    public Progress OpenSubProgress(int subPartLength, int startPosition, int endPosition, string message, bool ignoreMessages = false)
    {
        return new SubProgress(this, subPartLength, startPosition, endPosition, ignoreMessages, true, message);
    }

    #endregion

    #region operator overloading

    /// <summary>
    /// Increment progression.
    /// </summary>
    /// <param name="progress">Progression state</param>
    /// <returns></returns>
    public static Progress operator ++(Progress progress)
    {
        progress.Report(progress.Position + 1, default, true, false);
        return progress;
    }

    #endregion

}