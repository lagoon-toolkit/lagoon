namespace Lagoon.Shared.Model;

/// <summary>
/// Data query http response content
/// </summary>
/// <typeparam name="T"></typeparam>    
public class DataQueryResponse<T>
{
    /// <summary>
    /// Gets or sets data result
    /// </summary>
    public IEnumerable<T> Data { get; set; }

    /// <summary>
    /// Gets or sets calculation result
    /// </summary>
    public Dictionary<string, object> Calculations { get; set; }

    /// <summary>
    /// Gets or sets data count
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public DataQueryResponse()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data"></param>
    /// <param name="calculation"></param>
    /// <param name="count"></param>    
    public DataQueryResponse(IEnumerable<T> data, Dictionary<string, object> calculation, int count)
    {            
        Data = data is not null ? data : new List<T>();
        Calculations = calculation;
        Count = count;
    }

}
