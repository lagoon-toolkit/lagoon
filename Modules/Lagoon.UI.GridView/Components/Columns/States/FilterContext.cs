namespace Lagoon.UI.Components;

/// <summary>
/// The context for a filter render fragment.
/// </summary>
public class FilterContext
{

    #region fields

    /// <summary>
    /// The column state.
    /// </summary>
    private GridColumnState _columnState;

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the current filter of the column.
    /// </summary>
    public Filter Filter { get => _columnState.Filter; set => _columnState.SetCurrentFilter(value); }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="columnState">The column state</param>
    internal FilterContext(GridColumnState columnState)
    {
        _columnState = columnState;
    }

    #endregion

}
