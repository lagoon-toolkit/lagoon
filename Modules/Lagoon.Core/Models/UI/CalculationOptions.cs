namespace Lagoon.Helpers.Data;

/// <summary>
/// Calculation data definition
/// </summary>
public class CalculationOption
{
    /// <summary>
    /// Gets or sets field name used in calculation
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the expression representing the value.
    /// </summary>
    [JsonIgnore]
    public LambdaExpression ParametrizedValueExpression { get; set; }

    /// <summary>
    /// Gets or sets calculation function
    /// </summary>
    public DataCalculationType CalculationType { get; set; } = DataCalculationType.None;

}
