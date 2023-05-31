namespace Lagoon.UI.Demo.Tests;

[Route(ROUTE)]
public partial class SimpleGridView : LgPage
{

    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "SimpleGridView";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "SimpleGridView", IconNames.All.Flower2);
    }

    #endregion

    public string FixedProperty { get; set; } = "FixedProperty";

    #region "private classes"

    public class Color
    {
        public string Name { get; set; }

        public Color()
        {
        }

        public Color(string name)
        {
            Name = name;
        }
    }

    public class Theme
    {
        public static Theme Model { get; } = new Theme();

        public string Name { get; set; }
        public Guid Guid { get; set; }
        public List<Color> Colors { get; set; } = new() { new Color("Red"), new Color("Gold"), new Color("Green") };
        public Color MainColor { get; set; } = new();
        public int ColorIndex { get; set; } = 0;

        public Dictionary<string, Color> Dico { get; set; } = new()
        {
            { "red", new Color() },
            { "gold", new Color() },
            { "green", new Color() },
        };

        public Theme()
        {
        }

        public Theme(string name)
        {
            Name = name;
            MainColor.Name = "Main Color";
        }

    }

    #endregion

    #region "properties"

    public List<Theme> Themes { get; set; } = new() { new Theme("Custom") };

    public List<Color> Colors { get; set; } = new() { new Color("Red"), new Color("Gold"), new Color("Green") };

    #endregion

}
