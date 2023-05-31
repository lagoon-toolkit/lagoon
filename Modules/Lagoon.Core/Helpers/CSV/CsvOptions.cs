namespace Lagoon.Helpers;

/// <summary>
/// Configuration for the CSV document.
/// </summary>
public class CsvOptions
{

    #region Constants and private variabless

    private const char SEPARATOR = ';';
    private const char ALPHA_CHAR = ControlChars.Quote;
    private const ExportDelimiteMode DELIMITER_MODE = ExportDelimiteMode.Alpha;
    private const char DECIMAL_SIGN = '.';
    private const string DATE_FORMAT = "dd/MM/yyyy";
    private const string DATE_SEPARATOR = "/";
    private const ExportDateOrder DATE_ORDER = ExportDateOrder.DayMonthYear;
    private const string TRUE = "1";
    private const string FALSE = "0";

    #endregion

    #region properties

    /// <summary>
    /// Text encoding.
    /// </summary>
    public Encoding CharSet { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Separator between values.
    /// </summary>
    public char FieldSeparator { get; set; } = SEPARATOR;
    
    /// <summary>
    /// Delimiter around text values.
    /// </summary>
    public char Delimiter { get; set; } = ALPHA_CHAR;

    /// <summary>
    /// Delimiter mode.
    /// </summary>
    public ExportDelimiteMode DelimiteMode { get; set; } = DELIMITER_MODE;

    /// <summary>
    /// Decimal char.
    /// </summary>
    public char DecimalChar { get; set; } = DECIMAL_SIGN;

    /// <summary>
    /// Date format.
    /// </summary>
    public string DateFormat { get; set; } = DATE_FORMAT;

    /// <summary>
    /// Culture to use.
    /// </summary>
    public CultureInfo Culture { get; set; }

    /// <summary>
    /// Date separator.
    /// </summary>
    public string DateSeparator { get; set; } = DATE_SEPARATOR;

    /// <summary>
    /// Date parts order.
    /// </summary>
    public ExportDateOrder DateOrder { get; set; } = DATE_ORDER;

    /// <summary>
    /// String for true values.
    /// </summary>
    public string BoolTrueFormat { get; set; } = TRUE;

    /// <summary>
    /// String for false values.
    /// </summary>
    public string BoolFalseFormat { get; set; } = FALSE;

    /// <summary>
    /// Text to write at the begining of the CSV file.
    /// </summary>
    public string Preface { get; set; } = null;

    #endregion

}
