namespace Lagoon.UI.Components.Internal;

internal class AsyncLoadState<TItemValue>
{

    #region fields

    /// <summary>
    /// Asynchrone data loading is pending.
    /// </summary>
    private bool _isLoading;

    private string _searchText;
    private List<TItemValue> _unknownValues;

    #endregion

    #region properties

    /// <summary>
    /// Indicate if the last call ended with an error.
    /// </summary>
    public bool HasLoadingError { get; set; }

    /// <summary>
    /// Gets ors sets that an async load need to be done.
    /// </summary>
    public bool IsPending { get; set; }

    /// <summary>
    /// Asynchrone data loading is pending.
    /// </summary>
    public bool IsLoading => IsPending || _isLoading;

    /// <summary>
    /// Indicate the loading progression.
    /// </summary>
    public Progress Progress { get; set; }

    /// <summary>
    /// Cancellation token for the asynchrone loading.
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Call the async method to load items.
    /// </summary>
    /// <returns>The async method to load items.</returns>
    public async Task LoadAndProcessItemsAsync(LgComponentBase caller,
        Func<List<TItemValue>, string, Progress, CancellationToken, Task> onLoadItemsAsync,
        EventCallback<FilterChangeEventArgs>? onFilterChange, 
        string searchText, List<TItemValue> unknownValues = null)
    {
        while (IsPending)
        {
            // Keep the new search filters
            _searchText = searchText;
            _unknownValues = unknownValues;
            // Cancel the current loading to start a new one
            if (_isLoading)
            {
                CancellationTokenSource.Cancel();
                return;
            }
            // Start the async loading
            IsPending = false;
            // Initialise a token to cancel 
            CancellationTokenSource = new CancellationTokenSource();
            _isLoading = true;
            Progress = new("lgSelectLoading".Translate());
            try
            {
                // Reset the error message
                HasLoadingError = false;
                // Get a waiting context to cancel the loading if the page is close
                using (WaitingContext wc = caller.GetNewWaitingContext())
                {
                    if (wc is null)
                    {
                        await LoadItemsForCancellationTokenAsync(caller, onLoadItemsAsync, onFilterChange, CancellationTokenSource.Token);
                    }
                    else
                    {
                        // Link the loading token and the page token
                        using (CancellationTokenSource linkedToken =
                                    CancellationTokenSource.CreateLinkedTokenSource(CancellationTokenSource.Token, wc.CancellationToken))
                        {
                            await LoadItemsForCancellationTokenAsync(caller, onLoadItemsAsync, onFilterChange, linkedToken.Token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore the loading cancellation exceptions
            }
            catch (Exception ex)
            {
                HasLoadingError = true;
                caller.App.TraceException(ex);
            }
            finally
            {
                CancellationTokenSource.Dispose();
                Progress.ReportEnd();
                _isLoading = false;
            }
        }
    }

    /// <summary>
    /// Call the items loading with a specific cancellation token. 
    /// </summary>
    /// <param name="caller">The coponent source.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="onLoadItemsAsync">The method to load items.</param>
    /// <param name="onFilterChange">The "OnFilterChange" EventCallBack.</param>
    /// <returns></returns>
    private async Task LoadItemsForCancellationTokenAsync(LgComponentBase caller,
        Func<List<TItemValue>, string, Progress, CancellationToken, Task> onLoadItemsAsync,
        EventCallback<FilterChangeEventArgs>? onFilterChange,
        CancellationToken cancellationToken)
    {
        // Call the On filter change
        if (onFilterChange is not null)
        {
            await onFilterChange.Value.TryInvokeAsync(caller.App, new FilterChangeEventArgs(_searchText, cancellationToken, Progress));
        }
        if (onLoadItemsAsync is not null)
        {
            await onLoadItemsAsync(_unknownValues, _searchText, Progress, cancellationToken);
        }
    }

    #endregion

}
