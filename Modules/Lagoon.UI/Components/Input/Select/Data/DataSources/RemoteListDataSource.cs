namespace Lagoon.UI.Components.Input.Select.Data.DataSources;

internal class RemoteListDataSource
{
    //TODO Alix LgSelect - RemoteListDataSource


    #region fields

    //private readonly string _controllerUri;

    #endregion

    ///// <summary>
    ///// New instance from a controller URI.
    ///// </summary>
    ///// <param name="controllerUri">The controller URI.</param>
    //public ListDataSourceBase(string controllerUri)
    //{
    //    _hasAsyncLoading = true;
    //    _controllerUri = controllerUri;
    //}

    ///// <summary>
    ///// Get item from IQueryable controller.
    ///// </summary>
    ///// <param name="onlySelected">Indicate that only selected items must be returned.</param>
    ///// <param name="searchedText">If <paramref name="onlySelected"/> is <c>false</c>, filter to apply to the items.</param>
    ///// <param name="cancellationToken">Token to know if the task must be cancelled.</param>
    ///// <returns>Items to be rendered.</returns>
    //private async Task<IEnumerable<TItemValue>> GetControllerItemsAsync(bool onlySelected, string searchedText, CancellationToken cancellationToken)
    //{
    //    IQueryable<TItemValue> query = AuthenticatedHttpClient.GetRemoteQuery<TItemValue>(ControllerUri);
    //    if (onlySelected)
    //    {
    //        query = WhereInSelection(query);
    //    }
    //    else if (searchedText != "")
    //    {
    //        query = query.Where(i => i != null);
    //        // We don't use GetCalculatedText because the CheckTranslate method isn't underdstand by OData
    //        if (GetItemText is null)
    //        {
    //            query = query.Where(i => i.ToString().Contains(_searchedText, StringComparison.CurrentCultureIgnoreCase));
    //        }
    //        else
    //        {
    //            query = query.Where(i => GetItemText(i).Contains(_searchedText, StringComparison.CurrentCultureIgnoreCase));
    //        }
    //    }
    //    // Do the HTTP request
    //    cancellationToken.ThrowIfCancellationRequested();
    //    BatchResponse<IEnumerable<TItemValue>> response = await AuthenticatedHttpClient.BatchGetAsync<IEnumerable<TItemValue>>(query, cancellationToken);
    //    // Return items from response
    //    return response.Content;
    //}

}
