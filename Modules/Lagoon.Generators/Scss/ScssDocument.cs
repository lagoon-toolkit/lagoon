using System.Text.RegularExpressions;

namespace Lagoon.Generators;

/// <summary>
/// Classe permettant la création d'un fichier CSS à partir de plusieurs sources.
/// </summary>
public class ScssDocument
{

    #region fields

    private List<string> _ifParts;

    /// <summary>
    /// Longeur du contenu en caractères.
    /// </summary>
    private int _size;

    /// <summary>
    /// Liste des définitions de style.
    /// </summary>
    private List<string> _classParts = new();

    /// <summary>
    /// Liste des fonctions.
    /// </summary>
    private List<string> _mixinParts = new();

    /// <summary>
    /// Liste des variables
    /// </summary>
    private List<string> _varParts = new();

    /// Fournisseur de contenu Scss.
    private ScssSourceProvider _provider;

    #endregion

    #region constructors

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="T:System.Object" />.
    /// </summary>
    public ScssDocument()
    { }

    /// <summary>
    /// Initialisation d'une nouvelle instance de la classe.
    /// </summary>
    /// <param name="p_s_fileName">Chemin d'accès au fichier ".Scss".</param>
    /// <param name="rootUri">Uri to replace "~/".</param>
    public ScssDocument(string p_s_fileName, string rootUri) : this()
    {
        Load(p_s_fileName, rootUri);
    }

    #endregion

    #region methods

    /// <summary>
    /// Chargement du contenu du fichier.
    /// </summary>
    /// <param name="p_s_fileName">Nom du fichier.</param>
    /// <param name="rootUri">Uri to replace "~/".</param>
    public void Load(string p_s_fileName, string rootUri)
    {
        StartNewPart();
        string content = File.ReadAllText(p_s_fileName);
        LoadScss(content, Path.GetDirectoryName(p_s_fileName), rootUri);
    }

    /// <summary>
    /// Chargement du contenu du fichier.
    /// </summary>
    /// <param name="p_s_scssContent">Contenu du fichier Scss.</param>
    /// <param name="rootUri">Uri to replace "~/".</param>
    /// <param name="p_o_pathResolver">Gestionnaire permettant de récupérer le contenu Scss des @import.</param>
    public void LoadScss(string p_s_scssContent, string workingPath, string rootUri)
    {
        StringBuilder l_sb_buffer = new();
        char l_c_ignoreUntil = ControlChars.NullChar;
        int level = 0;
        char l_c;

        // On quitte si le document est vide
        if (p_s_scssContent == null)
        {
            return;
        }
        // On conserve le contexte pour le chargement
        _provider = new ScssSourceProvider(p_s_scssContent, workingPath, rootUri);
        // On interprète le contenu Scss
        while (_provider.MoveNext())
        {
            l_c = _provider.GetChar();
            // On regarde s'il s'agit du caractère BOM UTF8 qui se serait perdu en milieu de chaine
            // Sinon SharpScss déclenche une exception "Not enough space"
            if (l_c == '\uFEFF')
                continue;
            // On regarde si on est dans une chaine
            if (l_c_ignoreUntil == ControlChars.NullChar)
            {
                switch (l_c)
                {
                    case ';':
                        if (level == 0)
                        {
                            EndPart(l_sb_buffer, l_c);
                            continue;
                        }
                        break;
                    case '{':
                        level++;
                        break;
                    case '}':
                        level--;
#if DEBUG
                        if (level < 0)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
#endif
                        if (level <= 0)
                        {
                            EndPart(l_sb_buffer, l_c);
                            continue;
                        }
                        break;
                    case ControlChars.Quote:
                    case '\'':
                        {
                            l_c_ignoreUntil = l_c;
                            break;
                        }
                }
            }
            else if (l_c == l_c_ignoreUntil)
            {
                // On sort du bloc ouvert
                l_c_ignoreUntil = ControlChars.NullChar;
            }
            if (l_sb_buffer.Length != 0 || l_c != ' ')
            {
                l_sb_buffer.Append(l_c);
            }
        }
    }

