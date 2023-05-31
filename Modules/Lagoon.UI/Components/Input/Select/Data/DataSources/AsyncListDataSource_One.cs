namespace Lagoon.UI.Components;

/// <summary>
/// Provide the list of items to fill a component.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract class AsyncListDataSource<TValue> : AsyncListDataSource<TValue, TValue> where TValue : class
{

}

