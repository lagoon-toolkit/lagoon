using Microsoft.JSInterop.WebAssembly;

namespace Lagoon.UI.Components;

/// <summary>
/// Dropdown
/// </summary>
public partial class LgDropDown : LgAriaComponentBase
{
    #region fields

    /// <summary>
    /// Display or not the items list
    /// </summary>
    private bool _isOpen;

    /// <summary>
    /// Blazor component reference
    /// </summary>
    private IDisposable _dotNetObjectReference;

    /// <summary>
    /// Indicate show state has changed
    /// </summary>
    private bool _showChanged;

    /// <summary>
    /// Read only indicator
    /// </summary>
    private bool _previousReadOnly;

    /// <summary>
    /// Indicate parameter change
    /// </summary>
    private bool _hasChanged;

    #endregion

    #region render fragments

    /// <summary>
    /// Gets or sets dropdown linked control render
    /// </summary>
    [Parameter]
    public RenderFragment ControlContent { get; set; }

    /// <summary>
    /// Gets or sets dropdown render content
    /// </summary>
    [Parameter]
    public RenderFragment<bool> DropDownContent { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets dropdown auto open flag
    /// </summary>
    [Parameter]
    public bool AutoOpen { get; set; } = true;

    /// <summary>
    /// Gets or sets if the content can select multiple item
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; }

    /// <summary>
    /// Gets or sets aria role
    /// </summary>
    [Parameter]
    public string Role { get; set; } = "combobox";

    /// <summary>
    /// Gets or sets css class
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or set disable indicator
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or set read only indicator
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets visibility state
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets visibility state change event
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Gets or sets event called when dropdown is opened
    /// </summary>
    [Parameter]
    public EventCallback OnOpen { get; set; }

    /// <summary>
    /// Gets or sets event called when dropdown is closed
    /// </summary>
    [Parameter]
    public EventCallback OnClose { get; set; }

    /// <summary>
    /// Gets or sets keydown event in dropdown
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    /// <summary>
    /// Gets or sets if dropdown is open on focus
    /// </summary>
    [Parameter]
    public bool OpenOnFocus { get; set; }

    /// <summary>
    /// Gets or sets if dropdown is positionned over control content
    /// </summary>
    [Parameter]
    public bool OverControl { get; set; }

    /// <summary>
    /// Gets or sets dropdown minimal width in pixel
    ///     -1 = width calculated following width control        
    /// </summary>
    [Parameter]
    public int MinWidth { get; set; } = -1;

    /// <summary>
    /// Gets or sets dropdown minimal height in pixel
    ///     -1 = width calculated following width control        
    /// </summary>
    [Parameter]
    public int MinHeight { get; set; } = -1;

    #endregion

    #region properties

    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    protected string ElementId { get; } = GetNewElementId();

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    public ElementReference ElementRef { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        // Detect change for JS
        if (ReadOnly != _previousReadOnly)
        {
            _hasChanged = true;
            _previousReadOnly = ReadOnly;
        }
        base.OnParametersSet();
    }

    ///<inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await ToggleListAsync(IsOpen, false);
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            // Initialize dropdown JS part
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("Lagoon.LgDropDown", ElementRef, _dotNetObjectReference, new { 
                OpenOnFocus = OpenOnFocus,
                OverControl = OverControl,
                MinWidth = MinWidth,
                MinHeight = MinHeight
            });
        }
        else if (_hasChanged)
        {
            // Update JS after dropdown build change
            _hasChanged = false;
            await JS.InvokeVoidAsync("Lagoon.LgDropDownInit", ElementRef);
        }
        // Open list
        if (_showChanged)
        {
            _showChanged = false;
            await JS.InvokeVoidAsync("Lagoon.LgDropDownShowChanged", ElementRef, _isOpen);
        }
    }

    /// <summary>
    /// Change dropdown list visibility
    /// </summary>
    /// <param name="isOpen"></param>
    /// <param name="propagateChanged"></param>      
    /// <param name="activeChange"></param>
    /// <returns></returns>        
    private async Task ToggleListAsync(bool isOpen, bool propagateChanged, bool activeChange = true)
    {
        if (!Disabled && !ReadOnly && _isOpen != isOpen)
        {
            // Toggle the dropdown visibility
            _isOpen = isOpen;
            _showChanged = activeChange;
            bool stateHasChanged = true;
            if (propagateChanged)
            {
                if (IsOpenChanged.HasDelegate)
                {
                    stateHasChanged = false;
                    await IsOpenChanged.TryInvokeAsync(App, _isOpen);
                }
            }
            else
            {
                stateHasChanged = false;
            }
            if (isOpen && OnOpen.HasDelegate)
            {
                stateHasChanged = false;
                await OnOpen.TryInvokeAsync(App);
            }
            if (!isOpen && OnClose.HasDelegate)
            {
                stateHasChanged = false;
                await OnClose.TryInvokeAsync(App);
            }
            if (stateHasChanged)
            {
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Toggle items list
    /// </summary> 
    /// <param name="notTriggerChange"></param>
    [JSInvokable]
    public Task CloseDropdownAsync(bool notTriggerChange)
    {
        return ToggleListAsync(false, true, !notTriggerChange);
    }

    /// <summary>
    /// Toggle item list
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public Task ToggleListAsync()
    {
        return ToggleListAsync(!_isOpen, true);
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        _dotNetObjectReference.Dispose();
        // Dispose JS
        if (JS is WebAssemblyJSRuntime webAssemblyJSRuntime)
        {
            webAssemblyJSRuntime.InvokeVoid("Lagoon.LgDropDownDispose", ElementRef);
        }

        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.Add("form-input", "lgDropdown", CssClass);
    }

    /// <summary>
    /// Key up event propagation
    /// </summary>
    /// <param name="args"></param>        
    [JSInvokable]
    public async Task KeyUpAsync(KeyboardEventArgs args)
    {
        if (OnKeyUp.HasDelegate)
        {

            await OnKeyUp.TryInvokeAsync(App, args);
        }
    }

    #endregion
}
