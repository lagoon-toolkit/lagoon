namespace Lagoon.UI.GridView.Helpers;

/// <summary>
/// Gridview helpers
/// </summary>
public static class GridViewHelper
{

    /// <summary>
    /// Generic converter
    /// </summary>
    /// <param name="type">to convert type</param>
    /// <param name="value">value to convert</param>
    internal static object ConvertGenerictValue(string value, Type type)
    {
        try
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter != null)
            {
                return converter.ConvertFromInvariantString(value);
            }
            return default;
        }
        catch (Exception)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }

    /// <summary>
    /// Get the type of column needed for a type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The corresponding column type.</returns>
    internal static GridColumnType GetColumnType(Type type)
    {
        if (type is null)
        {
            return GridColumnType.None;
        }
        switch (Nullable.GetUnderlyingType(type) ?? type)
        {
            case Type intType when intType == typeof(int):
            case Type uintType when uintType == typeof(uint):
            case Type floatType when floatType == typeof(float):
            case Type doubleType when doubleType == typeof(double):
            case Type decimalType when decimalType == typeof(decimal):
            case Type longType when longType == typeof(long):
            case Type ulongType when ulongType == typeof(ulong):
            case Type shortType when shortType == typeof(short):
            case Type ushortType when ushortType == typeof(ushort):
            case Type byteType when byteType == typeof(byte):
                return GridColumnType.Numeric;
            case Type stringType when stringType == typeof(string):
                return GridColumnType.String;
            case Type boolType when boolType == typeof(bool):
                return GridColumnType.Boolean;
            case Type dateType when dateType == typeof(DateTime):
                return GridColumnType.DateTime;
            case Type enumType when enumType.BaseType == typeof(Enum):
                return GridColumnType.Enum;
            default:
                return GridColumnType.None;
        }
    }

}
