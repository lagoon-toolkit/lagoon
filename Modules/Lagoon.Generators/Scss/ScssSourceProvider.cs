namespace Lagoon.Generators;

/// <summary>
/// Fournisseur de contenu Scss.
/// </summary>
internal class ScssSourceProvider
{
    #region private fields

    private ScssSource _current;
    private readonly Stack<ScssSource> _stack = new();

    #endregion

    #region public properties

    public string WorkingPath { get => _current.WorkingPath; }

    public string WorkingRootUri { get => _current.WorkingRootUri; }

    #endregion

    #region constructor

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="T:System.Object" />.
    /// </summary>
    public ScssSourceProvider()
    {
    }

    /// <summary>
    /// Initialisation d'une nouvelle instance de la classe.
    /// </summary>
    /// <param name="p_s_Scss">Contenu du fichier Scss.</param>
    /// <param name="p_s_path">Gestionnaire de fichiers inclus.</param>
    /// <param name="rootUri">Uri to replace "~/".</param>

    public ScssSourceProvider(string p_s_Scss, string p_s_path, string rootUri)
    {
        _current = new ScssSource(p_s_Scss, p_s_path, rootUri);
    }

    #endregion

    #region methods

    public void Import(string relPath)
    {
        string fullPath = Path.Combine(WorkingPath, relPath);
        string directory = Path.GetDirectoryName(fullPath);
        if (!File.Exists(fullPath))
        {
            fullPath = Path.Combine(directory, "_" + Path.GetFileName(fullPath));
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(directory, Path.GetFileName(fullPath) + ".scss");
                if (!File.Exists(fullPath))
                {
                    fullPath = Path.Combine(WorkingPath, relPath) + ".scss";
                }
            }
        }
        Import(File.ReadAllText(fullPath), directory);
    }

    /// <summary>
    /// Ajout d'un contenu à l'emplacement actuelle de la lecture.
    /// </summary>
    /// <param name="p_s_Scss">Contenu du fichier Scss.</param>
    /// <param name="p_s_folder">Dossier contenant le fichier Scss.</param>
    public void Import(string p_s_Scss, string p_s_folder)
    {
        var rootUri = _current.WorkingRootUri;
        _stack.Push(_current);
        _current = new ScssSource(p_s_Scss, p_s_folder, rootUri);
    }

    /// <summary>
    /// On se place sur le prochain caractère et on retourne <c>True</c>si il reste des caractères à charger.
    /// </summary>
    /// <returns><c>True</c>si il reste des caractères à charger.</returns>
    public bool MoveNext()
    {
        do
        {
            if (_current.MoveNext())
            {
                return true;
            }

            if (_stack.Count == 0)
            {
                return false;
            }

            _current = _stack.Pop();
        }
        while (true);
    }

    /// <summary>
    /// Retourne le caractère le position actuelle.
    /// </summary>
    /// <returns>Le caractère le position actuelle.</returns>
    public char GetChar()
    {
        return _current.GetChar();
    }

    #endregion
}
