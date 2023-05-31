using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Components;

/// <summary>
/// Component to handle validation.
/// </summary>
public class LgValidator : LgValidationBase
{
    #region Private properties

    /// <summary>
    /// The list of errors to display.
    /// </summary>
    private ValidationMessageStore _messageStore;

    /// <summary>
    /// The current error list.
    /// </summary>
    private Dictionary<string, List<string>> _errors = new();

    #endregion

    #region Public properties

    /// <summary>
    /// Returns <c>true</c> if we have at least one error to display, <c>false</c> otherwise.
    /// </summary>
    public bool HasError => _errors.Any();

    /// <summary>
    /// Gets or sets the title displayed in the toastr error message
    /// </summary>
    [Parameter]
    public string Title { get; set; }

    #endregion

    #region Initialization

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        // Ensure 'CurrentEditContext' is accessible
        if (EditContext == null)
        {
            throw new InvalidOperationException(
                @$"{nameof(LgValidator)} requires a cascading
                       parameter of type {nameof(EditContext)}.
                       For example, you can use {nameof(LgValidator)}
                       inside an {nameof(EditForm)}.");
        }

        // Initializes an empty error list.
        _messageStore = new ValidationMessageStore(EditContext);
        // Handles validation & Field changed events.
        EditContext.OnValidationRequested += OnValidationRequested;
        EditContext.OnFieldChanged += OnFieldChanged;
    }

    #endregion

    #region Handle validation events

    /// <summary>
    /// Clears the errors list before new validation requests.
    /// </summary>
    private void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
        _errors.Clear();
        _messageStore.Clear();
        // Show error or generic error in toastr (according to configuration)
        ShowMessageInToastr();
    }

    /// <summary>
    /// Removes the errors for updated field.
    /// </summary>
    private void OnFieldChanged(object sender, FieldChangedEventArgs e)
    {
        // Clear custom validation message
        _errors.Remove(e.FieldIdentifier.FieldName);
        _messageStore.Clear(e.FieldIdentifier);
        // If instant toastr mode
        if (ActualErrorsDisplayOptions.HasFlag(EditFormErrorsDisplayOptions.ToastrOnBlur))
        {
            // Retrieve / show validation messages for this field
            ShowMessageList(EditContext.GetValidationMessages(e.FieldIdentifier));
        }
    }

    /// <summary>
    /// Displays the errors (or a generic message) if any in the application toastr.
    /// </summary>
    private void ShowMessageInToastr()
    {
        IEnumerable<string> validationMessages = EditContext.GetValidationMessages();
        if (validationMessages.Any())
        {
            // Toastr message
            if (ActualErrorsDisplayOptions.HasFlag(EditFormErrorsDisplayOptions.ToastrAllMessages))
            {
                ShowMessageList(validationMessages);
            }
            else if (ActualErrorsDisplayOptions.HasFlag(EditFormErrorsDisplayOptions.ToastrGenericMessage))
            {
                ShowError(string.Empty, string.IsNullOrEmpty(Title) ? "#lgEditFormValidation" : Title);
            }
        }
    }

    /// <summary>
    /// Show all validation messages in the toastr.
    /// </summary>
    /// <param name="messages">List of the messages.</param>
    private void ShowMessageList(IEnumerable<string> messages)
    {
        StringBuilder sb = null;
        foreach (string message in messages)
        {
            if (sb is null)
            {
                sb = new();
            }
            else
            {
                sb.AppendLine();
            }
            sb.Append("• ");
            sb.Append(App.BehaviorConfiguration.AliasMessages.GetDisplayMessage(message, false));
        }
        if (sb is not null)
        {
            ShowError(sb.ToString(), Title);
        }
    }

    #endregion

    #region Add / Clear errors list

    /// <summary>
    /// Displays the errors list.
    /// </summary>
    /// <param name="showInToastr">Errors to display</param>
    /// <exception cref="LgValidationException"></exception>
    public void ThrowPendingErrors(bool showInToastr = true)
    {
        if (!_errors.Any())
        {
            return;
        }

        // Link errors to ValidationMessage
        foreach (KeyValuePair<string, List<string>> err in _errors)
        {
            _messageStore.Add(EditContext.Field(err.Key), err.Value);
        }

        EditContext.NotifyValidationStateChanged();
        if (showInToastr)
        {
            ShowMessageInToastr();
        }
    }

    /// <summary>
    /// Clears all the errors.
    /// </summary>
    public void ClearErrors()
    {
        _errors.Clear();
        _messageStore.Clear();
        EditContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Adds an error message for a specific field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="errorMessage">The error to display (string or dico key).</param>
    public void AddError(string fieldName, string errorMessage)
    {
        if (!_errors.ContainsKey(fieldName))
        {
            _errors.Add(fieldName, new List<string>());
        }

        _errors[fieldName].Add(errorMessage.CheckTranslate());
    }

    /// <summary>
    /// Add a set of errors 
    /// </summary>
    /// <param name="errors">Errors to show</param>
    public void AddErrorList(Dictionary<string, List<string>> errors)
    {
        foreach (var entry in errors)
        {
            _errors.Add(entry.Key, new List<string>(entry.Value));
        }
    }

    /// <summary>
    /// Add a set of errors 
    /// </summary>
    /// <param name="errors">Errors to show</param>
    public void SetErrorList(Dictionary<string, List<string>> errors)
    {
        _errors.Clear();
        _messageStore.Clear();
        AddErrorList(errors);
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Dispose resources
    /// </summary> 
    protected override void Dispose(bool disposing)
    {
        EditContext.OnValidationRequested -= OnValidationRequested;
        EditContext.OnFieldChanged -= OnFieldChanged;
        base.Dispose(disposing);
    }

    #endregion
}