using System.Text.RegularExpressions;
using System.Xml;

namespace Lagoon.Generators;

internal class IconList : IconProvider
{

    #region constants

    public const string FOLDER_NAME = "icons";
    public const string SOURCE_FILE = "IconNames.cs";
    private const string SVG_FILTER = "*.svg";
    private const string SVG_FOLDER = "svg";
    private const string SPRITES_FOLDER = "sprites";
    public const string ID_PREFIX = "i-";
    public const string ID_ALIAS_PREFIX = ID_PREFIX + "a-";

    #endregion

    #region variables

    private readonly object _loadLock = new();
    private readonly string _iconFolderPath;
    private string _oldFolderHash;
    private string _aliasHash;
    #endregion

    #region properties

    public string Hash => _oldFolderHash + _aliasHash;

    #endregion

    #region constructors

    public IconList(string folderPath)
    {
        _iconFolderPath = folderPath;
    }

    #endregion

    #region methods

    public void Load(System.Threading.CancellationToken cancellationToken)
    {
        lock (_loadLock)
        {
            // We abort if cancellation asked
            if (!cancellationToken.IsCancellationRequested)
            {
                // Detect if the content changed
                MD5Builder md5 = new();
                md5.AddSubFolder(_iconFolderPath, SPRITES_FOLDER, SVG_FILTER);
                md5.AddSubFolder(_iconFolderPath, SVG_FOLDER, SVG_FILTER);
                string newHash = md5.GetHash();
                if (newHash == _oldFolderHash)
                {
                    // Quit if the folder content don't change
#if DEBUG
                    Log.ToFile($"Skip icon loading #{newHash}# for {_iconFolderPath}");
#endif
                    return;
                }
                // Load the symbol list
                SortedDictionary<string, IconSymbol> list = new();
                LoadFromSubPath(list, _iconFolderPath, SPRITES_FOLDER, LoadSpritesSymbols);
                LoadFromSubPath(list, _iconFolderPath, SVG_FOLDER, LoadSvgSymbols);
                Symbols = list;
                // Load the alias list
                SortedDictionary<string, IconAlias> aliases = new();
                // Loading the list of icon aliases from the source file.
                md5 = new();
                IconAlias iconAlias;
                foreach (Match match in GetRegExMatches(Path.Combine(_iconFolderPath,SOURCE_FILE), @"([^ <{]+) = All\.([^;<{]+)"))
                {
                    iconAlias = new(match.Groups[1].Value, match.Groups[2].Value);
                    aliases.Add(match.Groups[1].Value, iconAlias);
                    md5.Add(iconAlias.Id);
                    md5.Add(iconAlias.Value);
                }
                _aliasHash = md5.GetHash();
                // Remove "All" if exists to not brake "All" Property
                aliases.Remove("All");
                // Update the property
                Aliases = aliases;
                // Save the new loaded hash
                _oldFolderHash = newHash;
            }
        }
    }

    private static void LoadFromSubPath(SortedDictionary<string, IconSymbol> list, string path, string subPathName,
        Action<string, SortedDictionary<string, IconSymbol>> loadSymbols)
    {
        LoadFromPath(list, Path.Combine(path, subPathName), loadSymbols);
    }

