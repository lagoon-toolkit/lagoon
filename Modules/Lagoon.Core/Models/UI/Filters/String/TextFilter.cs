namespace Lagoon.UI.Components;

/// <summary>
/// Filter on string value.
/// </summary>
public class TextFilter : Filter<string, TextFilterItem>
{

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public TextFilter()
    {
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="searchedText">The text that must be contained in the string.</param>
    /// <param name="searchMode">Search mode (StartsWith, Contains, EndsWidth).</param>
    /// <param name="localCollationType">The type of case and accent sensitivity.</param>
    public TextFilter(FilterTextSearchMode searchMode, string searchedText, CollationType localCollationType)
        : this(searchMode, searchedText, localCollationType.ToCompareOptions())
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="searchedText">The text that must be contained in the string.</param>
    /// <param name="searchMode">Search mode (StartsWith, Contains, EndsWidth).</param>
    /// <param name="localCompareOptions">The compare option to be used by a local linq query.</param>
    public TextFilter(FilterTextSearchMode searchMode, string searchedText, CompareOptions localCompareOptions = CompareOptions.None)
    {
        AddTextSearch(searchMode, searchedText, localCompareOptions);
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="values">The list of values to include in the filtred list.</param>
    public TextFilter(params string[] values)
    {
        AddIncludedInList(values);
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="list">The list of values to include in the filtred list.</param>
    public TextFilter(IEnumerable<string> list)
    {
        AddIncludedInList(list);
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override string DefaultFormatValue(string value)
    {
        return value;
    }

    ///<inheritdoc/>
    protected override void BuildDescription(FilterDescriptionBuilder<string> description)
    {
        // Includes values list
        foreach (string value in EnumerateIncludedValues())
        {
            description.AppendValue(value);
        }
        // SearchText filters
        foreach (TextFilterItem filter in Values)
        {
            if (filter.SearchedText is not null)
            {
                description.AppendSeparator();
                description.Append(filter.SearchMode.GetDisplayName());
                description.Append(" “");
                description.AppendValue(filter.SearchedText, false);
                description.Append("”");
            }
        }
    }

    /// <summary>
    /// Get the text of the (first...) "Contains text" filter.
    /// </summary>
    /// <returns>The text of the (first...) "Contains text" filter.<c>null</c> id not found.</returns>
    public string GetSearchedText()
    {
        return Values.FirstOrDefault(x => x.SearchedText is not null)?.SearchedText;
    }

    /// <summary>.
    /// Add (or replace) a new filter testing if a field contains a text
    /// </summary>
    /// <param name="searchedText">The text that must be contained in the string.</param>
    /// <param name="searchMode">Search mode (StartsWith, Contains, EndsWidth).</param>
    /// <param name="localCompareOptions">The compare option to be used by a local linq query.</param>
    public void AddTextSearch(FilterTextSearchMode searchMode, string searchedText, CompareOptions localCompareOptions = CompareOptions.None)
    {
        Values.Add(new TextFilterItem() { SearchedText = searchedText, SearchMode = searchMode, CompareOptions = localCompareOptions });
    }

    #endregion

}
