namespace Lagoon.Generators;

internal class IconsMerger : IconProvider
{

    private StringBuilder _hashes;

    public IconsMerger(IconList iconList)
    {
        Aliases = new(iconList.Aliases);
        Symbols = new(iconList.Symbols);
        _hashes = new StringBuilder(iconList.Hash);
    }

    public void Add(IconList iconList)
    {
        foreach (KeyValuePair<string, IconAlias> a in iconList.Aliases)
        {
            if (!Aliases.ContainsKey(a.Key))
            {
                Aliases.Add(a.Key, a.Value);
            }
        }
        foreach (KeyValuePair<string, IconSymbol> s in iconList.Symbols)
        {
            if (!Symbols.ContainsKey(s.Key))
            {
                Symbols.Add(s.Key, s.Value);
            }
        }
        _hashes.Append(iconList.Hash);
    }

}
