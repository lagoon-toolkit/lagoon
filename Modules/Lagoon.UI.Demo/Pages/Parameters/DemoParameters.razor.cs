using Lagoon.UI.Demo.ViewModel;
using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Demo.Pages.Parameters;

public partial class DemoParameters : LgComponentBase
{
    protected FormData formData = new();
    protected EditContext editContext;

    #region Common Parameters
    [Parameter]
    public bool HasText { get; set; } = false;

    [Parameter]
    public bool HasIcons { get; set; } = false;

    [Parameter]
    public bool HasDisable { get; set; } = false;

    [Parameter]
    public bool HasClosable { get; set; } = false;

    [Parameter]
    public bool HasShowable { get; set; } = false;

    [Parameter]
    public bool HasReadonly { get; set; } = false;

    [Parameter]
    public bool HasDescription { get; set; } = false;
    #endregion

    #region other parameters
    [Parameter]
    public bool IsButtonType { get; set; } = false;

    [Parameter]
    public bool IsPagerType { get; set; } = false;

    [Parameter]
    public string Icon { get; set; }

    [Parameter]
    public EventCallback<string> IconChanged { get; set; }

    #endregion

    #region common values
    public string Text { get; set; } = "label";
    public bool Disable { get; set; } = false;
    public bool Closable { get; set; } = false;
    public bool Show { get; set; } = true;
    public bool Readonly { get; set; } = false;
    public string Description { get; set; } = "Description";
    #endregion

    #region others Type
    public ButtonSize ButtonSize { get; set; } = ButtonSize.Medium;
    public int Total { get; set; } = 10;
    public int Current { get; set; } = 5;
    public int MaxPagesToDisplay { get; set; } = 5;
    #endregion


    #region common OutputEvent
    [Parameter]
    public EventCallback<string> OnTextEdition { get; set; }

    [Parameter]
    public EventCallback<bool> OnDisableEdition { get; set; }

    [Parameter]
    public EventCallback<bool> OnClosableEdition { get; set; }

    [Parameter]
    public EventCallback<bool> OnShowEdition { get; set; }

    [Parameter]
    public EventCallback<bool> OnReadonlyEdition { get; set; }

    [Parameter]
    public EventCallback<string> OnDescriptionEdition { get; set; }
    #endregion

    #region OutputEvent Button
    [Parameter]
    public EventCallback<ButtonSize> OnButtonSizeEdition { get; set; }
    [Parameter]
    public EventCallback<int> OnTotalEdition { get; set; }
    [Parameter]
    public EventCallback<int> OnCurrentEdition { get; set; }
    [Parameter]
    public EventCallback<int> OnMaxPageToDisplayEdition { get; set; }
    #endregion

    #region init
    protected override void OnInitialized()
    {
        base.OnInitialized();
        editContext = new EditContext(formData);
    }
    #endregion

    #region common events        
    public Task OnChangeTextAsync()
    {
        return OnTextEdition.TryInvokeAsync(App, Text);
    }

    public async Task OnChangeIconAsync(ChangeEventArgs e)
    {
        Icon = (string)e.Value;
        if (IconChanged.HasDelegate)
        {
            await IconChanged.TryInvokeAsync(App, Icon);
        }
    }

    public Task OnChangeDisableAsync()
    {
        return OnDisableEdition.TryInvokeAsync(App, Disable);
    }

    public Task OnChangeClosableAsync()
    {
        return OnClosableEdition.TryInvokeAsync(App, Closable);
    }

    public Task OnChangeShowAsync()
    {
        return OnShowEdition.TryInvokeAsync(App, Show);
    }

    public Task OnChangeReadonlyAsync()
    {
        return OnReadonlyEdition.TryInvokeAsync(App, Readonly);
    }

    public Task OnChangeDescriptionAsync()
    {
        return OnDescriptionEdition.TryInvokeAsync(App, Description);
    }

    #endregion

    #region others events        
    public Task OnChangeButtonSizeAsync()
    {
        return OnButtonSizeEdition.TryInvokeAsync(App, ButtonSize);
    }

    public Task OnChangeTotalAsync()
    {
        return OnTotalEdition.TryInvokeAsync(App, Total);
    }

    public Task OnChangeCurrentAsync()
    {
        return OnCurrentEdition.TryInvokeAsync(App, Current);
    }

    public Task OnChangeMaxPageToDisplayAsync()
    {
        return OnMaxPageToDisplayEdition.TryInvokeAsync(App, MaxPagesToDisplay);
    }

    #endregion
}
