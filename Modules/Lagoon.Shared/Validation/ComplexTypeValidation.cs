namespace Lagoon.Shared.Validation;

/// <summary>
/// 
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ComplexTypeValidation
{

    /// <summary>
    /// The key to retrive the ObjectGraphDataAnnotationsValidator component.
    /// </summary>
    public static readonly object ValidationContextValidatorKey = new();

    /// <summary>
    /// The key to retrive the visited hashset.
    /// </summary>
    public static readonly object ValidatedObjectsKey = new();

}
