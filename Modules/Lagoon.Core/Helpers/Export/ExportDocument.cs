using System.Collections.Specialized;

namespace Lagoon.Helpers;

/// <summary>
/// Base class to exportable documents.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public abstract class ExportDocument<T> : IDisposable
{

    #region  fields

    /// <summary>
    /// Detect redundant calls
    /// </summary>
    private bool _disposed = false;

    #endregion

    #region  properties

    /// <summary>
    /// Fields list
    /// </summary>
    protected StringCollection Fields { get; } = new();

    /// <summary>
    /// ExportColumns
    /// </summary>
    protected ExportColumnCollection<T> Columns { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Load the PropertyInfo list for each column.
    /// </summary>
    /// <param name="exportColumns"></param>
    protected void InitializeColumnList(ExportColumnCollection<T> exportColumns)
    {
        // Load the list of column to export
        Columns = exportColumns ?? new ExportColumnCollection<T>(true);
    }


    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Close();
        }
        _disposed = true;
    }

    /// <summary>
    /// Free resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Open and close

    /// <summary>
    /// Opening a file
    /// </summary>
    protected abstract void Load();

    /// <summary>
    /// Close stream.
    /// </summary>
    protected abstract void Close();

    #endregion
}

