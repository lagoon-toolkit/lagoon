namespace Lagoon.UI.Components;

/// <summary>
/// Language change
/// </summary>
public class LanguageChangedEventArgs : EventArgs
{

    #region properties

    /// <summary>
    /// Get the new culture name. ("en-GB", fr-FR", ...)
    /// </summary>
    public string CultureName { get; }

    /// <summary>
    /// Get the new language code. ("en", fr", ...)
    /// </summary>
    public string TwoLetterISOLanguageName { get; }

    #endregion

    #region constructors

    /// <summary>
    /// Initialize new instance.
    /// </summary>
    public LanguageChangedEventArgs()
    {
        CultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;
        TwoLetterISOLanguageName = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    }

    #endregion

}
