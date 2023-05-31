namespace Lagoon.UI.Components;

/// <summary>
/// Select Option change
/// </summary>
public class OptionEventArgs<TValue> : EventArgs
{

    #region properties

    /// <summary>
    /// option value
    /// </summary>
    public TValue Value { get; }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public OptionEventArgs(TValue value)
    {
        Value = value;
    }

}
