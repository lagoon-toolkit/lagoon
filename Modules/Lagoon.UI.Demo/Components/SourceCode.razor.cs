namespace Lagoon.UI.Demo.Components;

public partial class SourceCode : LgComponentBase
{

    private bool _isLoading = true;
    private bool _collapsed = true;
    private bool _showCSharp;
    private ElementReference _divCode;

    [Parameter]
    public ISourceCodeProvider SourceCodeProvider { get; set; }

    public bool ShowCSharp { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            await SourceCodeProvider.DownloadSourcesAsync();
        }
        finally
        {
            _isLoading = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await JS.ScriptIncludeAsync("_content/Lagoon.UI.Demo/js/main.min.js");
        await JS.InvokeAsync<string>("window.Lagoon.hightlight", _divCode);
    }

    private void ShowCode(bool showCSharp)
    {
        _showCSharp = showCSharp;
        _collapsed = false;
    }
}
