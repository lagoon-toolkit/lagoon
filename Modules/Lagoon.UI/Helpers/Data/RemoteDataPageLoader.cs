using Lagoon.Shared.Model;

namespace Lagoon.Helpers.Data;

/// <summary>
/// Helper to gets pagged item list from a remote source.
/// </summary>
public class RemoteDataPageLoader<T> : CustomDataPageLoader<T>
{

    #region private class

    /// <summary>
    /// Query data builder interface.
    /// </summary>
    internal interface IQueryDataBuilder : IDataQueryRequest
    {

        public Task<DataQueryResponse<TResult>> PostAsync<TResult>(HttpClient httpClient, CancellationToken cancellationToken);

    }

    /// <summary>
    /// Query data builder.
    /// </summary>
    /// <typeparam name="TArg">Data type.</typeparam>
    internal class QueryDataBuilder<TArg> : DataQueryRequest<TArg>, IQueryDataBuilder
    {

        private string _controllerUri;

        public QueryDataBuilder(string controllerUri, TArg controllerQueryArg)
        {
            _controllerUri = controllerUri;
            ControllerQueryArg = controllerQueryArg;
        }

        public Task<DataQueryResponse<TResult>> PostAsync<TResult>(HttpClient httpClient, CancellationToken cancellationToken)
        {            
            return httpClient.TryPostAsync<QueryDataBuilder<TArg>, DataQueryResponse<TResult>>($"{_controllerUri}/{IDataQueryRequest.QueryIdentifier}", this, cancellationToken);            
        }

    }

    #endregion

    #region properties

    /// <summary>
    /// URI of the controller.
    /// </summary>
    public string ControllerUri { get; set; }

    /// <summary>
    /// Argument to send to the controller.
    /// </summary>
    public object ControllerQueryArg { get; set; }

    /// <summary>
    /// HTTP client.
    /// </summary>
    public HttpClient HttpClient { get; set; }

    #endregion

    #region methods

    private IQueryDataBuilder GetQueryDataBuilder()
    {
        Type argumentType = ControllerQueryArg?.GetType() ?? typeof(object);
        Type requestType = typeof(QueryDataBuilder<>).MakeGenericType(typeof(T), argumentType);
        return (IQueryDataBuilder)Activator.CreateInstance(requestType, new object[] { ControllerUri, ControllerQueryArg });
    }

    ///<inheritdoc/>
    public override async Task<IEnumerable<TValue>> GetSelectorValuesAsync<TValue>(Expression<Func<T, TValue>> filterExpression, CancellationToken cancellationToken = default)
    {
        if (filterExpression?.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"The member expression cannot be found ({filterExpression}).", nameof(filterExpression));
        }
        string propertyName = memberExpression.Member.Name;
        IQueryDataBuilder request = GetQueryDataBuilder();
        request.Sorts = new List<SortOption>() { new SortOption(filterExpression) };
        request.DistinctProperty = propertyName;
        DataQueryResponse<TValue> dataQueryResponse = await request.PostAsync<TValue>(HttpClient, cancellationToken);
        return dataQueryResponse.Data;
    }

    ///<inheritdoc/>
    public override async Task<DataPage<T>> GetDataPageAsync(IEnumerable<string> requiredFields, CancellationToken cancellationToken = default)
    {
        int totalResult;
        bool includeCount = PageSize > 0 && AllowCount;
        bool oneMore = PageSize > 0 && !AllowCount;
        int pageCount;
        bool isLastPage;

        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }
        // Initialize request
        IQueryDataBuilder request = GetQueryDataBuilder();
        request.Sorts = SortOptions;
        request.Filters = ModelFilter?.Filters;
        request.CurrentPage = CurrentPage;
        request.PageSize = PageSize;
        request.TotalCount = includeCount;
        request.Calculations = CalculationOptions;
        request.VisibleProperties = requiredFields;
        DataQueryResponse<T> dataQueryResponse = await request.PostAsync<T>(HttpClient, cancellationToken);
        if (PageSize > 0)
        {
            totalResult = dataQueryResponse.Count;
            if (totalResult > -1)
            {
                pageCount = GetPageCount(totalResult);
                // Check if the current page is after the last page
                while (CurrentPage > pageCount)
                {
                    CurrentPage = pageCount;
                    request.CurrentPage = CurrentPage;
                    dataQueryResponse = await request.PostAsync<T>(HttpClient, cancellationToken);
                    totalResult = dataQueryResponse.Count;
                    pageCount = GetPageCount(totalResult);
                }
                // Detect if we are on the last page.
                isLastPage = CurrentPage == pageCount;
            }
            else
            {
                // The page count is indeterminate
                pageCount = 0;
                // Check if the current page is after the last page
                while (CurrentPage > 1 && dataQueryResponse.Count == 0)
                {
                    CurrentPage--;
                    request.CurrentPage = CurrentPage;
                    dataQueryResponse = await request.PostAsync<T>(HttpClient, cancellationToken);
                }
                // Detect the last page (and remove the detection extra line)
                isLastPage = oneMore && DetectLastPage(dataQueryResponse.Data.ToList());
            }
        }
        else
        {
            // The page count is indeterminate
            pageCount = 0;
            isLastPage = true;
            totalResult = dataQueryResponse.Count;
        }
        return new DataPage<T>(CurrentPage, dataQueryResponse.Data, PageSize,
            isLastPage, pageCount, totalResult, dataQueryResponse.Calculations);
    }

    /// <summary>
    /// Gets the calculation for each columns.
    /// </summary>        
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<Dictionary<string, object>> GetCalculationAsync(CancellationToken cancellationToken = default)
    {
        if (CalculationOptions is null || !CalculationOptions.Any())
        {
            return null;
        }
        IQueryDataBuilder request = GetQueryDataBuilder();
        request.Filters = ModelFilter?.Filters;
        request.Calculations = CalculationOptions;
        request.CalculationOnly = true;
        DataQueryResponse<T> dataQueryResponse = await request.PostAsync<T>(HttpClient, cancellationToken);
        return dataQueryResponse.Calculations;
    }

    #endregion
}
