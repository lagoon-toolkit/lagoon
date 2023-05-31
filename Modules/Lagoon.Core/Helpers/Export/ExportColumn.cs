namespace Lagoon.Helpers;

/// <summary>
/// Export Column
/// </summary>
public class ExportColumn<TItem, TValue> : IExportColumn<TItem>
{

    #region properties

    /// <summary>
    /// Column title.
    /// </summary>
    public string ColumnTitle { get; set; }

    /// <summary>
    /// Group Title
    /// </summary>
    public string ColumnGroupTitle { get; set; }

    /// <summary>
    /// Method to get an item value for the column.
    /// </summary>
    public Func<TItem, TValue> GetValue { get; }

    /// <summary>
    /// Method to get an item value for the column.
    /// </summary>
    public Action<TItem, TValue> SetValue { get; }

    /// <summary>
    /// The type of value in this column.
    /// </summary>
    Type IExportColumn<TItem>.ValueType => typeof(TValue);

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="getValueMethod">The "GetValue" lambda.</param>
    /// <param name="setValueMethod">The "SetValue" lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportColumn(string columnTitle, Func<TItem, TValue> getValueMethod, Action<TItem, TValue> setValueMethod, string groupTitle = null)
    {
        ColumnTitle = columnTitle;
        ColumnGroupTitle = groupTitle;
        GetValue = getValueMethod;
        SetValue = setValueMethod;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="parameterizedValueExpression">The "GetValue" lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportColumn(string columnTitle, LambdaExpression parameterizedValueExpression, string groupTitle = null)
                : this(columnTitle, ((Expression<Func<TItem, TValue>>)parameterizedValueExpression).Compile(), null, groupTitle)
    { }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="itemParameter">Item parameter for the lambda.</param>
    /// <param name="valueParameter">Value parameter for the lambda.</param>
    /// <param name="getLambdaBody">Body definition of the GetValue lambda.</param>
    /// <param name="setLambdaBody">Body definition of the SetValue lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportColumn(string columnTitle, ParameterExpression itemParameter, ParameterExpression valueParameter,
        MethodCallExpression getLambdaBody, MethodCallExpression setLambdaBody, string groupTitle = null)
        : this(columnTitle, Expression.Lambda<Func<TItem, TValue>>(getLambdaBody, itemParameter).Compile(),
              Expression.Lambda<Action<TItem, TValue>>(setLambdaBody, itemParameter, valueParameter).Compile(), groupTitle)
    { }

    #endregion

    #region methods

    /// <summary>
    /// Get the value of the column.
    /// </summary>
    /// <param name="item">Item to get the value from.</param>
    /// <returns>The value of the column.</returns>
    object IExportColumn<TItem>.GetValue(TItem item)
    {
        return GetValue(item);
    }

    /// <summary>
    /// Set the value of the column.
    /// </summary>
    /// <param name="item">Item to update.</param>
    /// <param name="value">The new value.</param>
    void IExportColumn<TItem>.SetValue(TItem item, object value)
    {
        SetValue(item, (TValue)value);
    }

    #endregion

}