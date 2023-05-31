namespace Lagoon.UI.Components;

/// <summary>
/// Filter parameter
/// </summary>
public sealed class TextFilterItem : FilterItem<string>
{

    #region properties

    /// <summary>
    /// The search text mode.
    /// </summary>
    [JsonPropertyName("mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FilterTextSearchMode SearchMode { get; set; }

    /// <summary>
    /// Gets or sets the type of case and accent sensitivity
    /// </summary>             
    [JsonPropertyName("compare")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CompareOptions CompareOptions { get; set; } = CompareOptions.IgnoreCase;

    /// <summary>
    /// Text to be contained in filtred values.
    /// </summary>
    [JsonPropertyName("txt")]
    public string SearchedText { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Return the list of conditions for this filter.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The list of conditions for this filter.</returns>
    protected override IEnumerable<LambdaExpression> GetWhereIncludeExpressions(FilterWhereContext context)
    {
        if (!string.IsNullOrEmpty(SearchedText))
        {
            yield return GetSearchTextWhereExpression(context);
        }
        foreach (LambdaExpression expression in base.GetWhereIncludeExpressions(context))
        {
            yield return expression;
        }
    }

    /// <summary>
    /// The expression representing text contains condition.
    /// </summary>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>he expression representing text contains condition.</returns>
    private Expression<Func<string, bool>> GetSearchTextWhereExpression(FilterWhereContext context)
    {
        if (context.UseDefaultCollation)
        {
            return SearchMode switch
            {
                FilterTextSearchMode.StartsWith => x => x != null && x.StartsWith(SearchedText),
                FilterTextSearchMode.EndsWith => x => x != null && x.EndsWith(SearchedText),
                _ => x => x != null && x.Contains(SearchedText)
            };
        }
        else if(context.TargetEF)
        {
            // Remarks : Accents sensitivity is only available with collation use for DB requests
            return CompareOptions == CompareOptions.None
                ? SearchMode switch
                {
                    FilterTextSearchMode.StartsWith => x => x != null && x.StartsWith(SearchedText),
                    FilterTextSearchMode.EndsWith => x => x != null && x.EndsWith(SearchedText),
                    _ => x => x != null && x.Contains(SearchedText)
                }
                : SearchMode switch
                {                    
                    FilterTextSearchMode.StartsWith => x => x != null && x.ToLower().StartsWith(SearchedText.ToLower()),
                    FilterTextSearchMode.EndsWith => x => x != null && x.ToLower().EndsWith(SearchedText.ToLower()),
                    _ => x => x != null && x.ToLower().Contains(SearchedText.ToLower())
                };
        }
        else
            {
            return CompareOptions == CompareOptions.None
                ? SearchMode switch
                {
                    FilterTextSearchMode.StartsWith => x => x != null && x.StartsWith(SearchedText),
                    FilterTextSearchMode.EndsWith => x => x != null && x.EndsWith(SearchedText),
                    _ => x => x != null && x.Contains(SearchedText)
                }
                : SearchMode switch
                {
                    FilterTextSearchMode.StartsWith => x => x != null && CultureInfo.InvariantCulture.CompareInfo.IsPrefix(x, SearchedText, CompareOptions),
                    FilterTextSearchMode.EndsWith => x => x != null && CultureInfo.InvariantCulture.CompareInfo.IsSuffix(x, SearchedText, CompareOptions),
                    _ => x => x != null && CultureInfo.InvariantCulture.CompareInfo.IndexOf(x, SearchedText, CompareOptions) != -1
                };
        }
    }

    #endregion

}
