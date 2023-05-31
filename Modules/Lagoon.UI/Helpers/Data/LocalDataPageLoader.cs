namespace Lagoon.Helpers.Data;

/// <summary>
/// Data loader for local data
/// </summary>
/// <typeparam name="T"></typeparam>
public class LocalDataPageLoader<T> : CustomDataPageLoader<T>
{
    /// <summary>
    /// Gets or sets data source list
    /// </summary>
    public IQueryable<T> Items { get; set; }

    ///<inheritdoc/>
    public override Task<IEnumerable<TValue>> GetSelectorValuesAsync<TValue>(Expression<Func<T, TValue>> filterExpression, CancellationToken cancellationToken = default)
    {
        IEnumerable<TValue> query = PageDataLoader<T>.ApplyQueryFilterAndSort(Items, ModelFilter, SortOptions).Select(filterExpression);
        return Task.FromResult(query);
    }

    /// <inheritdoc/>
    public override async Task<DataPage<T>> GetDataPageAsync(IEnumerable<string> requiredFields, CancellationToken cancellationToken = default)
    {
        int pageCount;
        bool isLastPage;            
        // Get the filtred items
        IQueryable<T> query = PageDataLoader<T>.ApplyQueryFilterAndSort(Items, ModelFilter, SortOptions);            
        // Count the filtred items
        int totalResult = AllowCount ? query.Count() : -1;
        // Add the sort to the filtred items
        query = PageDataLoader<T>.ApplyQuerySort(query, SortOptions);
        IEnumerable<T> result;
        // Check the page number
        if (PageSize > 0)
        {
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            if (AllowCount)
            {
                pageCount = GetPageCount(totalResult);
                if (CurrentPage > pageCount)
                {
                    CurrentPage = pageCount;
                }
                isLastPage = CurrentPage == pageCount;
                result = ApplyQueryPagination(query, CurrentPage, PageSize);
            }
            else
            {
                // The page count is indeterminate
                pageCount = 0;
                // Going through all the elements seems costly, we keep the selected items in a list
                List<T> items;
                int currentPage = CurrentPage + 1;
                do
                {
                    currentPage--;
                    // Get the page items + one more (last page detection)
                    items = new(ApplyQueryPagination(query, currentPage, PageSize, true));
                } while (items.Count == 0 && currentPage > 1);
                CurrentPage = currentPage;
                // Detect the last page (and remove the detection extra line)
                isLastPage = DetectLastPage(items);
                result = items;
            }
        }
        else
        {
            CurrentPage = 1;
            pageCount = 1;
            isLastPage = true;
            result = query;
        }
        Dictionary<string, object> calculationResults = null;
        if (CalculationOptions is not null && CalculationOptions.Any())
        {
            calculationResults = await GetCalculationAsync(cancellationToken);
        }            
        return new DataPage<T>(CurrentPage, result, PageSize, isLastPage, pageCount, totalResult, calculationResults);
    }

    /// <summary>
    /// Gets the calculation for each columns.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The calculated values.</returns>
    public override Task<Dictionary<string, object>> GetCalculationAsync(CancellationToken cancellationToken = default)
    {            
        IQueryable<T> query = PageDataLoader<T>.ApplyQueryFilterAndSort(Items, ModelFilter, null);            
        Expression<Func<IGrouping<int, T>, Dictionary<string, object>>> lambda = ApplyCalculations(CalculationOptions);
        IQueryable<Dictionary<string, object>> result = query.GroupBy(c => 1).AsQueryable().Select(lambda);
        return Task.FromResult(result.SingleOrDefault());
    }

}
