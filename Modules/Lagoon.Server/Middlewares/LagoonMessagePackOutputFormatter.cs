using MessagePack;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Lagoon.Server.Middlewares;

/// <summary>
/// Custom OutputFormatter for MessagePack.
/// Not using the built-in OutputFormatter provided by the library because of the poor management of collections by the MessagePack library.
/// This Output formatter will transform all generic enumerable type (IQueryable&lt;T&gt;, WhereListIterator&lt;T&gt;, ...) into IEnumerable&lt;T&gt; before serialization with MessagePack
/// </summary>
public class LagoonMessagePackOutputFormatter : OutputFormatter
{

    #region fields

    // MessagePack content-type used in response header by the OutputFormatter
    private const string ContentType = "application/x-msgpack";

    // MessagePack options
    private readonly MessagePackSerializerOptions _options;

    // Keep a cache of already created type
    private static Dictionary<string, Type> _instancesTypes = new Dictionary<string, Type>();

    // The base type used to iterate over a collection
    private static Type _genericEnumerableType = typeof(IEnumerable<>);

    #endregion

    #region Initialization

    /// <summary>
    /// Build a new <see cref="OutputFormatter"/> for MessagePack
    /// </summary>
    public LagoonMessagePackOutputFormatter() : this(null)
    {
    }

    /// <summary>
    /// Build a new <see cref="OutputFormatter"/> for MessagePack
    /// </summary>
    /// <param name="options">MessagePack Serializer Options</param>
    public LagoonMessagePackOutputFormatter(MessagePackSerializerOptions options)
    {
        _options = options;
        SupportedMediaTypes.Add(ContentType);
    }

    #endregion

    #region OutputFormatter implementation

    /// <summary>
    /// Write the serialized data to the response
    /// </summary>
    /// <param name="context">OutputFormatterWriteContext</param>
    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        if (context.Object == null)
        {
            var writer = context.HttpContext.Response.BodyWriter;
            if (writer == null)
            {
                context.HttpContext.Response.Body.WriteByte(MessagePackCode.Nil);
                return Task.CompletedTask;
            }
            var span = writer.GetSpan(1);
            span[0] = MessagePackCode.Nil;
            writer.Advance(1);
            return writer.FlushAsync().AsTask();
        }
        else
        {
            var objectType = context.ObjectType == null || context.ObjectType == typeof(object) ? context.Object.GetType() : context.ObjectType;
            if (IsIterableType(context.ObjectType))
            {
                objectType = GetOrCreate(context.ObjectType);
            }
            var writer = context.HttpContext.Response.BodyWriter;
            if (writer == null)
            {
                return MessagePackSerializer.SerializeAsync(objectType, context.HttpContext.Response.Body, context.Object, this._options, context.HttpContext.RequestAborted);
            }
            MessagePackSerializer.Serialize(objectType, writer, context.Object, this._options, context.HttpContext.RequestAborted);
            return writer.FlushAsync().AsTask();
        }
    }

    #endregion

    #region privates methods

    /// <summary>
    /// Check if a type is in cache and if so return it.
    /// Otherwise create a new runtime type, add it to the cache and return it.
    /// </summary>
    /// <param name="objectType">Type for which we want an IEnumerable(objectType)</param>
    /// <returns></returns>
    private static Type GetOrCreate(Type objectType)
    {
        Type arg = objectType.GenericTypeArguments[0];
        if (_instancesTypes.TryGetValue(arg.FullName, out var typeFromCache))
        {
            return typeFromCache;
        }
        var type = _genericEnumerableType.MakeGenericType(new[] { objectType.GenericTypeArguments[0] });
        _instancesTypes.Add(arg.FullName, type);
        return type;
    }

    /// <summary>
    /// Return true if the type can be safely convert to an IEnumerable
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns></returns>
    private static bool IsIterableType(Type type)
    {
        return type.IsGenericType && type.GenericTypeArguments.Length == 1 && type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == _genericEnumerableType);
    } 

    #endregion

}
