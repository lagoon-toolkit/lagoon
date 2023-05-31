namespace Lagoon.Helpers.Data;

/// <summary>
/// Sort data definition
/// </summary>
public class SortOption
{

    /// <summary>
    /// Gets or sets the expression representing the function that return the wanted value with the item as parameter.
    /// </summary>
    [JsonIgnore]
    public LambdaExpression ParameterizedValueExpression { get; set; }

    /// <summary>
    /// Gets or sets the sort expression in string format
    /// </summary>
    public string StringifiedValueExpression { get; set; }

    /// <summary>
    /// Gets or sets sort direction
    /// </summary>
    public DataSortDirection Direction { get; set; }

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="sortExpression">The sort value expression.</param>
    /// <param name="direction">The sort direction.</param>
    public SortOption(LambdaExpression sortExpression, DataSortDirection direction = DataSortDirection.Asc)
    {
        ParameterizedValueExpression = sortExpression;
        StringifiedValueExpression = ParameterizedValueExpression.ToString();
        Direction = direction;
    }

    /// <summary>
    /// Constructor for unserialize class
    /// </summary>
    /// <param name="stringifiedValueExpression"></param>
    /// <param name="direction"></param>
    [JsonConstructor]
    public SortOption(string stringifiedValueExpression, DataSortDirection direction = DataSortDirection.Asc)
    {
        StringifiedValueExpression = stringifiedValueExpression;
        Direction = direction;
    }

    #endregion

}
