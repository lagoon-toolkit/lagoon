using Lagoon.Core.Helpers;

namespace Lagoon.UI.Components.Internal;

internal class LgDynamicValueDefinition : LgGridValueDefinition<Dictionary<string, ObjectWrapper>>
{

    #region fields

    private PropertyInfo _indexer;
    private PropertyInfo _valueProperty;

    #endregion

    #region properties

    public Type ValueType { get; }

    public override Func<Dictionary<string, ObjectWrapper>, object> GetValue { get; }

    #endregion

    #region constructor

    public LgDynamicValueDefinition(string name, Type valueType, GridColumnType columnType) : base(name, typeof(object), columnType)
    {
        ValueType = valueType;
        _indexer = (from p in typeof(Dictionary<string, ObjectWrapper>).GetDefaultMembers().OfType<PropertyInfo>()
                    let q = p.GetIndexParameters()
                    where q.Length == 1
                    select p).Single();
        _valueProperty = typeof(ObjectWrapper).GetProperty(nameof(ObjectWrapper.Value));
        GetValue = item => item[Name].Value;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override void AddValueChangedAttribute(RenderTreeBuilder builder, int sequence, object receiver, Dictionary<string, ObjectWrapper> item)
    {
        EventCallback<object> callback = EventCallback.Factory.Create<object>(receiver, __value => item[Name].Value = __value);
        builder.AddAttribute(sequence, nameof(LgGridColumn<object>.ValueChanged), callback);
    }

    public override LambdaExpression GetValueExpression(Dictionary<string, ObjectWrapper> item)
    {
        IndexExpression indexedProperty = Expression.Property(Expression.Constant(item), _indexer, Expression.Constant(Name));
        MemberExpression wrappedProperty = Expression.Property(indexedProperty, _valueProperty.Name);
        return Expression.Lambda<Func<object>>(wrappedProperty);
    }

    #endregion

}