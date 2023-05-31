namespace Lagoon.UI.Components;

/// <summary>
/// Parameters of the inputmask
/// </summary>
public class InputMaskOptions
{

    #region properties

    /// <summary>
    /// Gets or sets the input mask
    /// </summary>
    public string InputMask { get; set; }

    /// <summary>
    /// Gets or sets the input mask kind
    /// </summary>
    public InputMaskKind InputMaskKind { get; set; }

    /// <summary>
    /// Gets or sets the input mask in placeholder
    /// </summary>
    public string InputMaskPlaceholder { get; set; }

    /// <summary>
    /// Get or set the flag indicating if value should be returned without the mask.
    /// </summary>
    /// <example> phone number with mask '06-05-06-07-08' will ne returned as '0605060708' if <c>AutoUnmask</c> is set to <c>true</c></example>
    /// <value><c>false</c> by default</value>        
    public bool AutoUnmask { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Get the input mask attributes.
    /// </summary>
    /// <returns>The input mask attributes.</returns>
    public Dictionary<string, object> GetAttributes()
    {
        return new(EnumerateAttributes());
    }

    /// <summary>
    /// Enumerate the input mask attributes.
    /// </summary>
    /// <returns>The input mask attributes.</returns>
    public IEnumerable<KeyValuePair<string, object>> EnumerateAttributes()
    {
        yield return new(nameof(LgTextBox.InputMask), InputMask);
        yield return new(nameof(LgTextBox.InputMaskKind), InputMaskKind);
        yield return new(nameof(LgTextBox.InputMaskPlaceholder), InputMaskPlaceholder);
        yield return new(nameof(LgTextBox.AutoUnmask), AutoUnmask);
    }

    #endregion

}
