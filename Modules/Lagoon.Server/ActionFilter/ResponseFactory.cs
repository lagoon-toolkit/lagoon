using Lagoon.Helpers.Data;
using Lagoon.Model.Collation;
using Lagoon.Server.Extensions;
using Lagoon.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Cache = Lagoon.Server.Controllers.ResponseFactoryCache;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Response class
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResponseFactory<T> : IResponseFactory
{

    private ILgApplication _app;

    /// <summary>
    /// Constructor
    /// </summary>
    public ResponseFactory(ILgApplication app)
    {
        _app = app;
    }

    ///<inheritdoc/>
    public object GetResponse(IQueryable queryableValue, IDataQueryRequest request)
    {
        // Get returned type
        Type modelType = typeof(T);
        IQueryable<T> query = (IQueryable<T>)queryableValue;
        // Apply filters and sort
        query = ApplyFiltersSorts(query, request);
        // Check if the query can be translated by "Linq To Entities", else we work with a List<T> in memory
        if (!query.IsValidQuery())
        {
            _app.TraceWarning("Filtering or sorting options cannot be executed in the DbContext and will be executed in memory.");
            query = ApplyFiltersSorts(((IQueryable<T>)queryableValue).ToList().AsQueryable(), request);
        }
        // Get number of lines if needed
        int count = 0;
        if (request.TotalCount)
        {
            count = query.Count();
        }
        // Calculations
        Dictionary<string, object> calculationResults = null;
        if (request.Calculations is not null && request.Calculations.Any())
        {
            calculationResults = GetCalculations(query, request.Calculations);
        }
        // Apply paging                                        
        query = PageDataLoader<T>.ApplyQueryPagination(query, request.CurrentPage, request.PageSize);
        // Select properties
        List<string> selects = null;
        if (request.DistinctProperty is not null)
        {
            // Build query to return only the properties values                        
            PropertyInfo propInfo = modelType.GetProperty(request.DistinctProperty);
            Type responseType = propInfo.PropertyType;
            IQueryable distinctQuery = (IQueryable)Cache.QueryableSelect.MakeGenericMethod(modelType, responseType).Invoke(null, new object[] {
                        query,
                        GetPropertyValueLambda(request.DistinctProperty, modelType)
                    });
            MethodInfo distinctMethod = Cache.QueryableDistinct.MakeGenericMethod(responseType);
            distinctQuery = (IOrderedQueryable)distinctMethod.Invoke(null, new object[]
            {
                    distinctQuery
            });
            return Activator.CreateInstance(typeof(DataQueryResponse<>).MakeGenericType(responseType),
                new object[] { !request.CalculationOnly ? distinctQuery : null, calculationResults, count });
        }
        else if (request.VisibleProperties is not null && request.VisibleProperties.Any())
        {
            // Build class with only the selected properties
            selects = request.VisibleProperties.ToList();
            // Create select with only select fields filled
            Dictionary<string, PropertyInfo> properties = modelType.GetProperties()
                .Where(p => selects.Contains(p.Name)).ToDictionary(k => k.Name);
            ParameterExpression sourceItem = Expression.Parameter(modelType, "t");
            IEnumerable<MemberBinding> bindings = properties.Select(p =>
                Expression.Bind(p.Value, Expression.Property(sourceItem, properties[p.Key]))).OfType<MemberBinding>();
            // Create select selector with active properties
            Expression selector = Expression.Lambda(Expression.MemberInit(
                Expression.New(modelType.GetConstructor(Type.EmptyTypes)), bindings), sourceItem);
            query = (IQueryable<T>)query.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new Type[] { query.ElementType, modelType },
                Expression.Constant(query), selector));
        }
        return new DataQueryResponse<T>(!request.CalculationOnly ? query : null, calculationResults, count);
    }

    /// <summary>
    /// Apply sorts and filters without the pagination.
    /// </summary>
    private static IQueryable<T> ApplyFiltersSorts(IQueryable<T> query, IDataQueryRequest request)
    {
        // Detects if the query will be converted to an SQL request or not
        DbContext db = query.TryGetDbContext();
        FilterWhereContext context = db is IWithCollation dbc ? new(true, dbc.CollationType.HasValue) : new(db is not null, false);
        return PageDataLoader<T>.ApplyQueryFilterAndSort(query, GetModelFilter(request.Filters, context), GetSortOptions(request.Sorts));
    }

    /// <summary>
    /// Return calculation results
    /// </summary>
    private static Dictionary<string, object> GetCalculations(IQueryable query, IEnumerable<CalculationOption> calculationOptions)
    {
        Dictionary<string, object> results = null;
        Type modelType = typeof(T);
        foreach (CalculationOption calcul in calculationOptions)
        {
            Type propertyType = modelType.GetProperty(calcul.Field)?.PropertyType;
            object calculationResult = null;
            MethodInfo calculMethod = calcul.CalculationType switch
            {
                DataCalculationType.Average => Cache.QueryableAverage.MakeGenericMethod(modelType), // TODO Fred : PropertType ?
                DataCalculationType.Count => Cache.QueryableCountDistinct.MakeGenericMethod(modelType, propertyType),
                DataCalculationType.Max => Cache.QueryableMax.MakeGenericMethod(modelType, propertyType),
                DataCalculationType.Min => Cache.QueryableMin.MakeGenericMethod(modelType, propertyType),
                DataCalculationType.Sum => Cache.QueryableSum.MakeGenericMethod(modelType), // TODO Fred : PropertType ?
                _ => null,
            };
            if (calculMethod is not null)
            {
                try
                {
                    // Calculate column operation
                    calculationResult = calculMethod.Invoke(null, new object[]
                    {
                        query,
                        GetPropertyValueLambda(calcul.Field, modelType)
                    });
                }
                catch (TargetInvocationException)
                {
                    // sourQuery not have result
                }
                if (calculationResult is not null)
                {
                    results ??= new();
                    results.Add(calcul.Field, calculationResult);
                }
            }
        }
        return results;
    }

    /// <summary>
    /// Return list of the sorts expression
    /// </summary>
    /// <returns>The sort options</returns>
    private static IEnumerable<SortOption> GetSortOptions(IEnumerable<SortOption> sortOptions)
    {
        return sortOptions?.Select(s =>
        {
            s.ParameterizedValueExpression = UnStringifyExpressionLambda(s.StringifiedValueExpression);
            return s;
        });
    }

    /// <summary>
    /// Return filters parameters
    /// </summary>
    /// <param name="filters">The filters.</param>
    /// <param name="context">The context for the where condition creation (EFCore or IEnumerable in memory).</param>
    /// <returns>The model filter.</returns>
    private static ModelFilter<T> GetModelFilter(IEnumerable<ValueFilter> filters, FilterWhereContext context)
    {
        if (filters is null)
        {
            return default;
        }
        ModelFilter<T> modelFilter = new(context);
        foreach (ValueFilter valueFilter in filters)
        {
            modelFilter.AddFilterUnsafe(UnStringifyExpressionLambda(valueFilter.StringifiedSelector), valueFilter.Filter);
        }
        return modelFilter;
    }

    /// <summary>
    /// Transform expression.
    /// </summary>
    private static LambdaExpression UnStringifyExpressionLambda(string expression)
    {
        // Get parameter name
        string[] parameters = expression.Split("=>");
        if (parameters.Length != 2)
        {
            throw new ArgumentException("Sort expression is not correct.");
        }
        // Generate lambda from stringified expression
        ParameterExpression parameter = Expression.Parameter(typeof(T), parameters[0].Trim());
        ParsingConfig config = new();
        //config.CustomTypeProvider 
        //config.ExpressionPromoter 
        LambdaExpression lambdaExpression = DynamicExpressionParser.ParseLambda(config, new ParameterExpression[] { parameter }, null, expression);
        return lambdaExpression;
    }

    /// <summary>
    /// Get lambda expression to access to property by it's name
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="objectType"></param>
    /// <returns></returns>
    private static LambdaExpression GetPropertyValueLambda(string propertyName, Type objectType)
    {
        ParameterExpression parameter = Expression.Parameter(objectType);
        return Expression.Lambda(Expression.PropertyOrField(parameter, propertyName), parameter);
    }

}
