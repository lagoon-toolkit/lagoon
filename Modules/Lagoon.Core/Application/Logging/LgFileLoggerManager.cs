using System.Text.Encodings.Web;

namespace Lagoon.Core.Application.Logging;

/// <summary>
/// Server-side logger. Log all exception (except UserException) to a file
/// </summary>
public class LgFileLoggerManager : IDisposable
{
    #region static fields

    /// <summary>
    /// Lock to handle concurent file access
    /// </summary>
    private static readonly object _logStreamLock = new();

    /// <summary>
    /// Keep a reference to the instance of actual LgFileLogger
    /// </summary>
    private static LgFileLoggerManager _currentManager;

    #endregion

    #region fields

    /// <summary>
    /// Current date
    /// </summary>
    private double _currentOaDate;

    /// <summary>
    /// Path to the log file
    /// </summary>
    private string _filePath;

    /// <summary>
    /// Indicate if the JSON use a custom idented format.
    /// </summary>
    private bool _indented;

    // We use the JSON lines format (https://jsonlines.org/)
    private JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false, //Required for JSON lines format
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping //The quotation mark is encoded as \" rather than \u0022
    };

    /// <summary>
    /// Stream used by the JSONWriter
    /// </summary>
    private StreamWriter _streamWriter;

    #endregion

    #region properties

    /// <summary>
    /// Logger options
    /// </summary>
    public LgFileLoggerOptions Options { get; }

    #endregion

    #region Initialization

    /// <summary>
    /// New File logger
    /// </summary>
    /// <param name="options">Logger options</param>
    public LgFileLoggerManager(LgFileLoggerOptions options)
    {
        Options = options;
        _indented = Options.Format != LgFileLoggerFormat.JsonLines;
        _filePath = Options.GetFilePath();
        // Ensure directory exists
        Directory.CreateDirectory(Options.FolderPath);
        _currentManager = this;
    }

    #endregion

    #region Log stream intialization & Cyclic files management

    /// <summary>
    /// Init log stream
    /// </summary>
    private void InitLogStream()
    {
        int day = (int)DateTime.Now.ToOADate();
        bool isNew;

        // Check if the stream is already open
        if (_streamWriter is null)
        {
            // Check if log file exist
            isNew = !File.Exists(_filePath);
            if (!isNew)
            {
                // Check log retention day & max size
                isNew = Options.DayToKeep != 0 && day != (int)File.GetLastWriteTime(_filePath).ToOADate();
                if (!isNew)
                {
                    isNew = Options.MaxFileSizeInByte > 0 && new FileInfo(_filePath).Length > Options.MaxFileSizeInByte;
                }
            }
            if (!isNew)
            {
                try
                {
                    // Open the stream to the log file
                    OpenStreamWriter(FileMode.Append);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An exception occured while openning the log file : {ex.Message}");
                }
            }
        }
        else
        {
            // If the number of days of retention is exceeded or the file size exceeds the limit, close the stream
            isNew = Options.DayToKeep != 0 && day != _currentOaDate;
            if (!isNew)
            {
                isNew = Options.MaxFileSizeInByte > 0 && _streamWriter.BaseStream.Position > Options.MaxFileSizeInByte;
            }
            if (isNew)
            {
                _streamWriter.Dispose();
                _streamWriter = null;
            }
        }
        if (isNew)
        {
            try
            {
                // Keep log file according to the config.
                BackupLog();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occured while renaming old log files : {ex.Message}");
            }
            try
            {
                // Open a new log file
                OpenStreamWriter(FileMode.Create);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occured while openning the log file: {ex.Message}");
            }
        }
        _currentOaDate = day;
    }

    private void OpenStreamWriter(FileMode fileMode)
    {
        FileStream fileStream = new(_filePath, fileMode, FileAccess.Write, FileShare.Read, 64 * 1024);
        _streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
    }

    /// <summary>
    /// Remove old log files
    /// </summary>
    private void BackupLog()
    {
        lock (_logStreamLock)
        {
            // Remove old log file
            if (Options.DayToKeep > 0 && File.Exists(BackupFileName(Options.DayToKeep)))
            {
                File.Delete(BackupFileName(Options.DayToKeep));
            }
            // Rename other log files
            for (int i = Options.DayToKeep - 1; i >= 0; i--)
            {
                if (File.Exists(BackupFileName(i)))
                {
                    File.Move(BackupFileName(i), BackupFileName(i + 1));
                }
            }
        }
    }

    /// <summary>
    /// Return a log file name
    /// </summary>
    /// <param name="index">Log index</param>
    /// <returns>Log file name</returns>
    private string BackupFileName(int index)
    {
        return index == 0
            ? _filePath
            : Path.GetDirectoryName(_filePath) + "\\" + Path.GetFileNameWithoutExtension(_filePath) + "~" + index.ToString() + Path.GetExtension(_filePath);
    }

    /// <summary>
    /// Return all application log files
    /// </summary>
    /// <returns>A list of file path</returns>
    private IEnumerable<string> GetLogFiles()
    {
        int indexOldFile = 1;
        string tempPath;
        // Current application log file
        if (File.Exists(_filePath))
        {
            yield return _filePath;
        }
        // Split trace path 
        string logDir = Path.GetDirectoryName(_filePath);
        string logFilename = Path.GetFileNameWithoutExtension(_filePath);
        string logExt = Path.GetExtension(_filePath);
        // Look for old trace files
        while (true)
        {
            tempPath = Path.Combine(logDir, $"{logFilename}~{indexOldFile}{logExt}");
            if (File.Exists(tempPath))
            {
                yield return tempPath;
            }
            else
            {
                break;
            }
            indexOldFile++;
        }
    }

    /// <summary>
    /// Concatenate all application log file content into a single gzipped memory stream
    /// </summary>
    public static MemoryStream ExtractLogFile()
    {
        if (_currentManager == null)
        {
            return null;
        }
        MemoryStream memory = new();
        lock (_logStreamLock)
        {
            // If the log stream is open, force a flush before exporting log content
            _currentManager._streamWriter?.Flush();
            // Use gzip compression
            using (System.IO.Compression.GZipStream gzip = new(memory, System.IO.Compression.CompressionMode.Compress, true))
            {
                // Get all application log files and write content to the gzip stream
                using StreamWriter sw = new(gzip, Encoding.UTF8);
                foreach (string logFilePath in _currentManager.GetLogFiles())
                {
                    using FileStream fs = new(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (StreamReader sr = new(fs, Encoding.UTF8, false))
                    {
                        sw.Write(sr.ReadToEnd());
                    }
                }
            }
            memory.Position = 0;
        }
        return memory;
    }

    /// <summary>
    /// Concatenate all application log file content into a list of errors
    /// </summary>
    /// <param name="filter">A string to filter the levels to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static IEnumerable<LogData> ExtractLogData(string filter = null, CancellationToken cancellationToken = default)
    {
        // rq: ToList() to allow MessagePack serialization
        return _currentManager.GetAllLogEntries(filter, cancellationToken).ToList();
    }

    /// <summary>
    /// Concatenate all application log file content into a list of errors
    /// </summary>
    /// <param name="filter">A string to filter the levels to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public IEnumerable<LogData> GetAllLogEntries(string filter = null, CancellationToken cancellationToken = default)
    {
        int pos;
        foreach (string logFilePath in _currentManager.GetLogFiles())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            StringBuilder entry = new();
            using (FileStream fs = new(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) is not null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        if (line.EndsWith('}'))
                        {
                            if (entry.Length > 0)
                            {
                                entry.AppendLine(line);
                                line = entry.ToString();
                                entry.Length = 0;
                            }
                            if (filter is not null)
                            {
                                pos = line.IndexOf("l\":");
                                if (pos == -1 || !filter.Contains(line[pos + 4]))
                                {
                                    continue;
                                }
                            }
                            yield return JsonSerializer.Deserialize<LogData>(line, _jsonOptions);
                        }
                        else
                        {
                            entry.AppendLine(line);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region ILogger implementation

    /// <summary>
    /// Write the event to the file.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="time">The time of the event.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="stackTrace">The stack trace associated to the message.</param>
    /// <param name="category">The category</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <param name="side">The application source of the event.</param>
    /// <param name="context">he current context informations.</param>
    public void Log(LogLevel logLevel, DateTime time, string message, string stackTrace = null,
        string category = null, bool isAppCategory = false, LogSide side = LogSide.Server, string context = null)
    {
        lock (_logStreamLock)
        {
            // Init log stream
            InitLogStream();
            // Write log entry and track actual file size
            if (_streamWriter is not null)
            {
                WriteLogEntry(_streamWriter, time, side, logLevel, message, stackTrace, category, isAppCategory, context);
                // Always flush in debug or errors
                if (logLevel >= Options.AutoFlushLevel)
                {
                    _streamWriter.Flush();
                }
            }
        }
    }

    /// <summary>
    /// Write a log entry to the file.
    /// </summary>
    /// <param name="writer">The output stream.</param>
    /// <param name="time">The time at which the exception was triggered.</param>
    /// <param name="side">The log source.</param>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message.</param>
    /// <param name="stackTrace">The stack trace.</param>
    /// <param name="categoryName">The category.</param>
    /// <param name="isAppCategory">Indicate if the category come from the current application.</param>
    /// <param name="context">he current context informations.</param>
    private void WriteLogEntry(StreamWriter writer, DateTime time, LogSide side, LogLevel level, string message,
        string stackTrace, string categoryName, bool isAppCategory, string context)
    {
        // Quick write to file (See the LogDataConverter...)
        writer.Write("{\"Level\":\"");
        writer.Write(level.ToString());
        writer.Write("\",\"Time\":\"");
        writer.Write(JavaScriptEncoder.UnsafeRelaxedJsonEscaping.Encode(time.ToString(LogDataConverter<LogData>.DATETIME_FORMAT)));
        if (side == LogSide.Client)
        {
            writer.Write("\",\"Side\":\"Client");
        }
        if (categoryName is not null)
        {
            if (isAppCategory)
            {
                writer.Write("\",\"App\":\"y");
            }
            writer.Write("\",\"Category\":\"");
            writer.Write(categoryName);
        }
        writer.Write("\"");
        if (!string.IsNullOrEmpty(context))
        {
            writer.Write(",\"Context\":");
            WriteMultiLineString(writer, context);
        }
        writer.Write(",\"Message\":");
        WriteMultiLineString(writer, message);
        if (!string.IsNullOrEmpty(stackTrace))
        {
            writer.Write(",\"Stack\":");
            WriteMultiLineString(writer, stackTrace);
        }
        writer.Write('}');
        if (_indented)
        {
            writer.WriteLine();
        }
    }

    /// <summary>
    /// Write a string as Array of lines.
    /// </summary>
    /// <param name="writer">The output stream.</param>
    /// <param name="value">The value to write.</param>
    private void WriteMultiLineString(StreamWriter writer, string value)
    {
        StringBuilder line = new();
        writer.Write('[');
        if (_indented)
        {
            writer.WriteLine();
            writer.Write('\t');
        }
        writer.Write('"');
        foreach (char c in value)
        {
            switch (c)
            {
                case '\r':
                    continue;
                case '\n':
                    writer.Write(JavaScriptEncoder.UnsafeRelaxedJsonEscaping.Encode(line.ToString()));
                    line.Length = 0;
                    writer.Write("\",");
                    if (_indented)
                    {
                        writer.WriteLine();
                        writer.Write('\t');
                    }
                    writer.Write('"');
                    break;
                default:
                    line.Append(c);
                    break;
            }
        }
        writer.Write(JavaScriptEncoder.UnsafeRelaxedJsonEscaping.Encode(line.ToString()));
        writer.Write('"');
        if (_indented)
        {
            writer.WriteLine();
        }
        writer.Write(']');
    }

    #endregion

    #region Free resources

    /// <summary>
    /// Freeing resources.
    /// </summary>
    /// <param name="disposing">Free the managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_streamWriter is not null)
            {
                _streamWriter.Dispose();
                _streamWriter = null;
            }
        }
    }

    /// <summary>
    /// Freeing resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
