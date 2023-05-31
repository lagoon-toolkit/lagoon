namespace Lagoon.UI.Demo.Pages.Parameters;

public partial class PrefixSuffixParameters : LgComponentBase
{

    [Parameter]
    public PrefixSuffix PrefixSuffix { get; set; }

    [Parameter]
    public EventCallback<PrefixSuffix> PrefixSuffixChanged { get; set; }


    private async Task OnChangeAsync()
    {
        if (PrefixSuffixChanged.HasDelegate)
        {
            await PrefixSuffixChanged.TryInvokeAsync(App, PrefixSuffix);
        }
    }

}
