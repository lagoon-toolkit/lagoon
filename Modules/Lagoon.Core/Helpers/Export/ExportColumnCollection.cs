namespace Lagoon.Helpers;

/// <summary>
/// Definition of column list for an export.
/// </summary>
public class ExportColumnCollection<T> : List<IExportColumn<T>>
{

    #region constructors

    /// <summary>
    /// Create a new instance.
    /// </summary>
    public ExportColumnCollection() { }


    /// <summary>
    /// Create a new instance.
    /// </summary>
    public ExportColumnCollection(IEnumerable<IExportColumn<T>> columns)
    {
        AddRange(columns);
    }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="allProperties">Export all properties.</param>
    public ExportColumnCollection(bool allProperties) : this(allProperties, null, null) { }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="columns">Ordered list of column field names.</param>
    public ExportColumnCollection(IEnumerable<string> columns) : this(false, columns, null) { }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="getColumnTitle">Title for a field name.</param>
    /// <param name="columns">Ordered list of column field names.</param>
    public ExportColumnCollection(IEnumerable<string> columns, Func<string, string> getColumnTitle)
        : this(false, columns, getColumnTitle) { }

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="allProperties">Export all properties.</param>
    /// <param name="columns">Ordered list of column field names.</param>
    /// <param name="getColumnTitle">Title for a field name.</param>
    private ExportColumnCollection(bool allProperties, IEnumerable<string> columns, Func<string, string> getColumnTitle)
    {
        // Define the sub function
        string GetColumnTitle(string field) => getColumnTitle is not null ? getColumnTitle(field) : null;
        // Load the column field list
        List<string> fields = new();
        if (columns is null)
        {
            if(allProperties)
            {
                // Export all property from the dictionnary
                fields.AddRange(GetExportableProperties(typeof(T)));
            }
        }
        else
        {
            // Export only chosen columns
            fields.AddRange(columns.Where(x => !string.IsNullOrEmpty(x)));
        }
        // Add the titles to the column names
        foreach (string field in fields)
        {
            Add(field, GetColumnTitle(field));
        }
    }

    #endregion

    #region method

    /// <summary>
    /// Add a new column to the collection.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="getValueLambda">The method returning the value for an item.</param>
    /// <param name="groupTitle">The group title (if any)</param>
    public void Add<TValue>(string columnTitle, Func<T,TValue> getValueLambda, string groupTitle = null)
    {
        Add(new ExportColumn<T, TValue>(columnTitle, getValueLambda, null, groupTitle));
    }

    /// <summary>
    /// Add a new column to the collection.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="setValueLambda">The method setting the value to an item.</param>
    /// <param name="groupTitle">The group title (if any)</param>
    public void Add<TValue>(string columnTitle, Action<T, TValue> setValueLambda, string groupTitle = null)
    {
        Add(new ExportColumn<T, TValue>(columnTitle, null, setValueLambda, groupTitle));
    }

    /// <summary>
    /// Add a new column to the collection.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="getValueLambda">The method returning the value for an item.</param>
    /// <param name="setValueLambda">The method setting the value to an item.</param>
    /// <param name="groupTitle">The group title (if any)</param>
    public void Add<TValue>(string columnTitle, Func<T, TValue> getValueLambda, Action<T, TValue> setValueLambda, string groupTitle = null)
    {
        Add(new ExportColumn<T, TValue>(columnTitle, getValueLambda, setValueLambda, groupTitle));
    }

    /// <summary>
    /// Add a new column to the collection.
    /// </summary>
    /// <param name="propertyName">The property name. ( Ex: "Value" or "Child.Value")</param>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="groupTitle">The group title.</param>
    public void Add(string propertyName, string columnTitle = null, string groupTitle = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        string[] property = propertyName.Split('.');
        if (property.Length > 2)
        {
            throw new ArgumentException("The \"propertyName\" parameter supports only one sub level.", nameof(propertyName));
        }
        string subPropertyName = property.Length > 1 ? property[1] : null;
        propertyName = property[0];
        // Create the lamda expression
        ParameterExpression itemParameter = Expression.Parameter(typeof(T), "i");
        GetPropertyGetSetValueExpression(itemParameter, propertyName,
            out ParameterExpression valueParameter, 
            out MethodCallExpression getLambdaBody, out MethodCallExpression setLambdaBody);
        if (!string.IsNullOrEmpty(subPropertyName))
        {
            GetPropertyGetSetValueExpression(getLambdaBody, subPropertyName, 
                out valueParameter,
                out getLambdaBody, out setLambdaBody);
        }
        Add((IExportColumn<T>)Activator.CreateInstance(typeof(ExportColumn<,>)
            .MakeGenericType(typeof(T), getLambdaBody.Type), 
            columnTitle ?? propertyName, itemParameter, valueParameter, getLambdaBody, setLambdaBody, groupTitle));
    }

    /// <summary>
    /// Get the property call value expression.
    /// </summary>
    /// <param name="item">Source expression</param>
    /// <param name="valueParameter">The value parameter.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="getValueExpression">Get property value expression.</param>
    /// <param name="setValueExpression">Set property value expression.</param>
    private static void GetPropertyGetSetValueExpression(Expression item, string propertyName,
        out ParameterExpression valueParameter,
        out MethodCallExpression getValueExpression, out MethodCallExpression setValueExpression)
    {
        const BindingFlags BINDING_FLAGS = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

        PropertyInfo propertyInfo = item.Type.GetProperty(propertyName, BINDING_FLAGS);
        valueParameter = Expression.Parameter(propertyInfo.PropertyType, "v");
        getValueExpression = Expression.Call(item, propertyInfo.GetGetMethod());
        setValueExpression = Expression.Call(item, propertyInfo.GetSetMethod(), valueParameter);
    }

    /// <summary>
    /// Return all the public property name for a type.
    /// </summary>
    /// <param name="itemType">The type to browse.</param>
    /// <returns>The list of public properties.</returns>
    private static IEnumerable<string> GetExportableProperties(Type itemType)
    {
        return itemType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.GetCustomAttribute<ExportIgnoreAttribute>() is null).Select(x => x.Name);
    }

    #endregion

}