using System.Text.RegularExpressions;

namespace Lagoon.Core.Application.Logging;


/// <summary>
/// File logger options (output path, max size, ...)
/// </summary>
public class LgFileLoggerOptions
{

    #region properties

    /// <summary>
    /// Gets or sets the minimum level for which each message is directly flush to the file.
    /// The "None" value means : "Debug" if application is built in "DEBUG" configuration; else "Error".
    /// </summary>
    public LogLevel AutoFlushLevel { get; set; } = LogLevel.None;

    /// <summary>
    /// Number of days to keep trace files
    /// </summary>
    public int DayToKeep { get; set; } = 2;

    /// <summary>
    /// Is log activated ?
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Max number of files to store per day
    /// </summary>
    public int FileToKeep { get; set; } = 3;

    /// <summary>
    /// Path of the folder containing the log files
    /// </summary>
    public virtual string FolderPath { get; set; }

    /// <summary>
    /// The format of the log file.
    /// </summary>
    public LgFileLoggerFormat Format { get; set; } = LgFileLoggerFormat.IndentedJsonLines;

    /// <summary>
    /// Name of the file
    /// </summary>
    public virtual string LogFilename { get; set; }

    /// <summary>
    /// Max file size (supported unit K / M / G) 
    /// </summary>
    public string MaxFileSize
    {
        set => SetMaxFileSize(value);
    }

    /// <summary>
    /// The max file size.
    /// Default value : 5MB
    /// </summary>
    public long MaxFileSizeInByte { get; set; } = 5 * 1024 * 1024;


    /// <summary>
    /// Indicate if the connected user id must be trace in the log (if available).
    /// </summary>
    public bool ShowUser { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Get the complete path of the log file.
    /// </summary>
    /// <returns>The complete path of the log file.</returns>
    public string GetFilePath()
    {
        return Path.Combine(FolderPath, LogFilename);
    }

    /// <summary>
    /// Define the max file size from a string.
    /// </summary>
    /// <param name="value">The max file size.</param>
    private void SetMaxFileSize(string value)
    {
        // Check format (expected format : 1K | 1M | 1G)
        Regex r = new(@"^(\d)$?(K|M|G)");
        if (r.IsMatch(value.ToUpper()))
        {
            // Get groups
            Match match = r.Match(value.ToLower());
            if (int.TryParse(match.Groups[1].Value, out int size))
            {
                switch (match.Groups[2].Value)
                {
                    case "K":
                        MaxFileSizeInByte = size * 1024;
                        break;
                    case "M":
                        MaxFileSizeInByte = size * 1024 * 1024;
                        break;
                    case "G":
                        MaxFileSizeInByte = size * 1024 * 1024 * 1024;
                        break;
                }
            }
        }
        else
        {
            throw new Exception($"Unexpected MaxFileSize '{value}'. Must be in 'size''Unit' format (ex 5M, 200K, ...)");
        }
    }

    #endregion

}