    private static void LoadFromPath(SortedDictionary<string, IconSymbol> list, string path,
        Action<string, SortedDictionary<string, IconSymbol>> loadSymbols)
    {
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.EnumerateFiles(path, SVG_FILTER, SearchOption.AllDirectories))
            {
                loadSymbols(file, list);
            }
        }
    }

    /// <summary>
    /// Load icon list from SVG files in resource path.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="all">ImageName/Id.</param>
    /// <param name="aliases">AliasName/Id.</param>
    /// <param name="symbols">Synmbol XML code.</param>
    private static void LoadSpritesSymbols(string file, SortedDictionary<string, IconSymbol> all)
    {
        XmlDocument doc = new();
        string id, name;
        doc.Load(file);
        // Load symbol library
        XmlNodeList l_o_nodes = XmlHelper.SelectNodes(doc, "/svg/*");
        foreach (XmlNode node in l_o_nodes)
        {
            id = node.Attributes["id"]?.Value;
            if (string.IsNullOrEmpty(id))
            {
                continue;
            }
            name = BuildName(id);
            // Icons already added are ignored
            if (!all.ContainsKey(name))
            {
                AddSymbol(all, name, id, node);
            }
        }
    }

    /// <summary>
    /// Load icon list from SVG files in resource path.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="all">ImageName/Id.</param>
    /// <param name="aliases">AliasName/Id.</param>
    /// <param name="symbols">Synmbol XML code.</param>
    private static void LoadSvgSymbols(string file, SortedDictionary<string, IconSymbol> all)
    {
        XmlDocument doc = new();
        string id, name;

        doc.Load(file);
        // Find icons id in file
        XmlNode l_o_node = XmlHelper.SelectSingleNode(doc, "/svg");
        if (!l_o_node.HasChildNodes)
        {
            return;
        }
        if (l_o_node is null)
        {
            throw new Exception("No svg found in file : " + file);
        }
        string viewbox = XmlHelper.GetAttributeValue(l_o_node, "viewBox");
        if (string.IsNullOrEmpty(viewbox))
        {
            throw new Exception("No viewbox attribute found in file : " + file);
        }
        // Cleanup id
        id = Path.GetFileNameWithoutExtension(file);
        id = Regex.Replace(id, "([^-A-Za-z0-9 _])", "");
        id = Regex.Replace(id, "([A-Z])", " $1").Trim();
        id = Regex.Replace(id, "( |_)+", "-").ToLowerInvariant();
        name = BuildName(id);
        if (all.ContainsKey(name))
        {
            return;
        }
        // Cleanup node // xlmns ???
        XmlElement symbol = XmlHelper.AddNode(l_o_node, "symbol", "class", "bi bi-" + id, "id", id,
        "viewBox", viewbox, "fill", "currentColor");
        for (int i = l_o_node.ChildNodes.Count - 1; i >= 0; i--)
        {
            XmlNode c = l_o_node.ChildNodes[i];
            if (c.Name.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (c != symbol)
            {
                symbol.InsertBefore(c, symbol.FirstChild);
            }
        }
        // recherche des url
        string text = symbol.InnerXml;
        MatchCollection urls = Regex.Matches(text, @"url\(#([^)]*)\)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        List<XmlNode> keep = new();
        if (urls.Count > 0)
        {
            int cnt = 0;
            foreach (Match match in urls)
            {
                string oldUid = match.Groups[1].Value;
                XmlNode o = XmlHelper.SelectSingleNode(symbol, "//*[@id='" + oldUid + "']");
                if (o is null)
                {
                    continue;
                }
                //// Extract object from symbol, else URL will not work
                keep.Add(o);
                // Update id
                string uid = ID_PREFIX + id + "_ref" + cnt.ToString();
                // Update URL calls
                ReplaceUrlValue(symbol, oldUid, uid);
                XmlHelper.SetAttributeValue(o, "id", uid);
                // increment id
                cnt++;
            }
        }
        // Remove unused Id
        foreach (XmlNode n in XmlHelper.SelectNodes(symbol, "*[@id]"))
        {
            if (!keep.Contains(n))
            {
                XmlHelper.RemoveAttribute(n, "id");
            }
        }
        // Add icon to collection
        AddSymbol(all, name, id, symbol);
    }


    private static void AddSymbol(SortedDictionary<string, IconSymbol> all, string name, string id, XmlNode symbol)
    {
        // Add the new icon
        if (!id.StartsWith(ID_PREFIX)) { id = ID_PREFIX + id; }
        symbol.Attributes["id"].Value = id;
        symbol.Attributes.RemoveNamedItem("class");
        all.Add(name, new IconSymbol() { Id = id, Symbol = symbol.OuterXml });
    }

    /// <summary>
    /// Build constant name from svg id.
    /// </summary>
    /// <param name="img">SVG id.</param>
    /// <returns>Constant name.</returns>
    private static string BuildName(string img)
    {
        bool ucase = true;
        if (img.StartsWith(ID_PREFIX))
        {
            img = img.Substring(2);
        }
        StringBuilder sb = new(img.Length);
        foreach (char c in img)
        {
            switch (c)
            {
                case '-':
                    ucase = true;
                    break;
                default:
                    sb.Append(ucase ? char.ToUpper(c) : c);
                    ucase = false;
                    break;
            }
        }
        return sb.ToString();
    }

    private static void ReplaceUrlValue(XmlNode node, string oldUid, string newUid)
    {
        foreach (XmlAttribute att in node.Attributes)
        {
            if (att.Value.Equals("url(#" + oldUid + ")"))
            {
                att.Value = @"url(#" + newUid + ")";

            }
        }
        foreach (XmlNode child in node.ChildNodes)
        {
            ReplaceUrlValue(child, oldUid, newUid);
        }
    }

    private static IEnumerable<Match> GetRegExMatches(string file, string pattern)
    {
        if (File.Exists(file))
        {
            string source = File.ReadAllText(file);

            foreach (Match match in Regex.Matches(source, pattern, RegexOptions.Multiline))
            {
                yield return match;
            }
        }
    }

}

#endregion