    /// <summary>
    /// Une fois qu'on trouve un séparateur, on traite ce qui a été mis dans le buffer.
    /// </summary>
    /// <param name="p_o_node">Noeud.</param>
    /// <param name="p_sb">Buffer.</param>
    private void EndPart(StringBuilder p_sb, char endChar)
    {
        // On ajoute le dernier caractère
        p_sb.Append(endChar);
        // On remplace l'URL "~/" par l'URL du module            
        p_sb.Replace("~/", _provider.WorkingRootUri);
        // On ignore la commande !default du SASS -> Toutes les variables sont considérées comme default!
        TrimVariableDefault(p_sb);
        // On récupère la partie
        string part = p_sb.ToString();
        // On regarde s'il s'agit d'un import
        if (part.StartsWith("@import "))
        {                
            foreach (string relPath in GetImportNames(part).Reverse())
            {
                _provider.Import(relPath);
            }
        }
        else
        {
            if (part[0] == '$')
            {
                _varParts.Add(part);
            }
            else if (part.StartsWith("@function") || part.StartsWith("@mixin"))
            {
                _mixinParts.Add(part);
            }
            else if (part.StartsWith("@if"))
            {
                //rq: les if/else peuvent contenir des surcharges de variables
                int pos = part.IndexOf('{') + 1;
                _ifParts = pos < part.Length && part[pos] == '$' ? _varParts : _classParts;
                _ifParts.Add(part);
            }
            else if (part.StartsWith("@else"))
            {
                _ifParts.Add(part);
            }
            else
            {
                _classParts.Add(part);
            }
            // On incremente la taille totale
            _size += part.Length;
        }
        // On vide le buffer
        p_sb.Length = 0;
    }

    private void StartNewPart()
    {
        _ifParts = _classParts;
    }

    private static void TrimVariableDefault(StringBuilder p_sb)
    {
        if (p_sb[0] == '$' && EndsWithDefault(p_sb, p_sb.Length))
        {
            p_sb.Length -= 10;
            p_sb.Append(';');
        }
    }

    private static bool EndsWithDefault(StringBuilder p_sb, int length)
    {
        return (p_sb[length - 1] == ';'
            && p_sb[length - 2] == 't'
            && p_sb[length - 3] == 'l'
            && p_sb[length - 4] == 'u'
            && p_sb[length - 5] == 'a'
            && p_sb[length - 6] == 'f'
            && p_sb[length - 7] == 'e'
            && p_sb[length - 8] == 'd'
            && p_sb[length - 9] == '!');
    }

