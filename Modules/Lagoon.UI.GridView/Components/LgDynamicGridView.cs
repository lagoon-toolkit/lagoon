using Lagoon.Core.Helpers;
using Lagoon.UI.GridView.Helpers;

namespace Lagoon.UI.Components;

/// <summary>
/// Defines a grid view using a dynamic data source.
/// </summary>
public class LgDynamicGridView : LgGridView<Dictionary<string, ObjectWrapper>>
{

    #region private classes

    /// <summary>
    /// Comparer for selection based to KeyField property
    /// </summary>        
    private class DynamicSelectionComparer : IEqualityComparer<Dictionary<string, ObjectWrapper>>
    {
        private string _keyField;

        public DynamicSelectionComparer(string keyField)
        {
            _keyField = keyField;
        }

        public bool Equals(Dictionary<string, ObjectWrapper> x, Dictionary<string, ObjectWrapper> y)
        {
            if (x is null || y is null)
            {
                return x == y;
            }
            bool hasPropX = x.TryGetValue(_keyField, out ObjectWrapper wrapperX);
            bool hasPropY = y.TryGetValue(_keyField, out ObjectWrapper wrapperY);
            if (!hasPropX || !hasPropY)
            {
                return hasPropX == hasPropY;
            }
            if (wrapperX.Value is null)
            {
                return wrapperY is null;
            }
            else
            {
                return wrapperX.Value.Equals(wrapperY.Value);
            }
        }

        public int GetHashCode(Dictionary<string, ObjectWrapper> obj)
        {
            if (obj is not null && obj.TryGetValue(_keyField, out ObjectWrapper wrapper))
            {
                return wrapper.Value?.GetHashCode() ?? 0;
            }
            else
            {
                return 0;
            }
        }
    }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets the dynamic data source.
    /// </summary>
    [Parameter]
    public DynamicDataSource DynamicDataSource { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Items != null)
        {
            throw new NotSupportedException($"The \"{nameof(DynamicDataSource)}\" must be used and not the \"{nameof(Items)}\" parameter for the \"{nameof(LgDynamicGridView)}\" component.");
        }
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(Items, DynamicDataSource))
        {
            Items = DynamicDataSource;
            ResetItemModel();
        }
        base.OnParametersSet();
    }

    ///<inheritdoc/>
    internal override List<LgGridValueDefinition<Dictionary<string, ObjectWrapper>>> GetFieldDefinitionList()
    {
        GridColumnType columnType;
        List<LgGridValueDefinition<Dictionary<string, ObjectWrapper>>> list = new();
        foreach (KeyValuePair<string, Type> entry in DynamicDataSource.Properties)
        {
            columnType = GridViewHelper.GetColumnType(entry.Value);
            // Detect if a column type is defined for the property type
            if (columnType != GridColumnType.None)
            {
                list.Add(new LgDynamicValueDefinition(entry.Key, entry.Value, columnType));
            }
        }
        return list;
    }

    ///<inheritdoc/>
    internal override Type OnGetCellValueType(Type columnValueType, LambdaExpression valueExpression)
    {
        if (valueExpression?.ReturnType == typeof(ObjectWrapper))
        {
            string valueName = valueExpression.ToString().Split('"')[1];
            throw new InvalidOperationException($"You must replace ...[\"{valueName}\"]... by ...[\"{valueName}\"].Value... in the GridView columns definition.");
        }
        string propertyName = GetDataSourcePropertyName(valueExpression);
        if (propertyName is not null && DynamicDataSource is not null && DynamicDataSource.Properties.TryGetValue(propertyName, out Type type))
        {
            return type;
        }
        else
        {
            return columnValueType;
        }
    }

    ///<inheritdoc/>
    internal override string BuildColumnKey(LambdaExpression valueExpression, LambdaExpression parameterizedValueExpression)
    {
        return GetDataSourcePropertyName(valueExpression) ?? base.BuildColumnKey(valueExpression, parameterizedValueExpression);
    }

    ///<inheritdoc/>
    internal override Dictionary<string, ObjectWrapper> CreateItemModel()
    {
        return DynamicDataSource.Properties.ToDictionary(d => d.Key,
            d => new ObjectWrapper(GetDefaultOrEmptyValue(d.Value)));
    }

    /// <summary>
    /// Get the name of the "Key" field in the DynamicDataSource.
    /// </summary>
    /// <param name="expression">Expression of bound value.</param>
    /// <returns>The name of the "Key" field in the DynamicDataSource.</returns>
    private static string GetDataSourcePropertyName(LambdaExpression expression)
    {
        // We try to match an expression of the form @DataGridData["Property"].Value.
        if (expression is not null && expression.Body is MemberExpression memberExpression)
        {
            Expression firstArgument = null;
            if (memberExpression.Expression is MethodCallExpression methodCallExpression && methodCallExpression.Method.Name == "get_Item")
            {
                // Extract field name of expression like : x.get_Item("Age").Value
                firstArgument = methodCallExpression.Arguments[0];
            }
            else if (memberExpression.Expression is IndexExpression indexExpression)
            {
                // Extract field name of expression like : x["Age"].Value
                firstArgument = indexExpression.Arguments[0];
            }
            // Evaluate the key value
            if (firstArgument is not null)
            {
                if (firstArgument.NodeType is ExpressionType.Constant)
                {
                    return (string)(firstArgument as ConstantExpression).Value;
                }
                else if (firstArgument.NodeType is ExpressionType.MemberAccess)
                {
                    MemberExpression member = firstArgument as MemberExpression;
                    UnaryExpression objectMember = Expression.Convert(member, typeof(object));
                    Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    Func<object> getter = getterLambda.Compile();
                    return getter().ToString();
                }
            }
        }
        return null;
    }

    private static object GetDefaultOrEmptyValue(Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            // We can't return a nullable type because of the CLR boxing rule so we just return an instance of the underlying type.
            // It's taken care of in the PageDataLoader class.
            return Activator.CreateInstance(underlyingType);
        }
        else if (type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null)
        {
            return Activator.CreateInstance(type);
        }
        else if (type == typeof(string))
        {
            return string.Empty;
        }
        throw new NotSupportedException($"The type {type} is currently not supported as a column type.");
    }

    ///<inheritdoc/>
    protected override IEqualityComparer<Dictionary<string, ObjectWrapper>> CreateKeyFieldSelectionComparer()
    {
        return new DynamicSelectionComparer(KeyField);
    }

    #endregion
}