namespace Lagoon.Helpers;

/// <summary>
/// Numeric methods helper.
/// </summary>
public static class Numeric
{

    /// <summary>
    /// Clean numeric input value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string CleanNumericValue(string value)
    {
        StringBuilder sb = new(value);
        sb.Replace('\u00A0', ' ');
        sb.Replace(" ", "");
        if (CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator != ",")
        {
            sb.Replace(",", ".");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Check if value i numeric
    /// </summary>
    /// <param name="value">value to check</param>
    /// <returns></returns>
    public static bool IsNumeric(this string value)
    {
        return float.TryParse(value, out _);
    }

    #region Convertion Numérique en chaine

    /// <summary>
    /// Returns the string corresponding to the object passed in parameter without taking into account the
    /// regional settings.
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <returns></returns>
    public static string NumToStr(object value)
    {
        if (value is double)
        {
            return Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture);
        }
        else if (value is decimal)
        {
            return Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);
        }
        else if (value is float)
        {
            return Convert.ToSingle(value).ToString(CultureInfo.InvariantCulture);
        }
        else if (value is Enum)
        {
            return Convert.ToInt32(value).ToString();
        }
        else
        {
            return value.ToString();
        }
    }

    ///// <summary>
    ///// Returns the string corresponding to the double value passed in parameter without taking into account the
    ///// regional settings.
    ///// </summary>
    ///// <param name="value">value to convert</param>
    ///// <returns></returns>
    //public static string NumToStr(double value)
    //{
    //    return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    //}

    ///// <summary>
    ///// Returns the string corresponding to the decimal value passed as a parameter without taking into account the
    ///// regional settings.
    ///// </summary>
    ///// <param name="value">value to convert</param>
    ///// <returns></returns>
    //public static string NumToStr(decimal value)
    //{
    //    return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    //}

    ///// <summary>
    ///// Returns the string corresponding to the decimal value passed as a parameter without taking into account the
    ///// regional settings.
    ///// </summary>
    ///// <param name="value">value to check</param>
    ///// <returns></returns>
    //public static string NumToStr(float value)
    //{
    //    return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    //}


    #endregion


}
