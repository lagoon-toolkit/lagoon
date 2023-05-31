using ClosedXML.Excel;

namespace Lagoon.Helpers;

public class XlsxDocument<T> : ExportDocument<T>
{
    #region  fields

    /// <summary>
    /// IXLWorkbook
    /// </summary>
    private IXLWorkbook _workbook;


    /// <summary>
    /// IXLWorksheet
    /// </summary>
    private IXLWorksheet _worksheet;

    /// <summary>
    /// Xlsx Option 
    /// </summary>
    private readonly XlsxOptions _options;

    #endregion

    #region properties

    protected IXLWorkbook Workbook => _workbook;

    protected IXLWorksheet Worksheet => _worksheet;

    public XlsxOptions Options => _options;

    #endregion

    #region methods

    /// <summary>
    ///  XLSX file opening
    /// </summary>
    /// <param name="options">XLSX Options</param>
    /// <param name="exportColumns">Export Columns</param>
    /// <param name="autoCloseStream">IStream must be closed.</param>
    public XlsxDocument(XlsxOptions options, ExportColumnCollection<T> exportColumns = null)
    {
        if (options is null)
        {
            // Use default application parameters
            _options = GetDefaultOptions();
        }
        else
        {
            // Keep options from parameters 
            _options = options;
        }
        InitializeColumnList(exportColumns);
        Load();
    }

    /// <summary>
    /// Returns the default settings for reading and XLSX file generation.
    /// </summary>
    /// <returns>Les paramètres par défaut pour la lecture et la génération de fichier XLSX.</returns>
    public XlsxOptions GetDefaultOptions()
    {
        XlsxOptions l_o_options = new();

        // Apply default application settings
        return l_o_options;
    }

    #endregion

    #region Open and close

    /// <summary>
    /// Opening a file in XLSX format
    /// </summary>
    protected override void Load()
    {
        // Close object 
        Close();
        // Get new stream
        _workbook = new XLWorkbook();
        _worksheet = _workbook.Worksheets.Add("Data");

    }

    /// <summary>
    /// Close & Dispose
    /// </summary>
    protected override void Close()
    {
        // On efface les champs chargés
        Fields.Clear();
        // Ferme l'outil d'écriture
        if (_worksheet != null || _workbook != null)
        {
            _workbook.Dispose();
            _workbook = null;
            _worksheet = null;
        }
    }

    #endregion

    #region Write functions

    /// <summary>
    /// Sends the current line on the stream
    /// </summary>
    protected virtual void AddLine(int lineIndex)
    {
        int columnIndex = 1;
        // Loop on each fields
        foreach (string value in Fields)
        {
            _worksheet.Cell(lineIndex, columnIndex).Value = $"{value}";
            columnIndex++;
        }
        // Clear buffer
        Fields.Clear();
    }

    /// <summary>
    /// Add one group on the group line. The range of consecutive columns for this group is specified.
    /// It is to be noted, that if the same group names is spread on different separated ranges, this method
    /// will be called for all these ranges.
    /// </summary>
    /// <param name="lineIndex">the line containing the group names</param>
    /// <param name="groupName">the group name to add</param>
    /// <param name="startColumn">the first column where the group begins</param>
    /// <param name="endColumn">the first column where the group ends</param>
    protected virtual void AddOneGroup(int lineIndex,string groupName,int startColumn, int endColumn)
    {
        if (startColumn != endColumn)
        {
            _worksheet.Range(_worksheet.Cell(lineIndex, startColumn), _worksheet.Cell(lineIndex, endColumn)).Merge();
        }
        var cell = _worksheet.Cell(lineIndex, startColumn);
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Value = groupName;
    }

    /// <summary>
    /// Sends the current group header line on the stream, the cell are merged when they have a value and repeated
    /// </summary>
    protected virtual void AddGroupHeaderLine(int lineIndex)
    {
        int columnIndex = 1;
        string lastGroupName = null;
        int lastGroupFirstPos = -1; // value if lastGroupName is not null
        // Loop on each fields
        foreach (string value in Fields)
        {
            bool shouldClosePreviousGroup = true;

            if (!String.IsNullOrWhiteSpace(value))
            {
                if (lastGroupName == value)
                    shouldClosePreviousGroup = false;
            }
            if (shouldClosePreviousGroup)
            {
                if (lastGroupName != null)
                {
                    // display the previous group
                    AddOneGroup(lineIndex, lastGroupName, lastGroupFirstPos, columnIndex - 1);
                    lastGroupName = null;
                }
                if (String.IsNullOrWhiteSpace(value))
                {
                    AddOneGroup(lineIndex, String.Empty, columnIndex, columnIndex);
                } else
                {
                    lastGroupName = value;
                    lastGroupFirstPos = columnIndex;
                }

            }
            columnIndex++;
        }
        if (lastGroupName != null)
        {
            // display the last group if not displayed
            AddOneGroup(lineIndex, lastGroupName, lastGroupFirstPos, columnIndex - 1);
        }
        // Clear buffer
        Fields.Clear();
    }

    /// <summary>
    /// Adds a value to the table of fields and puts it between quotes if necessary
    /// </summary>
    private void AddFieldRaw(string value)
    {
        // Add the field to the list
        Fields.Add(value);
    }

