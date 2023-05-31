using System.ComponentModel.DataAnnotations;

namespace Lagoon.Shared.Validation;


/// <summary>
/// A System.ComponentModel.DataAnnotations.ValidationAttribute that indicates that
/// the property is a complex or collection type that further needs to be validated.
/// By default System.ComponentModel.DataAnnotations.Validator does not recurse in
/// to complex property types during validation. When used in conjunction with Microsoft.AspNetCore.Components.Forms.ObjectGraphDataAnnotationsValidator,
/// this property allows the validation system to validate complex or collection
/// type properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ValidateComplexTypeAttribute : ValidationAttribute
{

    ///<inheritdoc/>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        TryValidateRecursive(value, validationContext);
        return ValidationResult.Success;
    }

    private static bool TryValidateRecursive(object value, ValidationContext validationContext)
    {
        if (validationContext.Items.TryGetValue(ComplexTypeValidation.ValidationContextValidatorKey, out object value2))
        {
            if (value2 is IObjectGraphDataAnnotationsValidator objectGraphDataAnnotationsValidator)
            {
                HashSet<object> visited = (HashSet<object>)validationContext.Items[ComplexTypeValidation.ValidatedObjectsKey];
                objectGraphDataAnnotationsValidator.ValidateObject(value, visited);
                return true;
            }
        }
        return false;
    }


}
