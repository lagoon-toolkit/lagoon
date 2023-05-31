using System.Diagnostics;

namespace Lagoon.Helpers;


/// <summary>
/// Utilitaries
/// </summary>
public class Tools
{

    /// <summary>
    /// Convert a value in byte to human readable format
    /// </summary>
    /// <param name="byteToConvert">Byte number</param>
    /// <param name="format">Format to apply</param>
    /// <returns></returns>
    public static string BytesToHumanReadable(long byteToConvert, string format = "{0:0.##} {1}")
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = byteToConvert;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return string.Format(format, len, sizes[order]);
    }

    /// <summary>
    /// Get the content-type corresponding to the file extension.
    /// </summary>
    /// <param name="filename">The file name.</param>
    /// <param name="default">The mime type to use if the extension is unknown.</param>
    /// <returns>The content-type corresponding to the file extension.</returns>
    public static string ExtrapolateContentType(string filename, string @default = "application/octet-stream")
    {
        return System.IO.Path.GetExtension(filename)?.ToLowerInvariant() switch
        {
            ".bmp" => "image/bmp",
            ".css" => "text/css",
            ".csv" => "text/x-csv",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".exe" => "application/x-msdownload",
            ".gif" => "image/gif",
            ".gz" => "application/gzip",
            ".htm" => "text/html",
            ".html" => "text/html",
            ".ico" => "image/x-icon",
            ".js" => "application/javascript",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".rtf" => "application/msword",
            ".tif" => "image/tiff",
            ".ttf" => "application/x-font-ttf",
            ".txt" => "text/plain",
            ".woff" => "application/font-woff",
            ".xml" => "text/xml",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".zip" => "application/zip",
            _ => @default
        };
    }

    /// <summary>
    /// Detect if the parameter is a file name or a mime type.
    /// </summary>
    /// <param name="fileNameOrContentType">The file name or the mime type.</param>
    /// <param name="defaultContentType">The default extrapolated mime type.</param>
    /// <returns></returns>
    /// <remarks>If the string contains a '/', it's a mime type.</remarks>
    public static string DetectFileNameOrContentType(ref string fileNameOrContentType, string defaultContentType = "application/octet-stream")
    {
        string fileName = null;
        if (fileNameOrContentType is null || !fileNameOrContentType.Contains('/'))
        {
            fileName = fileNameOrContentType;
            fileNameOrContentType = ExtrapolateContentType(fileName, defaultContentType);
        }
        return fileName;
    }

    /// <summary>
    /// Get the caller of a "Log..." method.
    /// </summary>
    /// <returns>The simplified log if ".Log" found.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetLogStackTrace(LogLevel logLevel, Exception exception)
    {
        // Extract the stack trace from the exception object
        string fullStack = exception?.ToString();
        if (fullStack is not null || logLevel < LogLevel.Error)
        {
            return fullStack;
        }
        // Extract the stack trace from the current method call stack trace
        fullStack = new StackTrace(1, true).ToString();
        int start = fullStack.LastIndexOf(".Log");
        if (start != -1)
        {
            start = fullStack.IndexOf('\n', start) + 1;
            if (start != 0)
            {
                int end = fullStack.IndexOf("\n", start);
                if (end != -1)
                {
                    return $"Log{logLevel}\n{fullStack[start..end]}";
                }
            }
        }
        return fullStack;
    }
}
