namespace Lagoon.UI.Components;

/// <summary>
/// Create a data source from an Enum declaration. Use the [Display("#sample")] attribute on enumeration values
/// to define the text.
/// </summary>
/// <typeparam name="TEnum">The enum type to use.</typeparam>
public class EnumListDataSource<TEnum> : EnumListDataSourceBase<TEnum, TEnum> where TEnum : struct, Enum
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public EnumListDataSource()
    {
    }

#if ADVANCED_CONSTRUCTOR
/* !!!!!!!!!!!! Don't activate this, prefer to promote the use of "DisplayAttribute".Order/Name ... */

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="sortByText">If <c>true</c> the list is sorted by text; else the original order is preserved.</param>
    /// <param name="ignoredValues">List of values to ignore.</param>
    public EnumListDataSource(bool sortByText = false, params TEnum[] ignoredValues) : base(sortByText, ignoredValues)
    {
    }

#endif

    #endregion

    #region methods

    ///<inheritdoc/>
    public override TEnum GetItemValue(TEnum value)
    {
        return value;
    }

    internal override TEnum GetValueItem(TEnum value)
    {
        return value;
    }

    #endregion
}
