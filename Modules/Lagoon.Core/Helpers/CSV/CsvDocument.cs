using System.Collections;

namespace Lagoon.Helpers;

/// <summary>
/// Class to read or write a CSV document from a list.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CsvDocument<T> : ExportDocument<T>
{

    #region  fields

    /// <summary>
    /// Close indicator
    /// </summary>
    private bool _autoCloseStream;

    /// <summary>
    /// Stream object
    /// </summary>
    private Stream _stream;

    /// <summary>
    /// StreamWriter from _stream object
    /// </summary>
    private StreamWriter _writer;

    /// <summary>
    /// StreamReader from _stream object
    /// </summary>
    private StreamReader _reader;

    /// <summary>
    /// Csv Option 
    /// </summary>
    private CsvOptions _options;

    /// <summary>
    /// Model properties list
    /// </summary>
    protected Dictionary<IExportColumn<T>, PropertyInfo> Properties { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="path">CSV path</param>
    public CsvDocument(string path)
    {
        // Par défaut on ouvre le fichier en lecture
        Initialize(null, path, FileMode.Open, FileAccess.Read);
    }

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="path">CSV path</param>
    /// <param name="options">CSV Options</param>
    /// <param name="exportColumns">Export Columns</param>
    public CsvDocument(string path, CsvOptions options, ExportColumnCollection<T> exportColumns = null)
    {
        // Par défaut on ouvre le fichier en lecture
        Initialize(options, path, FileMode.Open, FileAccess.Read, exportColumns);
    }

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="path">CCSV path</param>
    /// <param name="mode">File opening mode</param>
    /// <param name="exportColumns">Export Columns</param>
    public CsvDocument(string path, FileMode mode, ExportColumnCollection<T> exportColumns = null)
    {
        Initialize(null, path, mode, GetFileAccess(mode), exportColumns);
    }

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="path">CSV path</param>
    /// <param name="mode">File opening mode</param>
    /// <param name="options">CSV Options</param>
    /// <param name="exportColumns">Export Columns</param>
    public CsvDocument(string path, FileMode mode, CsvOptions options, ExportColumnCollection<T> exportColumns = null)
    {
        // Suivant le mode choisit on définit un type d'accès
        Initialize(options, path, mode, GetFileAccess(mode), exportColumns);
    }

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="path">CSV path</param>
    /// <param name="mode">File opening mode</param>
    /// <param name="access">File access type</param>
    /// <param name="exportColumns">Export Columns</param>
    public CsvDocument(string path, FileMode mode, FileAccess access, ExportColumnCollection<T> exportColumns = null)
    {
        // On utilise le mode et le type d'accès définit par l'utilisateur
        Initialize(null, path, mode, access, exportColumns);
    }

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="path">CSV path</param>
    /// <param name="mode">File opening mode</param>
    /// <param name="access">File access type</param>
    /// <param name="options">CSV Options</param>
    /// <param name="exportColumns">Export Columns</param>
    public CsvDocument(string path, FileMode mode, FileAccess access, CsvOptions options, ExportColumnCollection<T> exportColumns = null)
    {
        // On utilise le mode et le type d'accès définit par l'utilisateur
        Initialize(options, path, mode, access, exportColumns);
    }

    /// <summary>
    /// CSV file opening
    /// </summary>
    /// <param name="stream">CSV stream</param>
    /// <param name="exportColumns">Export Columns</param>
    /// <param name="autoCloseStream">IStream must be closed when CSV document is disposed.</param>
    public CsvDocument(Stream stream, ExportColumnCollection<T> exportColumns = null, bool autoCloseStream = false)
    {
        Initialize(null, stream, exportColumns, autoCloseStream);
    }

    /// <summary>
    ///  CSV file opening
    /// </summary>
    /// <param name="stream">CSV stream</param>
    /// <param name="options">CSV Options</param>
    /// <param name="exportColumns">Export Columns</param>
    /// <param name="autoCloseStream">IStream must be closed when CSV document is disposed.</param>
    public CsvDocument(Stream stream, CsvOptions options, ExportColumnCollection<T> exportColumns = null, bool autoCloseStream = false)
    {
        Initialize(options, stream, exportColumns, autoCloseStream);
    }

    /// <summary>
    ///  CSV file opening
    /// </summary>
    /// <param name="options">CSV generation options</param>
    /// <param name="path">CSV path</param>
    /// <param name="mode">File opening mode</param>
    /// <param name="access">File access type</param>
    /// <param name="exportColumns">Export Columns</param>
    private void Initialize(CsvOptions options, string path, FileMode mode, FileAccess access, ExportColumnCollection<T> exportColumns = null)
    {
        Initialize(options, new System.IO.FileStream(path, mode, access), exportColumns, true);
    }

    /// <summary>
    ///  CSV file opening
    /// </summary>
    /// <param name="options">CSV Options</param>
    /// <param name="stream">CSV stream</param>
    /// <param name="exportColumns">Export Columns</param>
    /// <param name="autoCloseStream">IStream must be closed when CSV document is disposed.</param>
    private void Initialize(CsvOptions options, Stream stream, ExportColumnCollection<T> exportColumns = null, bool autoCloseStream = false)
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
        _autoCloseStream = autoCloseStream;
        _stream = stream;
        InitializeColumnList(exportColumns);
        Load();
    }

    /// <summary>
    /// Returns the default access type for an opening mode
    /// </summary>
    /// <param name="mode">Mode d'ouverture</param>
    /// <returns></returns>
    private static FileAccess GetFileAccess(FileMode mode)
    {
        switch (mode)
        {
            case FileMode.Open:
                {
                    return FileAccess.Read;
                }

            case FileMode.OpenOrCreate:
                {
                    return FileAccess.ReadWrite;
                }

            default:
                {
                    return FileAccess.Write;
                }
        }
    }

    /// <summary>
    /// Returns the default settings for reading and CSV file generation.
    /// </summary>
    /// <returns>Les paramètres par défaut pour la lecture et la génération de fichier CSV.</returns>
    public CsvOptions GetDefaultOptions()
    {
        CsvOptions l_o_options = new();

        // Apply default application settings
        // TODO later ApplicationManager.Current.OnInitCsvDocument(l_o_options);
        return l_o_options;
    }

    #endregion

    #region Open and close

    /// <summary>
    /// Opening a file in CSV format
    /// </summary>
    protected override void Load()
    {
        // Close object 
        Close();
        // Open stream writer and reader
        if (_stream.CanWrite)
        {
            _writer = new StreamWriter(_stream, _options.CharSet, 1024, !_autoCloseStream);
        }

        if (_stream.CanRead)
        {
            _reader = new StreamReader(_stream, _options.CharSet, true, 1024, !_autoCloseStream);
        }
    }

    /// <summary>
    /// Close stream reader and / or writer
    /// </summary>
    protected override void Close()
    {
        // On efface les champs chargés
        Fields.Clear();
        // Ferme l'outil de lecture
        if (_reader is not null)
        {
            _reader.Close();
            _reader = null;
        }
        // Ferme l'outil d'écriture
        if (_writer is not null)
        {
            _writer.Close();
            _writer = null;
        }
    }

    #endregion

    #region Read functions

    /// <summary>
    /// Returns the string corresponding to a field.
    /// </summary>
    private string GetString(int index)
    {
        return Fields[index];
    }

    /// <summary>
    /// Returns the numeric value corresponding to a field.
    /// </summary>
    private object GetDouble(int index)
    {
        if (double.TryParse(GetInvariantFormatedNumber(index, false), out double result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the long value corresponding to a field.
    /// </summary>
    private object GetLong(int index)
    {
        if (long.TryParse(GetInvariantFormatedNumber(index, true), out long result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the integer numeric value corresponding to a field.
    /// </summary>
    private object GetInt(int index)
    {

        if (int.TryParse(GetInvariantFormatedNumber(index, true), out int result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Convert the number in the string to an invariant culture number.
    /// </summary>
    /// <param name="index">Index of the field.</param>
    /// <param name="trimDecimals">Indicate if the decimal part must be removed.</param>
    /// <returns>The number in the string to an invariant culture number.</returns>
    private string GetInvariantFormatedNumber(int index, bool trimDecimals)
    {
        StringBuilder value = new(Fields[index]);
        // Remove spaces, commas, periods and parametric separator from the string.
        if (_options.DecimalChar != ' ')
        {
            value.Replace(" ", "");
        }
        if (_options.DecimalChar != ',')
        {
            value.Replace(",", "");
        }
        if (_options.DecimalChar != '.')
        {
            value.Replace(_options.DecimalChar, '.');
        }
        // Remove the decimal part if needed
        if (trimDecimals)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '.')
                {
                    value.Length = i;
                    break;
                }
            }
        }
        return value.ToString();
    }

    /// <summary>
    /// Returns the date corresponding to a field
    /// </summary>
    private object GetDate(int index)
    {
        return ConvertDate(Fields[index]);
    }

    /// <summary>
    /// Returns the boolean value corresponding to a field.
    /// </summary>
    private bool GetBool(int index)
    {
        return ConvertBool(Fields[index]);
    }

    /// <summary>
    /// Returns the guid corresponding to a field.
    /// </summary>
    private System.Guid GetGuid(int index)
    {
        return Guid.Parse(Fields[index]);
    }

    #endregion

    #region Wrtie functions

    /// <summary>
    /// Sends the current line on the stream
    /// </summary>
    public void AddLine()
    {
        bool b = false;

        // Loop on each fields
        foreach (object value in Fields)
        {
            if (b)
            {
                _writer.Write(_options.FieldSeparator);
            }
            else
            {
                b = true;
            }

            _writer.Write(value);
        }
        _writer.WriteLine();
        // Clear buffer
        Fields.Clear();
    }

    /// <summary>
    /// Adds a value to the table of fields and puts it between quotes if necessary
    /// </summary>
    private void AddFieldRaw(string value, bool alpha = false)
    {
        // The value is enclosed in quotation marks
        if ((alpha && (_options.DelimiteMode == ExportDelimiteMode.Alpha)) || (_options.DelimiteMode == ExportDelimiteMode.All))
        {
            value = EscapeString(value, _options.Delimiter);
        }
        // Add the field to the list
        Fields.Add(value);
    }

    /// <summary>
    /// Adds a new value to the current line.
    /// </summary>
    public void AddField(object value, bool alpha = true)
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
                    AddField(string.Empty, alpha);
                    break;
                }

            default:
                {
                    AddField(Convert.ToString(value).Replace("\"", "\"\""), alpha);
                    break;
                }
        }
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="alpha">Quote the value.</param>
    public void AddField(string value, bool alpha = true)
    {
        AddFieldRaw(value, alpha);
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddField(double value)
    {
        AddFieldRaw(Numeric.NumToStr(value).Replace('.', _options.DecimalChar));
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddField(long value)
    {
        AddFieldRaw(Numeric.NumToStr(value));
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddField(int value)
    {
        AddFieldRaw(Numeric.NumToStr(value));
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddField(DateTime value)
    {
        System.Globalization.CultureInfo lng = _options.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
        AddFieldRaw(value.ToString(_options.DateFormat, lng));
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">Date format.</param>
    public void AddField(DateTime value, string format)
    {
        AddFieldRaw(string.Format(format, value));
    }

    /// <summary>
    /// Add a new value to the line.
    /// </summary>
    /// <param name="value">The value.</param>
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

    #region Convertion and check functions

    /// <summary>
    /// Tries to convert a string into a date from the value.
    /// </summary>
    private object ConvertDate(string txt)
    {
        int pos;
        int[] val = new int[3];
        int idx;
        object convertDate;

        convertDate = null;
        txt = txt.Trim(); // Ignore spaces
        if (_options.DateSeparator == string.Empty)
        {
            // Format without separator : JJMMAA[AA] ou MMJJAA[AA]
            if (_options.DateOrder == ExportDateOrder.YearMonthDay)
            {
                // not possible to define number for year format
                return null;
            }

            pos = 1;
            for (idx = 0; idx <= 1; idx++)
            {
                val[idx] = Convert.ToInt32(txt.Substring(pos, 2));
                pos += 2;
            }
            val[2] = Convert.ToInt32(txt.Substring(pos, 4));
        }
        else
        {
            // Format with separator
            for (idx = 0; idx <= 1; idx++)
            {
                pos = txt.IndexOf(_options.DateSeparator);
                if (pos == -1)
                {
                    return null;
                }

                val[idx] = Convert.ToInt32(txt[..(pos - 1)]);
                txt = txt[(pos + _options.DateSeparator.Length)..];
            }
            val[2] = Convert.ToInt32(txt[..4]);
        }
        try
        {
            // Try to create a date from the value
            switch (_options.DateOrder)
            {
                case ExportDateOrder.DayMonthYear:
                    convertDate = new DateTime(val[2], val[1], val[0]);
                    break;
                case ExportDateOrder.MonthDayYear:
                    convertDate = new DateTime(val[2], val[0], val[1]);
                    break;
                case ExportDateOrder.YearMonthDay:
                    convertDate = new DateTime(val[0], val[1], val[2]);
                    break;
            }
        }
        catch (Exception)
        { }
        return convertDate;
    }

    /// <summary>
    /// Converts a string value to boolean
    /// </summary>
    private static bool ConvertBool(string txt)
    {
        // If the value is numeric it must be different from 0.
        if (Convert.ToInt32(txt) != 0)
        {
            return true;
        }

        return "VTOY".Contains(txt.TrimStart()[..1].ToUpper());
    }

    /// <summary>
    /// Frames a character string with the character provided for this purpose
    /// </summary>
    public static string EscapeString(string txt, char delimiter = '"')
    {
        if (txt is null)
        {
            // Null values should not be escaped
            return txt;
        }
        StringBuilder sb = new(txt.Length + 3);
        sb.Append(delimiter);
        sb.Append(txt);
        sb.Replace(delimiter.ToString(), $"{delimiter}{delimiter}");
        sb.Append(delimiter);
        return sb.ToString(1, sb.Length - 1);
    }

    #endregion

    #region Write data

    /// <summary>
    /// Inserts the preface text in the file if it is specified in options
    /// </summary>
    public void WritePreface()
    {
        if (_options.Preface is not null)
        {
            _writer.WriteLine(_options.Preface);
        }
    }

    /// <summary>
    /// Write Data into Csv
    /// </summary>
    /// <param name="items">Items list (datasource)</param>
    /// <param name="header">Header to write</param>
    /// <param name="alpha">Fields data must be quoted</param>
    private void WriteDataAsCsvWriter(IEnumerable<T> items, bool header, bool alpha)
    {
        // Header
        if (header)
        {
            foreach (IExportColumn<T> column in Columns)
            {
                AddField(column.ColumnTitle, alpha);
            }
            AddLine();
        }
        // Values
        foreach (T item in items)
        {
            foreach (IExportColumn<T> column in Columns)
            {
                AddField(column.GetValue(item), alpha);
            }
            AddLine();
        }
    }

    /// <summary>
    /// Serialize items to csv file
    /// </summary>
    /// <param name="items">Items list</param>
    /// <param name="alpha">Fields must be quoted</param>
    /// <param name="header">Header must be wirte</param>
    public void Serialize(IEnumerable<T> items, bool alpha = true, bool header = true)
    {
        if (items is not null)
        {
            WritePreface();
            WriteDataAsCsvWriter(items, header, alpha);
            _writer.Flush();
        }
    }

    /// <summary>
    /// Deserialize Csv file to items list
    /// </summary>
    /// <param name="header">File contains coluns header</param>
    public IEnumerable<T> Deserialize(bool header = true)
    {
        return FillItems(header);
    }

    /// <summary>
    /// Deserialize Csv file to items list
    /// </summary>
    /// <param name="alpha">Data framed by quotes</param>
    /// <param name="header">File contains coluns header</param>
    [Obsolete("You should use the Deserialize method without the alpha parameter")]
    public IEnumerable<T> Deserialize(bool alpha = true, bool header = true)
    {
        return FillItems(header);
    }

    /// <summary>
    /// Deserialize Csv file to items list
    /// </summary>
    /// <param name="items">Items list</param>
    /// <param name="alpha">Data framed by quotes</param>
    /// <param name="header">File contains coluns header</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("You should use the Deserialize(header) method which return the collection.")]
    public void Deserialize(ref IList<T> items, bool alpha = true, bool header = true)
    {
        foreach (T item in FillItems(header))
        {
            items.Add(item);
        }
    }

    /// <summary>
    /// Deserialize Csv file to items list
    /// </summary>
    /// <param name="items">Items list</param>
    /// <param name="header">File contains coluns header</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deserialize(ref IList<T> items, bool header = true)
    {
        foreach (T item in FillItems(header))
        {
            items.Add(item);
        }
    }

    #endregion

    #region Fonction pour les enumerations (deserialisation)

    /// <summary>
    /// Read a "line" from the current stream 
    /// (a line can be splitted on multiple lines according to the quote character)
    /// </summary>
    /// <returns><c>true</c> is has process data. <c>false</c> is the end of stream</returns>
    private bool ReadLine()
    {
        string row;
        bool isInQuote = false;
        StringBuilder valueBuilder = new();
        int charIndex;

        // We initialize the table of fields
        Fields.Clear();
        // We read a line from the file
        row = _reader.ReadLine();
        if (row is null)
        {
            // Return false if we reach the end of the file
            return false;
        }
        while (true)
        {
            // We build a new table
            charIndex = 1;
            while (charIndex <= row.Length)
            {
                if (row[charIndex - 1] == _options.Delimiter)
                {
                    if (isInQuote)
                    {
                        if ((charIndex < row.Length) && (row[charIndex] == _options.Delimiter))
                        {
                            // Doubled quote character => keep one
                            valueBuilder.Append(_options.Delimiter);
                            charIndex += 1;
                        }
                        else
                        {
                            isInQuote = false;
                        }
                    }
                    else
                    {
                        isInQuote = (valueBuilder.Length == 0);
                        if (!isInQuote)
                        {
                            valueBuilder.Append(_options.Delimiter);
                        }
                    }
                }
                else if (row[charIndex - 1] == _options.FieldSeparator)
                {
                    if (!isInQuote)
                    {
                        Fields.Add(valueBuilder.ToString());
                        valueBuilder.Clear();
                    }
                    else
                    {
                        // If we are in a string delimiter we add the value
                        valueBuilder.Append(_options.FieldSeparator);
                    }
                }
                else
                {
                    valueBuilder.Append(row[charIndex - 1]);

                }
                charIndex++;
            }
            // We check if the quote has been closed before the end of the line
            if (!isInQuote)
            {
                break;
            }
            // There is a line break in the field
            valueBuilder.AppendLine();
            // We load the next line (in continuity of the current line)
            row = _reader.ReadLine();
        }
        Fields.Add(valueBuilder.ToString());
        return true;
    }

    /// <summary>
    /// Fill in a list of items from the rows of the CSV file.
    /// </summary>
    /// <param name="header">File contains coluns header</param>
    private IEnumerable<T> FillItems(bool header)
    {
        List<IExportColumn<T>> columns = new();
        // Read the CSV header
        if (header)
        {
            if (ReadLine())
            {
                foreach (string title in Fields)
                {
                    columns.Add(Columns.FirstOrDefault(x => title.Equals(x.ColumnTitle, StringComparison.OrdinalIgnoreCase)));
                }
            }
        }
        else
        {
            columns.AddRange(Columns);
        }
        // Loop on each rows
        if (columns.Count > 0)
        {
            while (ReadLine())
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                ArrayList fieldsList = FieldsArray(columns);
                for (int l_i = 0; l_i <= fieldsList.Count - 1; l_i++)
                {
                    if (columns.ElementAtOrDefault(l_i) is not null)
                    {
                        object fieldValue = fieldsList[l_i];
                        columns[l_i]?.SetValue(item, fieldValue);
                    }
                }
                yield return item;
            }
        }
    }

    /// <summary>
    /// Returns a array list containing all the fields converted to the correct type.
    /// </summary>
    /// <param name="columns">cols definition from model</param>
    /// <returns></returns>
    private ArrayList FieldsArray(List<IExportColumn<T>> columns)
    {
        ArrayList fields = new();
        int idx;

        for (idx = 0; idx <= Fields.Count - 1; idx++)
        {
            IExportColumn<T> element = columns.ElementAtOrDefault(idx);
            if (element is not null)
            {
                switch (Type.GetTypeCode(Nullable.GetUnderlyingType(element.ValueType) ?? element.ValueType))
                {
                    case TypeCode.Double:
                    case TypeCode.Single:
                        {
                            fields.Add(GetDouble(idx));
                            break;
                        }

                    case TypeCode.Boolean:
                        {
                            fields.Add(GetBool(idx));
                            break;
                        }

                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        {
                            fields.Add(GetInt(idx));
                            break;
                        }
                    case TypeCode.SByte:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                        {
                            fields.Add(GetLong(idx));
                            break;
                        }

                    case TypeCode.DateTime:
                        {
                            fields.Add(GetDate(idx));
                            break;
                        }
                    case TypeCode.Object:
                        {
                            if (element.ValueType == typeof(Guid))
                            {
                                fields.Add(GetGuid(idx));
                            }
                            else
                            {
                                // Other types
                                TypeConverter converter = TypeDescriptor.GetConverter(element.ValueType);
                                object convertedvalue = converter.ConvertFrom(Fields[idx]);
                                fields.Add(convertedvalue);
                            }
                            break;
                        }
                    default:
                        {
                            fields.Add(GetString(idx));
                            break;
                        }
                }
            }
            else
            {
                // We keep the same number of elements as columns
                fields.Add(element);
            }
        }
        return fields;
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
    public static byte[] SerializeToByteArray(IEnumerable<T> items, ExportColumnCollection<T> exportColumns = null,
        CsvOptions options = null)
    {
        using (MemoryStream stream = new())
        {
            using (CsvDocument<T> XlsxSerialized = new(stream, options, exportColumns))
            {
                // Write Xslx content to memory
                XlsxSerialized.Serialize(items, true, true);
                // Return the file content
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }

    #endregion
}