    private static IEnumerable<string> GetImportNames(string part)
    {
        char stringChar = ControlChars.NullChar;
        char c;
        StringBuilder sb = new();
        for (int i = 8; i < part.Length; i++)
        {
            c = part[i];
            if (c == stringChar)
            {
                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                }
                stringChar = ControlChars.NullChar;
            }
            else
            {
                switch (c)
                {
                    case '\'':
                    case '"':
                        stringChar = c;
                        sb.Length = 0;
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

        }
    }

    /// <summary>
    /// Save as one SCSS file.
    /// </summary>
    /// <param name="scssFile">Path to the file.</param>
    public void SaveTo(string scssFile)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(scssFile));
        File.WriteAllText(scssFile, ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Get the cleanned SCSS file as one file.
    /// </summary>
    /// <returns>The cleanned SCSS file as one file.</returns>
    public override string ToString()
    {
        StringBuilder sb = new(_size);
        AppendList(sb, _mixinParts);
        AppendConsolidatedVariables(sb, false);
        AppendList(sb, _classParts);
        return sb.ToString();
    }

    private static void AppendList(StringBuilder sb, List<string> parts)
    {
        foreach (string part in parts)
        {
#if DEBUG
            sb.AppendLine(part);
#else
            sb.Append(part);
#endif
        }
    }

    /// <summary>
    /// Kee the good variable declaration and reorder them by dependencies.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="final">Indicate if the "@if" must be interpretated and self-referenced variables must be interpreted.</param>
    /// <returns></returns>
    private void AppendConsolidatedVariables(StringBuilder sb, bool final)
    {
        /*
        
        Modes :
            * Package : création d'un fichier .scss intermédiaire des package Lagoon.UI.*
            * Final : Génération du fichier .scss final qui sera compilé en CSS
        
        CONSOLIDATION

        Blocs @if :
            * Package : Laisser en place et ne pas évaluer
            * Final : Evaluer avec la valeur en cours dans la variable (Une variable doit toujours être définie au niveau du scope avant d'être évaluée)
            ex: @if $rfs-base-font-size-unit=="px" {$rfs-base-font-size:divide($rfs-base-font-size,$rfs-base-font-size * 0 + 1)}
                @else if $rfs-base-font-size-unit=="rem" {$rfs-base-font-size:divide($rfs-base-font-size,divide($rfs-base-font-size * 0 + 1,$rfs-rem-value))}
        
        Variables :
            * L'attribut "!default" est ignoré -> Toute déclaration de variable vient remplacer la précédente définition
            * Package : Ne garder que la derniere valeur (sauf si elle s'appelle elle-même : Self-called variables)
            * Final : Ne garder que la derniere valeur
            ex: $gridview-rgaa-indicator-offset:-2px;
                $gridview-cell-maxwidth:100%;

        Self-called variables :
            * Package : Laisser en place et ne pas évaluer
            * Final : Evaluer Ne garder qu'une valeur en remplaçant dans la selfcalled variable la précédente valeur.
            ex: $spacers:();
                $spacers:map-merge( ( 0:0,1:($spacer * .25),2:($spacer * .5),3:$spacer,4:($spacer * 1.5),5:($spacer * 3) ),$spacers );

         */

        Dictionary<string, ScssVariable> values = new();
        List<ScssVariable> all = new();
        List<ScssConditionalVariable> conditions = new();
        ScssConditionalVariable lastIf = null;

        // CONSOLIDATION
        ScssVariable var;
        foreach (string part in _varParts)
        {
            var = ScssVariable.FromPart(part);
            if (var.IsSelfDepending)
            {
                if (final)
                {
                    if (values.TryGetValue(var.Key, out ScssVariable old))
                    {
                        old.ApplySelfCall(var);
                    }
                    else
                    {
                        throw new Exception($"The {var.Key} variable can't be evaluated (No original value).");
                    }
                }
                else
                {
                    all.Add(var);
                }
            }
            else if (var is ScssConditionalVariable condition)
            {
                if (final)
                {
                    if (condition.IsElse)
                    {
                        if (lastIf is null)
                        {
                            throw new Exception($"The @if is not found for {condition.Declaration.Substring(0, 100)}");
                        }
                        lastIf.AddElse(condition);
                    }
                    else
                    {
                        lastIf = condition;
                        conditions.Add(condition);
                    }
                }
                else
                {
                    all.Add(var);
                }
            }
            else // simple variable
            {
                if (values.TryGetValue(var.Key, out ScssVariable old))
                {
                    old.ReplaceBy(var);
                }
                else
                {
                    values.Add(var.Key, var);
                    if (!final)
                    {
                        all.Add(var);
                    }
                }
            }
        }
        // ORDER VARIABLES BY DEPENDENCIES
        if (final)
        {
            // Resolve dependencies
            foreach (ScssVariable v in values.Values)
            {
                v.LoadDependencies(values);
                all.Add(v);
            }
            foreach (ScssConditionalVariable c in conditions)
            {
                c.LoadDependencies(values);
                all.Add(c);
            }
            // write variables by dependencies
            foreach (ScssVariable v in TopogicalSequenceDFS(all, v => v.Dependencies))
            {
                v.WriteTo(sb);
            }
        }
        else
        {
            foreach (ScssVariable v in all)
            {
                v.WriteTo(sb);
            }
        }
    }

    //https://stackoverflow.com/a/51235189/3568845
    private static IEnumerable<T> TopogicalSequenceDFS<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> deps) where T : ScssVariable
    {
        HashSet<T> yielded = new();
        HashSet<T> visited = new();
        Stack<Tuple<T, IEnumerator<T>>> stack = new();

        foreach (T t in source)
        {
            stack.Clear();
            if (visited.Add(t))
                stack.Push(new Tuple<T, IEnumerator<T>>(t, deps(t).GetEnumerator()));

            while (stack.Count > 0)
            {
                var p = stack.Peek();
                bool depPushed = false;
                while (p.Item2.MoveNext())
                {
                    var curr = p.Item2.Current;
                    if (visited.Add(curr))
                    {
                        stack.Push(new Tuple<T, IEnumerator<T>>(curr, deps(curr).GetEnumerator()));
                        depPushed = true;
                        break;
                    }
                    else if (!yielded.Contains(curr))
                        throw new Exception("err: Cycle ! " + curr.Declaration);
                }

                if (!depPushed)
                {
                    p = stack.Pop();
                    if (!yielded.Add(p.Item1))
                        throw new Exception("bug");
                    yield return p.Item1;
                }
            }
        }
    }

    #endregion

