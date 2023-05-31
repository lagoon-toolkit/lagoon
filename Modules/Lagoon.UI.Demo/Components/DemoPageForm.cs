using Lagoon.UI.Demo.ViewModel;
using Microsoft.AspNetCore.Components.Forms;

namespace Lagoon.UI.Demo.Pages.Form;

public partial class DemoPageForm : DemoPage
{
    protected FormData formData = new() { };
    protected LgEditForm formDemo;
    protected EditContext editContext;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        editContext = new EditContext(formData);
    }

    #region parameters
    protected string Label { get; set; } = "Label";
    protected bool IsDisable { get; set; } = false;
    protected bool IsReadOnly { get; set; } = false;

    protected void OnChangeLabel(string label)
    {
        Label = label;
    }
    protected void OnChangeDisable(bool disable)
    {
        IsDisable = disable;
    }
    protected void OnChangeReadOnly(bool readOnly)
    {
        IsReadOnly = readOnly;
    }
    #endregion
}
