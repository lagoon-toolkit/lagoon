namespace Lagoon.Generators;

internal class IconProvider
{

    public SortedDictionary<string, IconAlias> Aliases { get; set; }

    public SortedDictionary<string, IconSymbol> Symbols { get; set; }


    public void SaveToSvgFile(string filePath, bool includeALias)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        using StreamWriter svg = new(filePath);
        svg.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        svg.Write("<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
        // Images aliases
        if (includeALias)
        {
            foreach (KeyValuePair<string, IconAlias> entry in Aliases)
            {
                svg.Write("<symbol id=\"");
                svg.Write(IconSymbol.ID_ALIAS_PREFIX + entry.Key);
                svg.Write("\"><use xlink:href=\"#");
                svg.Write(Symbols[entry.Value.Value].Id);
                svg.WriteLine("\"/></symbol>");
            }
        }
        // Image symbols
        foreach (KeyValuePair<string, IconSymbol> entry in Symbols)
        {
            svg.Write(entry.Value.Symbol);
        }
        svg.Write("</svg>");
    }

    public void SaveToXmlFile(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        using StreamWriter xml = new(filePath);
        xml.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xml.WriteLine("<lagoonResources>");
        if (Aliases is not null)
        {
            xml.WriteLine("	<iconAliases>");
            foreach (KeyValuePair<string, IconAlias> entry in Aliases)
            {
                xml.Write("		<iconAlias id=\"");
                xml.Write(entry.Key);
                xml.Write("\" value=\"");
                xml.Write(entry.Value.Value);
                xml.WriteLine("\" />");
            }
            xml.WriteLine("	</iconAliases>");
        }
        xml.WriteLine("</lagoonResources>");
    }

}
