namespace Lagoon.UI.Components;


/// <summary>
/// Enumeration filter expressions.
/// </summary>
/// <typeparam name="TEnum">Enumeration type.</typeparam>
public class EnumFilter<TEnum> : Filter<TEnum, EnumFilterItem<TEnum>>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>        
    public EnumFilter()
    {            
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public EnumFilter(IEnumerable<TEnum> list)
    {
        AddIncludedInList(list);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override string DefaultFormatValue(TEnum value)
    {
        return value switch
        {
            null => null,
            Enum enumValue => enumValue.GetDisplayName(),
            _ => value.ToString()
        };
    }

    ///<inheritdoc/>
    protected override void BuildDescription(FilterDescriptionBuilder<TEnum> description)
    {
        foreach (TEnum value in EnumerateIncludedValues())
        {
            description.AppendValue(value);
        }
    }

    #endregion

}
