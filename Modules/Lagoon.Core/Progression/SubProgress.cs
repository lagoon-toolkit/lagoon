namespace Lagoon.Helpers;

/// <summary>
/// Sub progress state.
/// </summary>
internal class SubProgress : Progress
{

    #region fields

    private readonly Progress _parent;
    private bool _restoreIndeterminate;
    private double _step;
    private int _previousReportingParentPos;
    private readonly bool _ignoreMessages;
    private readonly int _subPartLength;

    #endregion

    #region constructors

    /// <summary>
    /// Initialise a new progression state.
    /// </summary>
    /// <param name="parent">Parent progression.</param>
    /// <param name="subPartLength">Length of the sub-progression inside the main progression.</param>
    /// <param name="startPosition">Start position for the sub progression.</param>
    /// <param name="endPosition">End position for the sub progression.</param>
    /// <param name="ignoreMessages">Ignore the sub progress report messages.</param>
    /// <param name="updateMessage">Update the message.</param>
    /// <param name="message">Initial message.</param>
    public SubProgress(Progress parent, int subPartLength,
        int startPosition, int endPosition, bool ignoreMessages, bool updateMessage = false, string message = null)
    {
        _parent = parent;
        _subPartLength = subPartLength;
        _ignoreMessages = ignoreMessages;
        Reset(startPosition, endPosition, updateMessage, message);
    }

    #endregion

    #region methods

    /// <summary>
    /// Restore the progression start state.
    /// </summary>
    protected override void OnResetting()
    {
        base.OnResetting();
        RestoreIndeterminate();
        // Override the parent progress bound
        _restoreIndeterminate = !IsIndeterminate && _parent.IsIndeterminate;
        if (_restoreIndeterminate)
        {
            _parent.Reset(StartPosition, EndPosition, false);
        }
        // Step position
        if (EndPosition > StartPosition)
        {
            _step = (double)_subPartLength / (EndPosition - StartPosition);
        }
    }

    /// <summary>
    /// The progression is over.
    /// </summary>
    protected override void OnEnding()
    {
        base.OnEnding();
        RestoreIndeterminate();
    }

    /// <summary>
    /// Restore the indeterminate state for the parent.
    /// </summary>
    private void RestoreIndeterminate()
    {
        if (_restoreIndeterminate)
        {
            _parent.Reset(0, 0, false);
            _restoreIndeterminate = false;
        }
    }

    ///<inheritdoc/>
    protected override void Report(int position, string message, bool updatePosition, bool updateMessage, bool autoEnd = true)
    {
        if (_ignoreMessages) updateMessage = false;
        base.Report(position, message, updatePosition, updateMessage, autoEnd);
        if (updatePosition && !IsIndeterminate)
        {
            var newPos = (int)Math.Floor((position - StartPosition) * _step);
            _parent.Increment(newPos - _previousReportingParentPos, message, updateMessage, !_restoreIndeterminate);
            _previousReportingParentPos = newPos;
        }
        else if (updateMessage)
        {
            _parent.Report(message);
        }
    }

    #endregion

}
