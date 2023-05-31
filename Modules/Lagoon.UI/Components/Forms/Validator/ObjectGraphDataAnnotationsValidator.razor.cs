using Lagoon.Shared.Validation;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace Lagoon.UI.Components.Forms;

/// <summary>
/// Recursive data annotation validator.
/// </summary>
public class ObjectGraphDataAnnotationsValidator : ComponentBase, IObjectGraphDataAnnotationsValidator
{

    private ValidationMessageStore _validationMessageStore;

    [CascadingParameter]
    internal EditContext EditContext { get; set; }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        _validationMessageStore = new ValidationMessageStore(EditContext);
        EditContext.OnValidationRequested += delegate
        {
            _validationMessageStore.Clear();
            ValidateObject(EditContext.Model, new HashSet<object>());
            EditContext.NotifyValidationStateChanged();
        };
        EditContext.OnFieldChanged += delegate (object sender, FieldChangedEventArgs eventArgs)
        {
            EditContext editContext = EditContext;
            ValidationMessageStore validationMessageStore = _validationMessageStore;
            FieldIdentifier fieldIdentifier = eventArgs.FieldIdentifier;
            ValidateField(editContext, validationMessageStore, in fieldIdentifier);
        };
    }

    void IObjectGraphDataAnnotationsValidator.ValidateObject(object value, HashSet<object> visited)
    {
        ValidateObject(value, visited);
    }

    internal void ValidateObject(object value, HashSet<object> visited)
    {
        if (value == null || !visited.Add(value))
        {
            return;
        }

        if (value is IEnumerable<object> enumerable)
        {
            int num = 0;
            foreach (object item in enumerable)
            {
                ValidateObject(item, visited);
                num++;
            }

            return;
        }

        List<ValidationResult> list = new();
        ValidateObject(value, visited, list);
        foreach (ValidationResult item2 in list)
        {
            if (!item2.MemberNames.Any())
            {
                ValidationMessageStore validationMessageStore = _validationMessageStore;
                FieldIdentifier fieldIdentifier = new(value, string.Empty);
                validationMessageStore.Add(in fieldIdentifier, item2.ErrorMessage);
                continue;
            }

            foreach (string memberName in item2.MemberNames)
            {
                FieldIdentifier fieldIdentifier2 = new(value, memberName);
                _validationMessageStore.Add(in fieldIdentifier2, item2.ErrorMessage);
            }
        }
    }

    private void ValidateObject(object value, HashSet<object> visited, List<ValidationResult> validationResults)
    {
        ValidationContext validationContext = new(value);
        validationContext.Items.Add(ComplexTypeValidation.ValidationContextValidatorKey, this);
        validationContext.Items.Add(ComplexTypeValidation.ValidatedObjectsKey, visited);
        Validator.TryValidateObject(value, validationContext, validationResults, validateAllProperties: true);
    }

    private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
    {
        PropertyInfo property = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName);
        if (property != null)
        {
            object value = property.GetValue(fieldIdentifier.Model);
            ValidationContext validationContext = new(fieldIdentifier.Model)
            {
                MemberName = property.Name
            };
            List<ValidationResult> list = new();
            Validator.TryValidateProperty(value, validationContext, list);
            messages.Clear(in fieldIdentifier);
            messages.Add(in fieldIdentifier, list.Select((ValidationResult result) => result.ErrorMessage));
            editContext.NotifyValidationStateChanged();
        }
    }

}
