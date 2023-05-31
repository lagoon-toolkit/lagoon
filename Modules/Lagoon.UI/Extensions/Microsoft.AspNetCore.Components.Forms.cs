namespace Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// Extensions methods
/// </summary>
public static class Extensions
{

    /// <summary>
    /// Mark all fields of the EditContext as 'Unmodified' and signal the validation state has changed
    /// </summary>
    /// <param name="editContext">Context to reset</param>
    public static void ResetFormTracker(this EditContext editContext)
    {
        editContext.MarkAsUnmodified();
        editContext.NotifyValidationStateChanged();
    }

}