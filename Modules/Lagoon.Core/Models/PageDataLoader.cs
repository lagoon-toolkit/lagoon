namespace Lagoon.Helpers.Data;

/// <summary>
/// Help to sort, filter and paging data
/// </summary>
/// <typeparam name="T"></typeparam>
public static class PageDataLoader<T>
{

    /// <summary>
    /// Sort, filter paging data
    /// </summary>
    /// <param name="query">Query</param>        
    /// <param name="modelFilter">filters parameters</param>
    /// <param name="sortOptions">sort parameters</param>
    public static IQueryable<T> ApplyQueryFilterAndSort(IQueryable<T> query, ModelFilter<T> modelFilter, IEnumerable<SortOption> sortOptions)
    {
        // Filters
        query = query.ApplyModelFilter(modelFilter);
        // Sorting
        query = ApplyQuerySort(query, sortOptions);
        return query;
    }

    /// <summary>
    /// Sort, filter paging data
    /// </summary>
    /// <param name="query">Query</param>        
    /// <param name="sortOptions">sort parameters</param>
    public static IQueryable<T> ApplyQuerySort(IQueryable<T> query, IEnumerable<SortOption> sortOptions)
    {
        // Sorting
        if (sortOptions is not null)
        {
            IOrderedQueryable<T> sorted = null;
            foreach (SortOption sortOption in sortOptions)
            {
                if (sorted is null)
                {
                    string methodName = sortOption.Direction == DataSortDirection.Asc ? "OrderBy" : "OrderByDescending";
                    sorted = ApplyOrder(query, methodName, sortOption.ParameterizedValueExpression);
                }
                else
                {
                    string methodName = sortOption.Direction == DataSortDirection.Asc ? "ThenBy" : "ThenByDescending";
                    sorted = ApplyOrder(sorted, methodName, sortOption.ParameterizedValueExpression);
                }
            }
            if (sorted is not null)
            {
                query = sorted;
            }
        }
        return query;
    }

    /// <summary>
    /// Add pagination to query
    /// </summary>
    /// <param name="query"></param>
    /// <param name="currentPage"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static IQueryable<T> ApplyQueryPagination(IQueryable<T> query, int currentPage, int pageSize)
    {
        return pageSize < 1 ? query : query.Skip((currentPage - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Apply sort parameter to query
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="methodName">The orderby method name.</param>
    /// <param name="parameterizedValueExpression">Expression representing the function that return the wanted value with the item as parameter.</param>
    /// <returns>The new query.</returns>
    private static IOrderedQueryable<T> ApplyOrder(IQueryable<T> query, string methodName, LambdaExpression parameterizedValueExpression)
    {
        return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
            method => method.Name == methodName
                    && method.IsGenericMethodDefinition
                    && method.GetGenericArguments().Length == 2
                    && method.GetParameters().Length == 2)
            .MakeGenericMethod(parameterizedValueExpression.Parameters[0].Type, parameterizedValueExpression.ReturnType)
            .Invoke(null, new object[] { query, parameterizedValueExpression });
    }

    /// <summary>
    /// Return expression for calculations 
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Expression<Func<IGrouping<int, T>, Dictionary<string, object>>> ApplyCalculations(CalculationOption[] options)
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

}