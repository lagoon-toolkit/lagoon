namespace Lagoon.Helpers.Data;

/// <summary>
/// Helper to gets pagged item list.
/// </summary>
public abstract class CustomDataPageLoader<T>
{

    #region properties

    /// <summary>
    /// Gets or sets if we must retrieve the total count of items
    /// </summary>
    public bool AllowCount { get; set; }

    /// <summary>
    /// Gets or sets index of the current page
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets number of rows by page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets sorts parameters
    /// </summary>
    public IEnumerable<SortOption> SortOptions { get; set; }

    /// <summary>
    /// Gets or sets the filter for the model.
    /// </summary>
    public ModelFilter<T> ModelFilter { get; set; }

    /// <summary>
    /// Gets or sets calculation parameters
    /// </summary>
    public IEnumerable<CalculationOption> CalculationOptions { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Get all the distinct values for the specific expresssion.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="filterExpression">The filter expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All the distinct values for the specific expresssion.</returns>
    public abstract Task<IEnumerable<TValue>> GetSelectorValuesAsync<TValue>(Expression<Func<T, TValue>> filterExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the data of the current page.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The data of the current page.</returns>
    public Task<DataPage<T>> GetDataPageAsync(CancellationToken cancellationToken = default)
    {
        return GetDataPageAsync(null, cancellationToken);
    }

    /// <summary>
    /// Gets the data of the current page.
    /// </summary>
    /// <param name="visibleFields">Fields needed.</param>        
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The data of the current page.</returns>
    public abstract Task<DataPage<T>> GetDataPageAsync(IEnumerable<string> visibleFields, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the calculation for each columns.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The calculated values.</returns>
    public abstract Task<Dictionary<string, object>> GetCalculationAsync(CancellationToken cancellationToken = default);

    #endregion

    #region static methods

    /// <summary>
    /// Add pagination to query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="currentPage"></param>
    /// <param name="pageSize"></param>
    /// <param name="oneMore">Indicate to return on more item to detect the last page.</param>
    /// <returns></returns>
    protected static IQueryable<T> ApplyQueryPagination(IQueryable<T> query, int currentPage, int pageSize, bool oneMore = false)
    {
        return query.Skip((currentPage - 1) * pageSize).Take(oneMore ? pageSize + 1 : pageSize);
    }

    /// <summary>
    /// Return expression for calculations 
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected static Expression<Func<IGrouping<int, T>, Dictionary<string, object>>> ApplyCalculations(IEnumerable<CalculationOption> options)
    {
        ParameterExpression g = Expression.Parameter(typeof(IGrouping<int, T>));
        List<Expression> body = new();
        ParameterExpression result = Expression.Variable(typeof(Dictionary<string, object>));
        MethodInfo add = typeof(Dictionary<string, object>).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(object) }, null);
        body.Add(Expression.Assign(result, Expression.New(typeof(Dictionary<string, object>))));
        // Create lambda with dictionary result
        foreach (CalculationOption option in options)
        {
            ConstantExpression key = Expression.Constant(option.Field);
            Expression calculation = Calculation(option.CalculationType, g, option.ParametrizedValueExpression);
            body.Add(Expression.Call(result, add, key, Expression.Convert(calculation, typeof(object))));

        }
        body.Add(result);
        BlockExpression block = Expression.Block(new[] { result }, body);
        // Create final lambda
        return Expression.Lambda<Func<IGrouping<int, T>, Dictionary<string, object>>>(block, g);
    }

    /// <summary>
    /// Return expression relative of the calculation
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="group"></param>
    /// <param name="parameterizedValueExpression">Expression representing the method to get the column value.</param>
    private static Expression Calculation(DataCalculationType operation, Expression group, LambdaExpression parameterizedValueExpression)
    {
        string calculationOp = Enum.GetName(typeof(DataCalculationType), operation);
        Type propertyType = parameterizedValueExpression.ReturnType;
        // Select return type following calculation
        Expression[] arguments = null;
        Type[] genericArgs = null;
        switch (operation)
        {
            case DataCalculationType.Count:
                // Count distinct                    
                MethodCallExpression selectField = Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Select),
                    new[] { typeof(T), propertyType },
                    group,
                    parameterizedValueExpression);
                genericArgs = new[] { propertyType };
                MethodCallExpression distinct = Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Distinct),
                    genericArgs,
                    selectField);
                arguments = new Expression[] { distinct };
                break;
            case DataCalculationType.Sum:
            case DataCalculationType.Average:
                genericArgs = new[] { typeof(T) };
                arguments = new[] { group, parameterizedValueExpression };
                break;
            default:
                genericArgs = new[] { typeof(T), propertyType };
                arguments = new[] { group, parameterizedValueExpression };
                break;
        }
        return Expression.Call(
                    typeof(Enumerable),
                    calculationOp,
                    genericArgs,
                    arguments);
    }

    /// <summary>
    /// Detect if the page is the last page when the list is loaded with one extra line.
    /// </summary>
    /// <param name="items">Items list.</param>
    /// <returns>The boolean </returns>
    protected bool DetectLastPage(List<T> items)
    {
        bool isLastPage = items.Count <= PageSize;
        // We remove the "OneMore" for last page detection
        if (items.Count > PageSize)
        {
            items.RemoveAt(PageSize);
        }
        return isLastPage;
    }

    /// <summary>
    /// Return the page count based on the total item count.
    /// </summary>
    /// <param name="totalCount">The total item count.</param>
    /// <returns>The pages count.</returns>
    protected int GetPageCount(int totalCount)
    {
        int pageCount = (int)Math.Ceiling((double)totalCount / PageSize);
        if (pageCount < 1)
        {
            pageCount = 1;
        }
        return pageCount;
    }

    #endregion

}