    /// <summary>
    /// Adds a new value to the current line.
    /// </summary>
    protected virtual void AddField(object value)
    {
        if (value is null)
        {
            AddFieldRaw("");
            return;
        }
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.Double:
            case TypeCode.Single:
            case TypeCode.Decimal:
                {
                    AddField(Convert.ToDouble(value));
                    break;
                }

            case TypeCode.Boolean:
                {
                    AddField(Convert.ToBoolean(value));
                    break;
                }

            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                {
                    AddField(Convert.ToInt64(value));
                    break;
                }

            case TypeCode.DateTime:
                {
                    AddField((DateTime)value);
                    break;
                }

            case TypeCode.DBNull:
                {
                    AddField(string.Empty);
                    break;
                }

            default:
                {
                    AddField(Convert.ToString(value));
                    break;
                }
        }
    }

    public void AddField(string value)
    {
        AddFieldRaw(value);
    }

    public void AddField(double value)
    {
        AddFieldRaw(Numeric.NumToStr(value).Replace('.', _options.DecimalChar));
    }

    public void AddField(long value)
    {
        AddFieldRaw(Numeric.NumToStr(value));
    }

    public void AddField(int value)
    {
        AddFieldRaw(Numeric.NumToStr(value));
    }

    public void AddField(DateTime value)
    {
        System.Globalization.CultureInfo lng = _options.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
        AddFieldRaw(value.ToString(_options.DateFormat, lng));
    }

    public void AddField(DateTime value, string format)
    {
        AddFieldRaw(string.Format(format, value));
    }

    public void AddField(bool value)
    {
        if (value)
        {
            AddFieldRaw(_options.BoolTrueFormat);
        }
        else
        {
            AddFieldRaw(_options.BoolFalseFormat);
        }
    }

    #endregion

    #region Write data

    /// <summary>
    /// Write Data into Xlsx
    /// </summary>
    /// <param name="items">Items list (datasource)</param>
    /// <param name="header">Header to write</param>
    /// <param name="fileDestination">XLSX File</param>
    protected virtual void WriteDataAsXlsxWriter(IEnumerable<T> items, bool header, Stream stream)
    {
        int lineindex = 1;
        // Header
        if (header)
        {
            lineindex = WriteFullHeader(lineindex);
        }
        // Values
        foreach (T item in items)
        {
            foreach (IExportColumn<T> column in Columns)
            {
                AddField(column.GetValue(item));
            }
            AddLine(lineindex);
            lineindex++;
        }
        _workbook.SaveAs(stream);
    }

    /// <summary>
    /// Write the full header of the table which is an optional group header and the standard header
    /// </summary>
    /// <param name="lineindex"></param>
    /// <returns></returns>
    protected virtual int WriteFullHeader(int lineindex)
    {
        if (Options.GroupNameOnDistinctRow)
        {
            lineindex = WriteGroupHeaderLine(lineindex);
        }
        lineindex = WriteColumnHeaderLine(lineindex);
        return lineindex;
    }

    /// <summary>
    /// Write the header line containing the column names
    /// </summary>
    /// <param name="lineindex">The line number from which the header can be written</param>
    /// <returns>the line number below the header</returns>
    protected virtual int WriteColumnHeaderLine(int lineindex)
    {
        foreach (IExportColumn<T> column in Columns)
        {
            AddField(column.ColumnTitle);
        }
        AddLine(lineindex);
        lineindex++;
        return lineindex;
    }

    /// <summary>
    /// Write the group header line. (i.e. the line containing the group names)
    /// </summary>
    /// <param name="lineindex">The line number from which the header can be written</param>
    /// <returns>the line number below the header</returns>
    protected virtual int WriteGroupHeaderLine(int lineindex)
    {
        foreach (IExportColumn<T> column in Columns)
        {
            AddField(column.ColumnGroupTitle);
        }
        AddGroupHeaderLine(lineindex);
        lineindex++;
        return lineindex;
    }

    /// <summary>
    /// Serialize items to xlsx file
    /// </summary>
    /// <param name="items">Items list</param>
    /// <param name="fileDestination">XLSX File</param>
    /// <param name="header">Header must be wrote</param>
    public virtual void Serialize(IEnumerable<T> items, Stream stream, bool header = true)
    {
        if (items is not null)
        {
            WriteDataAsXlsxWriter(items, header, stream);
        }
    }

    #endregion

    #region GetByteArray

    /// <summary>
    /// Serialize items to xlsx file and write content into a byte array.
    /// </summary>
    /// <param name="items">Items list</param>
    /// <param name="exportColumns">Export Columns</param>
    /// <param name="options">XLSX Options</param>
    /// <returns>The content of the XSLX file.</returns>
    public static byte[] SerializeToByteArray(IEnumerable<T> items, ExportColumnCollection<T> exportColumns = null, XlsxOptions options = null)
    {
        using (MemoryStream stream = new())
        {
            using (XlsxDocument<T> XlsxSerialized = new(options, exportColumns))
            {
                // Write Xslx content to memory
                XlsxSerialized.Serialize(items, stream, true);
                // Return the file content
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }

    #endregion

}
