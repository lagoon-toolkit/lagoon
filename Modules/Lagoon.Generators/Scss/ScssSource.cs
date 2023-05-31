namespace Lagoon.Generators;


public class ScssSource
{

    #region private fields

    /// Contenu du fichier Scss.
    private readonly char[] _content;
    /// Position de la lecture.
    private int _position = -1;
    /// Longueur de la chaine de caractères.
    private readonly int _length;

    #endregion

    #region properties

    public string WorkingPath { get; }
    public string WorkingRootUri { get; }

    #endregion

    #region constructor

    /// <summary>
    /// Initialisation d'une nouvelle instance de la classe.
    /// </summary>
    /// <param name="p_s_Scss">Contenu du fichier Scss.</param>
    /// <param name="p_s_folder">Dossier contenant le fichier Scss.</param>
    /// <param name="rootUri">Uri to replace "~/".</param>
    public ScssSource(string p_s_Scss, string p_s_folder, string rootUri)
    {
        _content = CleanUp(p_s_Scss).ToCharArray();
        _length = _content.Length;
        WorkingPath = p_s_folder;
        WorkingRootUri = rootUri;
    }

    #endregion

    #region methods

    /// <summary>
    /// Enleve les commentaires, les espaces inutiles et les retour à la ligne d'un fichier CSS
    /// </summary>
    /// <param name="p_s_text">Contenu d'un fichier CSS</param>
    /// <returns></returns>
    private static string CleanUp(string p_s_text)
    {
        var l_sb = new StringBuilder(p_s_text.Length);
        char l_c_ignoreUntil;
        char l_c_added;
        char l_c_prec;
        char l_c;
        bool l_b_add;

        // On supprime les caractères inutiles
        l_sb.Length = 0;
        l_c_ignoreUntil = ControlChars.NullChar;
        l_b_add = true;

        for (int l_i = 0; l_i <= p_s_text.Length - 1; l_i++)
        {
            // On récupère les caractères
            l_c = p_s_text[l_i];
            if (l_i > 0)
                l_c_prec = p_s_text[l_i - 1];
            else
                l_c_prec = ' ';
            // On récupère le caractère précédement ajouté à la liste
            if (l_sb.Length > 0)
                l_c_added = l_sb[l_sb.Length - 1];
            else
                l_c_added = ' ';
            // On ne travaille qu'avec un seul caractère de retour à la ligne
            if (l_c == ControlChars.Lf)
                l_c = ControlChars.Cr;
            // On regarde si on est dans une chaine, un commentaire...
            if (l_c_ignoreUntil == ControlChars.NullChar)
            {
                switch (l_c)
                {
                    case ControlChars.Cr:
                    case ControlChars.Tab:
                        {
                            // On remplace les retour à la ligne et les tabulation par des espaces
                            l_c = ' ';
                            break;
                        }

                    case ControlChars.Quote:
                    case '\'':
                        {
                            // On entre dans une chaine
                            l_c_ignoreUntil = l_c;
                            break;
                        }
                }
                if (l_c_added == '/')
                {
                    if (l_c == '/')
                    {
                        // Commentaire de ligne
                        l_c_ignoreUntil = ControlChars.Cr;
                        l_sb.Length -= 1;
                        l_b_add = false;
                        continue;
                    }
                    else if (l_c == '*')
                    {
                        // Bloc de commentaire
                        l_c_ignoreUntil = '/';
                        l_sb.Length -= 1;
                        l_b_add = false;
                        continue;
                    }
                }
                // On supprime les espaces inutiles
                if (l_c_added == ' ' && " };:,<>=".IndexOf(l_c) != -1 && l_sb.Length > 0)
                    l_sb.Length -= 1;
                if (l_c == ' ' && "{;:,<>=".IndexOf(l_c_added) != -1)
                    continue;
                if (l_c_added == ';' && ";}".IndexOf(l_c) != -1)
                    l_sb.Length -= 1;
            }
            else if (l_c == l_c_ignoreUntil)
            {
                // On regarde si on sort effectivement d'un commentaire
                if (l_c != '/' || l_c_prec == '*')
                {
                    // On sort du bloc ouvert
                    l_c_ignoreUntil = ControlChars.NullChar;
                    // On sort d'un commentaire
                    if (!l_b_add)
                    {
                        l_b_add = true;
                        continue;
                    }
                }
            }
            // On ajoute caractère
            if (l_b_add)
                l_sb.Append(l_c);
        }
        return l_sb.ToString();
    }

    /// <summary>
    /// On se place sur le prochain caractère et on retourne <c>True</c>si il reste des caractères à charger.
    /// </summary>
    /// <returns><c>True</c>si il reste des caractères à charger.</returns>
    public bool MoveNext()
    {
        _position += 1;
        if (_position >= _length)
            return false;
        return true;
    }

    /// <summary>
    /// Retourne le caractère le position actuelle.
    /// </summary>
    /// <returns>Le caractère le position actuelle.</returns>
    public char GetChar()
    {
        return _content[_position];
    }
}

#endregion
