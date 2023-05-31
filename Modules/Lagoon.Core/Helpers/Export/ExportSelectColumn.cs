namespace Lagoon.Helpers;

/// <summary>
/// Export Select Column
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TDataSourceItemValue"></typeparam>
public class ExportSelectColumn<TItem, TValue, TDataSourceItemValue> : IExportColumn<TItem>
{
    #region properties

    /// <summary>
    /// The data source. 
    /// </summary>
    public ListDataSource<TDataSourceItemValue, TValue> DataSource { get; set; }

    /// <summary>
    /// Export format
    /// </summary>
    public ExportFormat ExportFormat { get; set; }

    /// <summary>
    /// Column title.
    /// </summary>
    public string ColumnTitle { get; set; }

    /// <summary>
    /// Group Title
    /// </summary>
    public string ColumnGroupTitle { get; set; }

    /// <summary>
    /// The type of value in this column.
    /// </summary>
    Type IExportColumn<TItem>.ValueType => typeof(TValue);

    /// <summary>
    /// Method to get an item value for the column.
    /// </summary>
    public Func<TItem, TValue> GetValue { get; }

    /// <summary>
    /// Method to get an item value for the column.
    /// </summary>
    public Action<TItem, TValue> SetValue { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="getValueMethod">The "GetValue" lambda.</param>
    /// <param name="dataSource">The source.</param>
    /// <param name="exportFormat">Export format.</param>
    /// <param name="setValueMethod">The "SetValue" lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportSelectColumn(string columnTitle, Func<TItem, TValue> getValueMethod, Action<TItem, TValue> setValueMethod, ListDataSource<TDataSourceItemValue, TValue> dataSource, ExportFormat exportFormat, string groupTitle = null)
    {
        ColumnTitle = columnTitle;
        ColumnGroupTitle = groupTitle;
        GetValue = getValueMethod;
        DataSource = dataSource;
        ExportFormat = exportFormat;
        SetValue = setValueMethod;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="parameterizedValueExpression">The "GetValue" lambda.</param>
    /// <param name="dataSource">The source.</param>
    /// <param name="exportFormat">the format for export.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportSelectColumn(string columnTitle, LambdaExpression parameterizedValueExpression, ListDataSource<TDataSourceItemValue, TValue> dataSource, ExportFormat exportFormat, string groupTitle = null)
                : this(columnTitle, ((Expression<Func<TItem, TValue>>)parameterizedValueExpression).Compile(), null, dataSource, exportFormat, groupTitle)
    { }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="itemParameter">Item parameter for the lambda.</param>
    /// <param name="valueParameter">Value parameter for the lambda.</param>
    /// <param name="getLambdaBody">Body definition of the GetValue lambda.</param>
    /// <param name="dataSource">The source.</param>
    /// <param name="exportFormat">the format for export.</param>
    /// <param name="setLambdaBody">Body definition of the SetValue lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportSelectColumn(string columnTitle, ParameterExpression itemParameter, ParameterExpression valueParameter,
        MethodCallExpression getLambdaBody, MethodCallExpression setLambdaBody, ListDataSource<TDataSourceItemValue, TValue> dataSource, ExportFormat exportFormat, string groupTitle = null)
        : this(columnTitle, Expression.Lambda<Func<TItem, TValue>>(getLambdaBody, itemParameter).Compile(),
              Expression.Lambda<Action<TItem, TValue>>(setLambdaBody, itemParameter, valueParameter).Compile(), dataSource, exportFormat, groupTitle)
    { }

    #endregion

    #region methods

    object IExportColumn<TItem>.GetValue(TItem value)
    {
        if (ExportFormat == ExportFormat.Text)
        {
            IEnumerable<TDataSourceItemValue> items = DataSource?.Items;
            if (items is not null)
            {
                TDataSourceItemValue item = items.FirstOrDefault(i => DataSource.GetItemValue(i).Equals(GetValue(value)));
                if (item is not null)
                {
                    return DataSource?.GetItemText(item);
                }
            }
            return null;
        }
        return GetValue(value);
    }

    void IExportColumn<TItem>.SetValue(TItem item, object value)
    {
        SetValue(item, (TValue)value);
    }

    #endregion

}
