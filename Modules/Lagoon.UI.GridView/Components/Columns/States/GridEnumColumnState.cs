namespace Lagoon.UI.Components.Internal;

/// <summary>
/// The state of Enum column.
/// </summary>
internal class GridEnumColumnState<TEnum> : GridColumnState
{

    #region fields

    private Dictionary<Enum, string> _displayNames;

    /// <summary>
    /// Gets or sets default value to export.
    /// </summary>
    public ExportEnumFormat ExportFormat { get; set; }

    /// <summary>
    /// Gets or sets default language.
    /// </summary>
    public string ExportFormatLanguage { get; set; }

    #endregion

    #region constructors

    ///<inheritdoc/>
    internal override void OnInitialized()
    {
        base.OnInitialized();
        FilterShowAllItems = true;
        LoadEnumDisplayNames();
    }

    /// <summary>
    /// Load enum display names.
    /// </summary>
    internal void LoadEnumDisplayNames()
    {
        Type type = CellValueType;
        _displayNames = new();
        // Get underlying enum type for nullable enums
        Type nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType is not null)
        {
            type = nullableType;
        }
        foreach (Enum value in Enum.GetValues(type))
        {
            _displayNames.Add(value, value.GetDisplayName());
        }
    }

    /// <summary>
    /// Gets the display name associated to an enum number.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal string GetEnumDisplayName(Enum value)
    {
        if (_displayNames.TryGetValue(value, out string displayName))
        {
            return displayName;
        }
        return value.ToString();
    }

    /// <inheritdoc/>
    internal override IExportColumn<TItem> GetExportColumn<TItem>(string columnTitle, string groupTitle)
    {
        if (CellValueType is null || ParameterizedValueExpression is null)
        {
            return null;
        }
        return (IExportColumn<TItem>)Activator
            .CreateInstance(typeof(ExportEnumColumn<,>)
            .MakeGenericType(typeof(TItem), CellValueType), columnTitle, ParameterizedValueExpression, ExportFormat, ExportFormatLanguage, groupTitle);
    }

    #endregion
}
