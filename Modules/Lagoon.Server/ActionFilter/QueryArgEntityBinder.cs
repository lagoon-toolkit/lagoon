using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lagoon.Server.Controllers;

/// <summary>
/// Binder for remote data loader with complementary data
/// </summary>
internal class QueryArgEntityBinder : IModelBinder
{

    ///<inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string value = bindingContext.ValueProvider.GetValue("value").FirstValue;
        if (value is not null)
        {
            // Deserialize value 
            Type bindedType = bindingContext.ModelType.GenericTypeArguments[0];
            object parsedData = JsonSerializer.Deserialize(
                    value,
                    bindedType,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
                );
            Type genericType = typeof(QueryArg<>).MakeGenericType(bindedType);
            object remoteData = Activator.CreateInstance(genericType, parsedData);
            bindingContext.Result = ModelBindingResult.Success(remoteData);
        }
        return Task.CompletedTask;
    }

}