namespace Lagoon.UI.Components;


/// <summary>
/// Enumeration filter expressions.
/// </summary>
/// <typeparam name="TItem">Select item type.</typeparam>
public class SelectFilter<TItem> : Filter<TItem, SelectFilterItem<TItem>>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>        
    public SelectFilter()
    {            
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public SelectFilter(IEnumerable<TItem> list)
    {
        AddIncludedInList(list);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TItem value)
    {
        return null;
    }

    ///<inheritdoc/>
    protected override void BuildDescription(FilterDescriptionBuilder<TItem> description)
    {
        foreach (TItem value in EnumerateIncludedValues())
        {
            description.AppendValue(value);
        }
    }

    #endregion

}
