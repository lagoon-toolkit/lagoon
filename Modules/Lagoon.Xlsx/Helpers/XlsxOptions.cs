using ClosedXML.Excel;

namespace Lagoon.Helpers;

public class XlsxOptions : LoadOptions
{
    #region Constants and private variabless

    private const char DECIMAL_SIGN = '.';
    private const string DATE_FORMAT = "dd/MM/yyyy";
    private const ExportDateOrder DATE_ORDER = ExportDateOrder.DayMonthYear;
    private const string TRUE = "1";
    private const string FALSE = "0";

    #endregion

    #region public properties

    public char DecimalChar { get; set; } = DECIMAL_SIGN;
    public string DateFormat { get; set; } = DATE_FORMAT;
    public CultureInfo Culture { get; set; }
    public ExportDateOrder DateOrder { get; set; } = DATE_ORDER;
    public string BoolTrueFormat { get; set; } = TRUE;
    public string BoolFalseFormat { get; set; } = FALSE;

    /// <summary>
    /// Specify if the group name is on a different row
    /// </summary>
    public bool GroupNameOnDistinctRow { get; set; }
    #endregion
}
