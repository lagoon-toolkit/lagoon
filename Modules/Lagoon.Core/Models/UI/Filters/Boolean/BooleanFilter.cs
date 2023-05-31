using Lagoon.Core.Helpers;

namespace Lagoon.UI.Components;


/// <summary>
/// Boolean filter expressions.
/// </summary>
/// <typeparam name="TBool">bool or bool? type.</typeparam>
public class BooleanFilter<TBool> : Filter<TBool, BooleanFilterItem<TBool>>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public BooleanFilter()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public BooleanFilter(params TBool[] values)
    {
        AddIncludedInList(values);
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public BooleanFilter(IEnumerable<TBool> list)
    {
        AddIncludedInList(list);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    public override void AddIncludedInList(IEnumerable<TBool> values)
    {
        base.AddIncludedInList(values.OrderBy(b => b));
    }

    ///<inheritdoc/>
    protected override void BuildDescription(FilterDescriptionBuilder<TBool> description)
    {
        foreach (TBool value in EnumerateIncludedValues())
        {
            description.AppendValue(value);
        }
    }

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TBool value)
    {
        return BooleanFormatter.Default.Format(value);
    }

    #endregion

}
