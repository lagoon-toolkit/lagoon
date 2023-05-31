namespace Lagoon.Server.Controllers;

/// <summary>
/// Model used to get data from remote data loader
/// </summary>
/// <typeparam name="T">Return type</typeparam>
public class QueryArg<T>
{

    #region properties

    /// <summary>
    /// Gets typed data value
    /// </summary>          
    public T Value { get; }

    #endregion

    #region methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value"></param>
    public QueryArg(T value)
    {
        Value = value;
    }

    #endregion
}