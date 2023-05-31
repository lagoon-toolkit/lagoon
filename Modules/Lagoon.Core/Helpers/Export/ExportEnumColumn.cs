namespace Lagoon.Helpers;

/// <summary>
/// Export Enum column
/// </summary>
public class ExportEnumColumn<TItem, TValue> : IExportColumn<TItem>
{

    #region fields

    /// <summary>
    /// Enum formated values.
    /// </summary>
    private Dictionary<Enum, string> _displayNames;

    #endregion

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
    /// The format to be used to format the values.
    /// </summary>
    public ExportEnumFormat ExportFormat { get; set; }

    /// <summary>
    /// The language to use if the <see cref="ExportFormat"/> property is <see cref="ExportEnumFormat.DisplayName"/>.
    /// If ValueFormatLanguage is <c>null</c> or undefined, the current language is used.
    /// </summary>
    public string ExportFormatLanguage { get; set; }

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
    /// <param name="exportFormat">export format</param>
    /// <param name="exportFormatLanguage">the default language to export.</param>
    /// <param name="setValueMethod">The "SetValue" lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportEnumColumn(string columnTitle, Func<TItem, TValue> getValueMethod, Action<TItem, TValue> setValueMethod, ExportEnumFormat exportFormat, string exportFormatLanguage, string groupTitle = null)
    {
        ColumnTitle = columnTitle;
        ColumnGroupTitle = groupTitle;
        GetValue = getValueMethod;
        ExportFormat = exportFormat;
        ExportFormatLanguage = exportFormatLanguage;
        SetValue = setValueMethod;
        // Crate a formated values dictionnary
        LoadEnumDisplayNames();
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="parameterizedValueExpression">The "GetValue" lambda.</param>
    /// <param name="exportFormat">export format</param>
    /// <param name="exportFormatLanguage">the default language to export.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportEnumColumn(string columnTitle, LambdaExpression parameterizedValueExpression, ExportEnumFormat exportFormat, string exportFormatLanguage, string groupTitle = null)
                : this(columnTitle, ((Expression<Func<TItem, TValue>>)parameterizedValueExpression).Compile(), null, exportFormat, exportFormatLanguage, groupTitle)
    { }

    /// <summary>
    /// New instance
    /// </summary>
    /// <param name="columnTitle">The column title.</param>
    /// <param name="itemParameter">Item parameter for the lambda.</param>
    /// <param name="valueParameter">Value parameter for the lambda.</param>
    /// <param name="getLambdaBody">Body definition of the GetValue lambda.</param>
    /// <param name="exportFormatLanguage">the default language to export.</param>
    /// <param name="exportFormat">export format</param>
    /// <param name="setLambdaBody">Body definition of the SetValue lambda.</param>
    /// <param name="groupTitle">The group title</param>
    public ExportEnumColumn(string columnTitle, ParameterExpression itemParameter, ParameterExpression valueParameter,
        MethodCallExpression getLambdaBody, MethodCallExpression setLambdaBody, ExportEnumFormat exportFormat, string exportFormatLanguage, string groupTitle = null)
        : this(columnTitle, Expression.Lambda<Func<TItem, TValue>>(getLambdaBody, itemParameter).Compile(),
              Expression.Lambda<Action<TItem, TValue>>(setLambdaBody, itemParameter, valueParameter).Compile(), exportFormat, exportFormatLanguage, groupTitle)
    { }

    #endregion

    #region methods

    /// <summary>
    /// Load enum display names.
    /// </summary>
    internal void LoadEnumDisplayNames()
    {
        if (ExportFormat == ExportEnumFormat.Value)
        {
            _displayNames = null;
        }
        else
        {
            _displayNames = new();
            // Get underlying enum type for nullable enums
            Type type = typeof(TValue).GetNullableUnderlyingTypeOrThis();
            foreach (Enum value in Enum.GetValues(type))
            {
                _displayNames.Add(value, ExportFormat == ExportEnumFormat.Text
                    ? value.ToString()
                    : value.GetDisplayName(language: ExportFormatLanguage));
            }
        }
    }

    /// <inheritdoc />
    object IExportColumn<TItem>.GetValue(TItem item)
    {
        if (GetValue(item) is null)
        {
            return null;
        }

        if (_displayNames is null)
        {
            return GetValue(item);
        }
        else
        {
            return _displayNames.TryGetValue(GetValue(item) as Enum, out string displayName) ? displayName : (object)item.ToString();
        }
    }

    void IExportColumn<TItem>.SetValue(TItem item, object value)
    {
        SetValue(item, (TValue)value);
    }

    #endregion

}
