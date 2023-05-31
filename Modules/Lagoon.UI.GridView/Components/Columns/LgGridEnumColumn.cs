using Lagoon.UI.Application;

namespace Lagoon.UI.Components;

/// <summary>
/// Enum column
/// </summary>
/// <typeparam name="TColumnValue">The type of data bound to the column. (Always object for the <see cref="LgDynamicGridView"/>).</typeparam>
public class LgGridEnumColumn<TColumnValue> : LgGridBaseValueColumn<TColumnValue>
{
    #region Parameters

    /// <summary>
    /// Gets or sets default value to export.
    /// </summary>
    [Parameter]
    public ExportEnumFormat? ExportFormat { get; set; }

    /// <summary>
    /// Invariant language.
    /// </summary>
    [Parameter]
    public string ExportFormatLanguage { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    internal override void OnStateInitialized()
    {
        base.OnStateInitialized();
        (State as GridEnumColumnState<TColumnValue>).ExportFormat = ExportFormat ?? GridViewBehaviour.Options.ExportConfiguration.ExportEnumFormat;
        (State as GridEnumColumnState<TColumnValue>).ExportFormatLanguage = string.IsNullOrEmpty(ExportFormatLanguage) ? GridViewBehaviour.Options.ExportConfiguration.ExportFormatLanguage : ExportFormatLanguage;
    }

    ///<inheritdoc/>
    internal override Type OnGetCellComponentType(Type itemType, Type columnValueType, Type cellValueType)
    {
        Type enumType = Nullable.GetUnderlyingType(cellValueType) ?? cellValueType;
        return typeof(LgGridEnumCell<,,,>).MakeGenericType(itemType, columnValueType, cellValueType, enumType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterType(Type cellValueType)
    {
        return typeof(EnumFilter<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override Type OnGetFilterBoxType(Type cellValueType)
    {
        return typeof(LgEnumFilterBox<>).MakeGenericType(cellValueType);
    }

    ///<inheritdoc/>
    internal override GridColumnState CreateState()
    {
        return new GridEnumColumnState<TColumnValue>();
    }

    ///<inheritdoc/>
    internal override string FormatValueAsString<TCellValue>(TCellValue cellValue)
    {
        if (cellValue is Enum value)
        {
            return ((GridEnumColumnState<TColumnValue>)State).GetEnumDisplayName(value);
        }
        else
        {
            return string.Empty;
        }
    }

    #endregion

}
