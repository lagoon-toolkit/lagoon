namespace Lagoon.Shared.Validation;

/// <summary>
/// Interface implemented by the ObjectGraphDataAnnotationsValidator.
/// </summary>
public interface IObjectGraphDataAnnotationsValidator
{

    /// <summary>
    /// Validate the value for the component.
    /// </summary>
    void ValidateObject(object value, HashSet<object> visited);

}
