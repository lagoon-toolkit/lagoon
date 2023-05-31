using Lagoon.Core.Application;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Lagoon.Helpers;

/// <summary>
/// Helper extension methods
/// </summary>
public static partial class Extensions
{

    #region Extensions methods for the dictionnaries

    /// <summary>
    /// If key start with '#' then return the corresponding value from the dico (or #key?(languageKey) if not found in the dico)
    /// If not return key as value.
    /// </summary>
    /// <remarks>
    /// This function should only be used by component projects
    /// </remarks>
    /// <param name="key">Key to check</param>
    /// <param name="args">Replacement args for String.Format</param>
    /// <returns>The corresponding value from the dico if starting with '#', else return key as value</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string CheckTranslate(this string key, params string[] args)
    {
        return TranslationNeeded(key) ? LgApplicationBase.Current.Dico(key[1..], args) : key;
    }

    /// <summary>
    /// If key start with '#' then return the corresponding value from the dico (or #key?(languageKey) if not found in the dico)
    /// If not return key as value.
    /// </summary>
    /// <remarks>
    /// This function should only be used by component projects
    /// </remarks>
    /// <param name="key">Key to check</param>
    /// <param name="language">The target language to use</param>
    /// <param name="args">Replacement args for String.Format</param>
    /// <returns>The corresponding value from the dico if starting with '#', else return key as value</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string CheckTranslateFromLanguage(this string key, string language, params string[] args)
    {
        return TranslationNeeded(key) ? LgApplicationBase.Current.DicoFromLanguage(language, key[1..], args) : key;
    }

    /// <summary>
    /// Indicate if the text must be translated.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns>If the text must be translated.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TranslationNeeded(this string key)
    {
        return key is not null && key.StartsWith("#");
    }

    #endregion

    #region Enum extensions

    /// <summary>
    /// Get the display name for an Enum. If the display name attribute starts with '#', it's specify a dictionnary key.
    /// Ex : &lt;Display(Name="#MyKey")>
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <param name="translate">If <b>true</b>, translate the key of the dico with a '#' prefix.</param>
    /// <param name="language">The language to use.<c>null</c> to use the context language.</param>
    /// <returns>Return the DisplayAttribute Name if has one, else return the memeber name.</returns>
    public static string GetDisplayName(this Enum value, bool translate = true, string language = null)
    {
        string valueName = value.ToString();
        DisplayAttribute attribute = value.GetType().GetField(valueName)?.GetCustomAttribute<DisplayAttribute>();
        return attribute is null ? valueName : translate ? attribute.GetName().CheckTranslateFromLanguage(language) : attribute.GetName();
    }

    /// <summary>
    /// Gets the display attribute of an Enum.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>Return the Display attribute or null.</returns>
    public static DisplayAttribute GetDisplayAttribute(this Enum value)
    {
        return value.GetType().GetField(value.ToString())?.GetCustomAttribute<DisplayAttribute>();
    }

    #endregion

    #region JSON extensions

    /// <summary>
    /// Convert a boolean to a string representing the JSON (or javascript) boolean value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <returns>The string representing the JSON boolean value.</returns>
    public static string JsonEncode(this bool value)
    {
        return value ? "true" : "false";
    }

    #endregion

    #region Collation

    /// <summary>
    /// Return compare info options following the collation type
    /// </summary>
    /// <param name="collationType"></param>
    /// <returns></returns>
    public static CompareOptions ToCompareOptions(this CollationType collationType)
    {
        return collationType switch
        {
            // Case and accent insensitive
            CollationType.IgnoreCaseAndAccent => CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace,
            //Case insensitive and accent sensitive
            CollationType.IgnoreCase => CompareOptions.IgnoreCase,
            _ => CompareOptions.None,
        };
    }

    /// <summary>
    /// Remove accent from string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(this string value)
    {
        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    #endregion

}
