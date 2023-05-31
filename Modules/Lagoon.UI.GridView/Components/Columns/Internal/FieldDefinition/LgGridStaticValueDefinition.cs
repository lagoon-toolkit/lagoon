namespace Lagoon.UI.Components.Internal;

internal class LgGridStaticValueDefinition<TItem, TValue> : LgGridValueDefinition<TItem>
{

    #region field

    private readonly PropertyInfo _propertyInfo;

    #endregion

    #region properties

    public override Func<TItem, object> GetValue { get; }

    #endregion

    #region constructor

    public LgGridStaticValueDefinition(PropertyInfo propertyInfo, GridColumnType columnType)
        : base(propertyInfo.Name, propertyInfo.PropertyType, columnType)
    {
        _propertyInfo = propertyInfo;
        GetValue = GetItemGetValueParameterizedFunc();
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override void AddValueChangedAttribute(RenderTreeBuilder builder, int sequence, object receiver, TItem item)
    {
        EventCallback<TValue> callback = EventCallback.Factory.Create(receiver, GetItemSetValueAction(item));
        builder.AddAttribute(sequence, nameof(LgGridColumn<object>.ValueChanged), callback);
    }

    private Func<TItem, object> GetItemGetValueParameterizedFunc()
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TItem), "x");
        MemberExpression itemProperty = Expression.Property(parameter, _propertyInfo);
        MethodCallExpression getPropertyBody = Expression.Call(itemProperty, _propertyInfo.GetGetMethod());
        return Expression.Lambda<Func<TItem, object>>(getPropertyBody).Compile();
    }

    public override LambdaExpression GetValueExpression(TItem item)
    {
        MemberExpression itemProperty = Expression.Property(Expression.Constant(item), _propertyInfo);
        MethodCallExpression getPropertyBody = Expression.Call(itemProperty, _propertyInfo.GetGetMethod());
        return Expression.Lambda<Func<TValue>>(getPropertyBody);
    }

    private Action<TValue> GetItemSetValueAction(TItem item)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TValue), "__value");
        MemberExpression itemProperty = Expression.Property(Expression.Constant(item), _propertyInfo);
        MethodCallExpression setPropertyBody = Expression.Call(itemProperty, _propertyInfo.GetSetMethod(), parameter);
        return Expression.Lambda<Action<TValue>>(setPropertyBody, parameter).Compile();
    }

    #endregion

}
