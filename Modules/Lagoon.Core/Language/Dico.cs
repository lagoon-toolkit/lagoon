namespace Lagoon.Core.Language;


/// <summary>
/// Dictionary language management
/// </summary>
internal class Dico
{

    #region Private properties

    /// <summary>
    /// Cultures (indexed by language: fr / en / ...)
    /// </summary>
    private readonly Dictionary<string, string> _cultures = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Dictionnaries (indexed by language: fr / en / ...)
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, string>> _dictionnaries = new();

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="appAssembly">Main app assembly.</param>
    public Dico(Assembly appAssembly)
    {
        Stream resStream = appAssembly.GetManifestResourceStream("Lagoon.Dico")
            ?? throw new Exception($"The \"Lagoon.Dico\" resource is not found in {appAssembly?.GetName()}.");
        using (resStream)
        {
            // Use UTF-16 (Unicode) to handle Asian Characters
            using (StreamReader sr = new(resStream, Encoding.Unicode))
            {
                // Load the culture dictionnary
                StringBuilder cultures = new();
                char c;
                do
                {
                    c = (char)sr.Read();
                    cultures.Append(c);
                } while (c != '}');
                _cultures = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(cultures.ToString());
                // Load the translations dictionnary
                string json = sr.ReadToEnd();
                _dictionnaries = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
            }
        }
    }

#endregion

#region Public methods

    /// <summary>
    /// Get a string from languages dictionnary.
    /// </summary>
    /// <param name="languageKey">Dico language to use</param>
    /// <param name="dicoKey">Dico key to translate</param>
    public string GetTranslation(string languageKey, string dicoKey)
    {
        if (_dictionnaries.TryGetValue(languageKey, out Dictionary<string, string> lngDico) && lngDico.TryGetValue(dicoKey, out string value))
        {
            // Return the translated text
            return value;
        }
        else
        {
            //Translation not found
            return string.Format("{0}?({1})", languageKey, dicoKey);
        }
    }

    /// <summary>
    /// Return the first defined culture.
    /// </summary>
    public string GetDefaultCulture()
    {
        string culture = _cultures.Values.FirstOrDefault();
        if (string.IsNullOrEmpty(culture))
        {
            string language = _cultures.Keys.FirstOrDefault();
            throw new Exception($"The culture (\"id\") is not defined in the \"Dico.xml\" file for the \"{language}\" language.");
        }
        return culture;
    }

    /// <summary>
    /// Try to return the culture corresponding to the language.
    /// </summary>
    /// <param name="language">The language.</param>
    public string TryGetCulture(string language)
    {
        return string.IsNullOrEmpty(language) ? null : _cultures.TryGetValue(language, out string culture) ? culture : null;
    }

    /// <summary>
    /// Return the list of loaded languages (fr, en, ...)
    /// </summary>
    public IEnumerable<string> GetLanguages()
    {
        return _dictionnaries.Keys;
    }

    /// <summary>
    /// Return the list of loaded culture (fr-FR, en-US, ...)
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<string> GetCultures()
    {
        return _cultures?.Values ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Return all language keys used (or loaded by the application)
    /// </summary>
    /// <returns>All language</returns>
    public IEnumerable<string> GetAllKeys()
    {
        return _dictionnaries.Values.SelectMany(t => t.Keys).Distinct();
    }

#endregion

#region Private methods

    /// <summary>
    /// Add additionnal dictionnary or overload existing key at runtime. 
    /// </summary>
    /// <param name="additionnalDico"></param>
    internal void LoadAdditionnalDico(Dictionary<string, Dictionary<string, string>> additionnalDico)
    {
        foreach (KeyValuePair<string, Dictionary<string, string>> additionnalEntry in additionnalDico)
        {
            if (!_dictionnaries.TryGetValue(additionnalEntry.Key, out Dictionary<string, string> dico))
            {
                dico = new Dictionary<string, string>();
                _dictionnaries.Add(additionnalEntry.Key, dico);
            }
            foreach (KeyValuePair<string, string> entry in additionnalEntry.Value)
            {
                if (dico.ContainsKey(entry.Key))
                {
                    dico[entry.Key] = entry.Value;
                }
                else
                {
                    dico.Add(entry.Key, entry.Value);
                }
            }
        }
    }

#endregion

}