    private class ScssVariable
    {
        protected static Regex VarExtractor = new("\\$[-a-z0-9]*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string _key;
        private List<ScssVariable> _dependencies;
        private bool _isSelfDepending;
        private string _part;

        public string Key => _key;
        public string Declaration => _part;
        public List<ScssVariable> Dependencies => _dependencies;

        public bool IsCondition => Key is null;

        public bool IsSelfDepending => _isSelfDepending;

        protected List<string> RawDependencies;

        public static ScssVariable FromPart(string part)
        {
            if (part[0] == '$')
            {
                //$var
                return new(part);
            }
            else
            {
                //@if or @else
                return new ScssConditionalVariable(part);
            }
        }

        protected ScssVariable(string part, List<string> rawDependencies)
        {
            _part = part;
            RawDependencies = rawDependencies;
        }

        private ScssVariable(string part)
        {
            _part = part;
            int pos = part.IndexOf(':');
            _key = part.Substring(0, pos);
            RawDependencies = RegexGetList(VarExtractor, part.Substring(pos + 1), 0);
            _isSelfDepending = RawDependencies.Contains(Key);
        }

        public virtual void LoadDependencies(Dictionary<string, ScssVariable> dico)
        {
            _dependencies = new();
            LoadDependencies(dico, RawDependencies, _dependencies);
        }

        protected static void LoadDependencies(Dictionary<string, ScssVariable> dico, List<string> source, List<ScssVariable> target)
        {
            foreach (string dep in source)
            {
                if (dico.TryGetValue(dep, out ScssVariable v))
                {
                    if (!target.Contains(v))
                    {
                        target.Add(v);
                    }
                }
                else
                {
                    throw new Exception($"The {dep} is not defined !");
                }
            }
        }


        public void ReplaceBy(ScssVariable var)
        {
            _key = var.Key;
            _part = var.Declaration;
            RawDependencies = var.RawDependencies;
            _dependencies = var._dependencies;
            _isSelfDepending = var.IsSelfDepending;
        }

        public void ApplySelfCall(ScssVariable var)
        {
            int start = _key.Length + 1;
            string oldValue = _part.Substring(start).TrimEnd(';');
            string newValue = var._part.Substring(start).Replace(Key, oldValue);
            _part = $"{Key}:{newValue}";
            RawDependencies = RegexGetList(VarExtractor, newValue, 0);
            _isSelfDepending = false;
        }

        internal virtual void WriteTo(StringBuilder sb)
        {
#if DEBUG
            sb.AppendLine(_part);
#else
            sb.Append(_part);
#endif

        }
    }

    private class ScssConditionalVariable : ScssVariable
    {

        protected static Regex DependerExtractor = new("(\\$[-a-z0-9]*):", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private List<ScssConditionalVariable> _elses;

        private List<string> _rawDependers = new();

        public ScssConditionalVariable(string part)
            : base(part, GetRawDependencies(part))
        {
            _rawDependers = RegexGetList(DependerExtractor, part.Substring(part.IndexOf('{')), 1);
        }

        public bool IsElse => Declaration.StartsWith("@else");

        private static List<string> GetRawDependencies(string part)
        {
            return RegexGetList(VarExtractor, part.Substring(0, part.IndexOf('{')), 0);
        }

        internal void AddElse(ScssConditionalVariable condition)
        {
            _elses ??= new();
            _elses.Add(condition);
        }

        public override void LoadDependencies(Dictionary<string, ScssVariable> dico)
        {
            // Add dependencies to evaluate the @if
            base.LoadDependencies(dico);
            // Indicate to initialized variables that they depends of the condition
            UpdateDependers(dico, _rawDependers);
            // Add the dependencied to evaluate the @else
            if (_elses is not null)
            {
                foreach (var e in _elses)
                {
                    LoadDependencies(dico, e.RawDependencies, Dependencies);
                    UpdateDependers(dico, e._rawDependers);
                }
            }
#if DEBUG //TOCLEAN
            Console.WriteLine($"{Dependencies.Count}");
#endif
        }

        private void UpdateDependers(Dictionary<string, ScssVariable> dico, List<string> rawDependers)
        {
            foreach (string dep in rawDependers)
            {
                if (dico.TryGetValue(dep, out ScssVariable v))
                {
                    if (!v.Dependencies.Contains(this))
                    {
#if DEBUG //TOCLEAN
                        Console.WriteLine($"{Declaration.Substring(Declaration.IndexOf('{'))} -> {v.Declaration}");
#endif
                        Dependencies.Add(v);
                        //if (v.Key != "$rfs-base-font-size" && v.Key !="")
                        //{
                        //    v.Dependencies.Add(this);
                        //}
                    }
                }
                else
                {
                    throw new Exception($"The ${dep} is not defined !");
                }
            }
        }

        internal override void WriteTo(StringBuilder sb)
        {
            base.WriteTo(sb);
            if (_elses is not null)
            {
                foreach (var e in _elses)
                {
                    e.WriteTo(sb);
                }
            }
        }

    }

    private static List<string> RegexGetList(Regex regex, string input, int index)
    {
        List<string> list = new();
        foreach (Match match in regex.Matches(input).Cast<Match>())
        {
            list.Add(match.Groups[index].Value);
        }
        return list;
    }

    #region static methods

    public static string CalculateFolderHash(string mainScssFile)
    {
        if (!File.Exists(mainScssFile))
        {
            return null;
        }
        MD5Builder md5 = new();
        md5.AddFolder(Path.GetDirectoryName(mainScssFile), "*.scss");
        return md5.GetHash();

    }

    #endregion

}
