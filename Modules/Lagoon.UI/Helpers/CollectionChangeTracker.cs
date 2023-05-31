using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Lagoon.UI.Helpers;

/// <summary>
/// Class to detect the changes on an observable collection or to detect when a collection is replaced by another.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CollectionChangeTracker<T> : IDisposable
{

    #region fields

    /// <summary>
    /// Indicate if an observable collection has been updated since the last call to "Track" method.
    /// </summary>
    private bool _changed;

    /// <summary>
    /// The reference to the last collection used by the "Track" method.
    /// </summary>
    private ICollection<T> _lastCollection;

    /// <summary>
    /// Method to call when the list has changed.
    /// </summary>
    private Action _updateCallback;

    /// <summary>
    /// Indicate that the tracker has been disposed.
    /// </summary>
    private bool _disposedValue;

    #endregion

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="updateCallback">Method called when the list changed.</param>
    public CollectionChangeTracker(Action updateCallback)
    {
        _updateCallback = updateCallback;
    }

    /// <summary>
    /// Check if the list
    /// </summary>
    /// <param name="collection"></param>
    public void Track(ICollection<T> collection)
    {
        if (!ReferenceEquals(collection, _lastCollection))
        {
            if (_lastCollection is ObservableCollection<T> listUnregister)
            {
                listUnregister.CollectionChanged -= OnCollectionChanged;
            }
            _lastCollection = collection;
            if (_lastCollection is ObservableCollection<T> listRegister)
            {
                listRegister.CollectionChanged += OnCollectionChanged;
            }
            _changed = false;
            _updateCallback();
        }
        else if (_changed)
        {
            _changed = false;
            _updateCallback();
        }
    }

    /// <summary>
    /// Event raise when selection collection has changed.
    /// </summary>
    /// <param name="sender">Source.</param>
    /// <param name="e">Event args.</param>
    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _changed = true;
    }

    /// <summary>
    /// Free event handlers.
    /// </summary>
    /// <param name="disposing">Indicate that the object is being disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_lastCollection is ObservableCollection<T> list)
                {
                    list.CollectionChanged -= OnCollectionChanged;
                }
            }
            _disposedValue = true;
        }
    }

    /// <summary>
    /// Free event handlers.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
