namespace Lagoon.UI.Components;

/// <summary>
/// Arguments when changing value.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class ChangingEventArgs<T>
{

    #region fields

    private T _oldValue;

    #endregion

    #region properties


    /// <summary>
    /// Gets the object that contains data about the control.
    /// </summary>
    public object Item { get; }

    /// <summary>
    /// The previous value.
    /// </summary>
    public T OldValue => _oldValue;

    /// <summary>
    /// The new value.
    /// </summary>
    public T NewValue { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="oldValue">The current value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="item">The object that contains data about the control.</param>
    public ChangingEventArgs(T oldValue, T newValue, object item)
    {
        _oldValue = oldValue;
        NewValue = newValue;
        Item = item;
    }

    #endregion

}
