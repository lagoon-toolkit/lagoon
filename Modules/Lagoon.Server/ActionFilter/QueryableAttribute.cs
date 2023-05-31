using Lagoon.Shared.Model;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;
using System.Linq.Dynamic.Core;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Attribute to manage filters and sorting
/// </summary>
public class QueryableAttribute : ActionFilterAttribute
{
    #region fields

    /// <summary>
    /// Data query request identify
    /// </summary>
    private bool _isDataQuery;

    /// <summary>
    /// Data query request
    /// </summary>
    private IDataQueryRequest _request;

    #endregion

    ///<inheritdoc/>        
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if the request id an data query request
        _isDataQuery = context.HttpContext.Request.Headers.ContainsKey(IDataQueryRequest.HeaderQueryIdentifier);
        if (_isDataQuery)
        {
            // Get filters, sorts, pagination, calculation object            
            ParameterDescriptor parameter = context.ActionDescriptor.Parameters.FirstOrDefault(p =>
            {
                return p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(QueryArg<>);
            });
            Type parameterType = parameter?.ParameterType?.GenericTypeArguments[0] ?? typeof(object);
            Type dataQueryType = typeof(DataQueryRequest<>).MakeGenericType(parameterType);
            context.HttpContext.Request.Body.Position = 0;
            _request = (IDataQueryRequest)await context.HttpContext.Request.ReadFromJsonAsync(dataQueryType, context.HttpContext.RequestAborted);
            // Pass controller argument
            if (_request.ControllerQueryArgValue is not null && parameter is not null)
            {
                context.ActionArguments[parameter.Name] =
                    Activator.CreateInstance(parameter.ParameterType, _request.ControllerQueryArgValue);
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }

    ///<inheritdoc/>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (_isDataQuery)
        {
            // Modify result to apply filter, sort and pagination
            if (context.Result is ObjectResult objectResult)
            {
                objectResult.Formatters.Add(ResponseFactoryCache.Formatter);
                IQueryable queryableValue = objectResult.Value is IQueryable qv
                    ? qv
                    : objectResult.Value is IEnumerable ev
                        ? ev.AsQueryable()
                        : throw new InvalidOperationException("Unable to detect the model type.");
                IResponseFactory responseFactory = (IResponseFactory)context.HttpContext.RequestServices.GetService(
                    typeof(ResponseFactory<>)
                    .MakeGenericType(queryableValue.ElementType));
                objectResult.Value = responseFactory.GetResponse(queryableValue, _request);
            }
        }
        base.OnActionExecuted(context);
    }

}
