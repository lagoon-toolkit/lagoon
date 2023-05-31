namespace Lagoon.UI.Demo.Pages.Parameters;

public partial class IconParameter : LgComponentBase
{

    private static List<Icon> _icons = GetReducedIconList();

    [Parameter]
    public string Icon { get; set; }

    [Parameter]
    public EventCallback<string> IconChanged { get; set; }

    [Parameter]
    public string CssClass { get; set; }

    [Parameter]
    public EventCallback<ChangeEventArgs> OnChange { get; set; }

    private static List<Icon> GetReducedIconList()
    {
        return GetIcons(typeof(IconNames)).Concat(GetAdditionalIcons()).OrderBy(i => i.Label).ToList();
    }

    private static IEnumerable<Icon> GetIcons(Type type)
    {
        return type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
        .Select(x => new Icon(x.GetValue(null).ToString(), x.Name));
    }

    private static IEnumerable<Icon> GetAdditionalIcons()
    {
        yield return new Icon(IconNames.All.Box, nameof(IconNames.All.Box));
        yield return new Icon(IconNames.All.Search, nameof(IconNames.All.Search));
        yield return new Icon(IconNames.All.Palette, nameof(IconNames.All.Palette));
    }

    public async Task OnChangeIconAsync(ChangeEventArgs e)
    {
        Icon = (string)e.Value;
        if (IconChanged.HasDelegate)
        {
            await IconChanged.TryInvokeAsync(App, Icon);
        }
        if (OnChange.HasDelegate)
        {
            await OnChange.TryInvokeAsync(App, e);
        }
    }

}
