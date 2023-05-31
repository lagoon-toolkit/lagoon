namespace Lagoon.UI.Components.Internal;

internal abstract class LgGridValueDefinition<TItem>
{
    public string Name { get; }

    public abstract Func<TItem, object> GetValue { get; }

    public Type ColumnType { get; }

    public LgGridValueDefinition(string name, Type boundType, GridColumnType columnType)
    {
        Name = name;

        Type typeColumn = columnType switch
        {
            GridColumnType.Boolean => typeof(LgGridBooleanColumn<>),
            GridColumnType.DateTime => typeof(LgGridDateColumn<>),
            GridColumnType.Enum => typeof(LgGridEnumColumn<>),
            GridColumnType.Numeric => typeof(LgGridNumericColumn<>),
            GridColumnType.String => typeof(LgGridColumn<>),
            _ => throw new InvalidOperationException()
        };
        ColumnType = typeColumn.MakeGenericType(boundType);
    }

    //public abstract EventCallback<ChangeEventArgs> CreateEventCallBackBinder(EventCallbackFactory factory, object receiver, TItem value);

    public abstract LambdaExpression GetValueExpression(TItem item);

    internal abstract void AddValueChangedAttribute(RenderTreeBuilder builder, int sequence, object receiver, TItem item);

}
