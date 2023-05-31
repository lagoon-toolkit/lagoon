using Lagoon.UI.Components.Input.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;

/// <summary>
/// Multiline text box
/// </summary>
public partial class LgSummaryBox : LgInputRenderBase<string>
{

    #region fields

    /// <summary>
    /// Gets or sets the display value DOM element reference.
    /// </summary>
    protected ElementReference ElementRef;

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    /// <summary>
    /// Dropdown reference
    /// </summary>
    private LgDropDown _dropDown;

    /// <summary>
    /// Modal visibility state
    /// </summary>
    private bool _modalVisible;

    /// <summary>
    /// Reference to modal
    /// </summary>
    private LgModal _modalReference;

    #endregion

    #region parameters

    /// <summary>
    /// Gets or sets a callback called on the onfocus event.
    /// </summary>
    [Parameter]
    public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>
    /// Gets or sets a callback called on the onkey event.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    /// <summary>
    /// Gets or sets a callback called dropdown or modal is closed.
    /// </summary>
    [Parameter]
    public EventCallback OnClose { get; set; }

    /// <summary>
    /// Gets or sets a callback called dropdown or modal is opened.
    /// </summary>
    [Parameter]
    public EventCallback OnOpen { get; set; }

    /// <summary>
    /// Gets or sets the input placeholder
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the input prefix.
    /// </summary>
    [Parameter]
    public string Prefix { get; set; }

    /// <summary>
    /// Indicate if prefix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType PrefixType { get; set; }

    /// <summary>
    /// Gets or sets the input suffix.
    /// </summary>
    [Parameter]
    public string Suffix { get; set; }

    /// <summary>
    /// Indicate if suffix is simple text or icon from library.
    /// </summary>
    [Parameter]
    public InputLabelType SuffixType { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters entered
    /// </summary>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets if update use validation button
    /// </summary>        
    protected bool ButtonValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets textarea height in pixel              
    ///     Parameter is not working in inline mode     
    /// </summary>
    [Parameter]
    public int TextareaHeight { get; set; }

    /// <summary>
    /// Gets or sets if the entering input is in modal or inline
    /// </summary>        
    protected bool Modal { get; set; } = true;

    /// <summary>
    /// Gets or sets the modal title
    /// </summary>
    [Parameter]
    public string ModalTitle { get; set; }

    /// <summary>
    /// Gets or sets if the modal or dropdown open on focus
    /// </summary>
    [Parameter]
    public bool OpenOnFocus { get; set; }

    /// <summary>
    /// Gets or sets if displayed value is wrapped
    /// </summary>
    [Parameter]
    public bool WrapContent { get; set; }

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets current value in buttons mode management
    /// </summary>
    internal string SummaryValue { get; set; }

    #endregion

    #region methods

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        if (!Modal)
        {
            builder.OpenComponent<LgDropDown>(1);
            builder.AddAttribute(2, nameof(LgDropDown.OverControl), true);
            builder.AddAttribute(3, nameof(LgDropDown.CssClass), SummaryCssClass());
            builder.AddAttribute(4, nameof(LgDropDown.ControlContent), (RenderFragment)((subBuilder) =>
            {
                RenderControl(subBuilder, ReadOnly, false);

            }));
            builder.AddAttribute(5, nameof(LgDropDown.DropDownContent), (RenderFragment<Boolean>)((isOpen) =>
                (subBuilder) =>
                {
                    if (isOpen)
                    {
                        TextAreaRender(subBuilder);
                        ButtonsRender(subBuilder);
                    }
                }
            ));
            builder.AddAttribute(6, nameof(LgDropDown.OnKeyUp), OnKeyUp);
            builder.AddAttribute(7, nameof(LgDropDown.OnOpen), EventCallback.Factory.Create(this, () =>
            {
                SummaryValue = Value;
            }));
            builder.AddAttribute(8, nameof(LgDropDown.OpenOnFocus), OpenOnFocus);
            builder.AddAttribute(9, nameof(LgDropDown.Disabled), Disabled);
            builder.AddAttribute(10, nameof(LgDropDown.ReadOnly), ReadOnly);
            builder.AddComponentReferenceCapture(11, inst =>
            {
                _dropDown = (LgDropDown)inst;
            });
            builder.CloseComponent();
        }
        else
        {
            builder.OpenElement(1, "div");
            builder.AddAttribute(2, "class", SummaryCssClass());
            builder.AddAttribute(3, "tabindex", "0");
            if (OpenOnFocus)
            {
                builder.AddAttribute(4, "onfocusin", EventCallback.Factory.Create(this, OpenModalAsync));
            }
            if (Modal)
            {
                builder.AddAttribute(5, "onkeyup", EventCallback.Factory.Create<KeyboardEventArgs>(this, KeyUpAsync));
            }
            builder.AddAttribute(6, "id", ElementId);
            builder.OpenRegion(7);
            RenderControl(builder, ReadOnly, !Disabled);
            if (_modalVisible)
            {
                builder.OpenComponent<LgModal>(100);
                builder.AddAttribute(101, nameof(LgModal.Visible), true);
                builder.AddAttribute(102, nameof(LgModal.Title), ModalTitle ?? Label);
                builder.AddAttribute(103, nameof(LgModal.ChildContent), (RenderFragment)((subBuilder) =>
                {
                    TextAreaRender(subBuilder);
                    ButtonsRender(subBuilder);
                }));
                builder.AddAttribute(104, nameof(LgModal.OnClose), EventCallback.Factory.Create(this, async (CloseModalEventArgs args) =>
                {
                    if (SummaryValue != Value)
                    {
                        args.Cancel = true;
                        ShowConfirm("lgSummaryBoxConfirmSave".Translate(), async () =>
                        {
                            _modalVisible = false;
                            if (OnClose.HasDelegate)
                            {
                                await OnClose.TryInvokeAsync(App);
                            }
                            else
                            {
                                StateHasChanged();
                            }
                        });
                    }
                    else
                    {
                        await JS.FocusAsync($"#{ElementId}");
                        _modalVisible = false;
                        if (OnClose.HasDelegate)
                        {
                            await OnClose.TryInvokeAsync(App);
                        }
                        else
                        {
                            StateHasChanged();
                        }
                    }
                }));
                builder.AddEventStopPropagationAttribute(105, "onclick", true);
                builder.AddComponentReferenceCapture(106, r => _modalReference = (LgModal)r);
                builder.CloseComponent();
            }
            builder.CloseRegion();
            builder.CloseElement();
        }

    }

    /// <summary>
    /// Return dropdown control class
    /// </summary>
    /// <returns></returns>
    private string SummaryCssClass()
    {
        LgCssClassBuilder cb = new();
        cb.Add("summarybox");
        cb.AddIf(Modal && !WrapContent, "summarybox-modal");
        cb.AddIf(Modal && WrapContent, "summarybox-modal-wrap");
        cb.AddIf(_modalVisible, "summarybox-open");
        return cb.ToString();
    }

    /// <summary>
    /// Rendering of the input field
    /// </summary>
    /// <param name="builder"></param>
    private void TextAreaRender(RenderTreeBuilder builder)
    {
        builder.OpenComponent<LgTextBox>(1);
        builder.AddAttribute(2, nameof(LgTextBox.CssClass), "summarybox-text");
        builder.AddAttribute(3, nameof(LgTextBox.TextMode), TextBoxMode.Multiline);
        builder.AddAttribute(4, nameof(LgTextBox.ReadOnly), ReadOnly);
        if (!ButtonValidation)
        {
            builder.AddAttribute(5, nameof(LgTextBox.Value), Value);
            builder.AddAttribute(6, nameof(LgTextBox.ValueChanged), ValueChanged);
            builder.AddAttribute(7, nameof(LgTextBox.ValueExpression), ValueExpression);
        }
        else
        {
            builder.AddAttribute(5, nameof(LgTextBox.Value), SummaryValue);
            builder.AddAttribute(6, nameof(LgTextBox.OnInput), EventCallback.Factory.Create<ChangeEventArgs>(this, SummaryValueChanged));
        }
        builder.AddAttribute(8, nameof(LgTextBox.MaxLength), MaxLength);
        builder.AddAttribute(9, nameof(LgTextBox.AutoFocus), true);
        builder.AddAttribute(9, nameof(LgTextBox.Placeholder), Placeholder);
        if (Modal)
        {
            builder.AddAttribute(10, "style", TextareaHeight > 0 ? $"height:{TextareaHeight}px;" : "");
        }
        builder.CloseComponent();
    }

    /// <summary>
    /// Rendering of the validation buttons
    /// </summary>
    /// <param name="builder"></param>
    private void ButtonsRender(RenderTreeBuilder builder)
    {
        if (ButtonValidation && !ReadOnly)
        {
            string cancelLabel = "lgSummaryBoxCancel".Translate();
            string saveLabel = "lgSummaryBoxSave".Translate();
            builder.OpenElement(20, "div");
            builder.AddAttribute(21, "class", "summarybox-buttons");
            builder.OpenComponent<LgButton>(30);
            builder.AddAttribute(31, nameof(LgButton.Tooltip), saveLabel);
            builder.AddAttribute(32, nameof(LgButton.AriaLabel), saveLabel);
            builder.AddAttribute(33, nameof(LgButton.IconName), IconNames.Save);
            builder.AddAttribute(34, nameof(LgButton.OnClick), EventCallback.Factory.Create<ActionEventArgs>(this, ValidateChangeAsync));
            builder.AddAttribute(35, nameof(LgButton.Text), saveLabel);
            builder.AddEventStopPropagationAttribute(36, "onclick", true);
            builder.CloseComponent();
            builder.CloseElement();
        }
    }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        // TODO: check if textarea size grow following content is needed
        //if (firstRender || _dropDown.IsOpen)
        //{
        //    // Initialize dropdown JS part                
        //    await JS.InvokeVoidAsync("Lagoon.LgSummaryBox", _dropDown.ElementRef);
        //}
        // Prefix and suffix positionment
        if (firstRender && ((!string.IsNullOrEmpty(Prefix) && PrefixType == InputLabelType.Text)
            || (!string.IsNullOrEmpty(Suffix) && SuffixType == InputLabelType.Text)))
        {
            await JS.InvokeVoidAsync("Lagoon.LgTextBox.InitPrefixSuffixPadding", ElementRef);
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        base.OnBuildClassAttribute(builder);
        builder.AddIf(!HasLabel(), "input-without-lbl");
    }

    /// <summary>
    /// Render display value
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="readOnly"></param>
    /// <param name="clickModal"></param>
    private void RenderControl(RenderTreeBuilder builder, bool readOnly, bool clickModal)
    {
        bool hasSuffix = !string.IsNullOrEmpty(Suffix);
        bool hasPrefix = !string.IsNullOrEmpty(Prefix);
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", $"form-input {InputCssClass}");
        if (clickModal)
        {
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create<ChangeEventArgs>(this, OpenModalAsync));
            builder.AddEventStopPropagationAttribute(3, "onclick", true);
        }
        // Prefix
        if (!string.IsNullOrEmpty(Prefix))
        {
            builder.OpenElement(10, "span");
            builder.AddAttribute(11, "class", "form-input-prefix");

            if (PrefixType == InputLabelType.IconName)
            {
                builder.OpenComponent<LgIcon>(12);
                builder.AddAttribute(13, nameof(LgIcon.IconName), Prefix);
                builder.CloseComponent();
            }
            else
            {
                builder.AddContent(12, Prefix);
            }
            builder.CloseElement();
        }
        //Suffix
        if (hasSuffix)
        {
            builder.OpenElement(20, "span");
            builder.AddAttribute(21, "class", "form-input-suffix");

            if (SuffixType == InputLabelType.IconName)
            {
                builder.OpenComponent<LgIcon>(22);
                builder.AddAttribute(23, nameof(LgIcon.IconName), Suffix);
                builder.CloseComponent();
            }
            else
            {
                builder.AddContent(22, Suffix);
            }
            builder.CloseElement();
        }
        builder.OpenElement(30, "span");
        builder.AddMultipleAttributes(31, AdditionalAttributes);
        string cssClass = readOnly ? "txtb-ro" : $"form-control txtb{(hasPrefix ? " prefix" : "")}{(hasSuffix ? " suffix" : "")}";
        if (string.IsNullOrEmpty(Tooltip))
        {
            builder.AddAttribute(32, "class", $"summarybox-value text-truncate {cssClass}");
        }
        else
        {
            builder.AddAttribute(32, "class", $"summarybox-value {cssClass}");
            builder.AddAttribute(33, "title", Tooltip.CheckTranslate());
        }

        if (Disabled)
        {
            builder.AddAttribute(34, "disabled", "disabled");
        }
        int lastIndex = RenderAccessibilityAttribute(builder, 40);
        string displayedValue = !string.IsNullOrEmpty(CurrentValue) ?
            CurrentValue :
            (ReadOnly ? "emptyReadonlyValue".Translate() : Placeholder);
        builder.AddContent(100, displayedValue);
        builder.AddElementReferenceCapture(200, reference => ElementRef = reference);
        builder.CloseElement();
        builder.CloseElement();
    }

    /// <summary>
    /// Keyup event in modal mode only
    /// </summary>
    /// <param name="args"></param>
    private async Task KeyUpAsync(KeyboardEventArgs args)
    {
        if (args.Code == "Enter")
        {
            await OpenModalAsync();
        }
    }

    /// <summary>
    /// Open modal
    /// </summary>
    private async Task OpenModalAsync()
    {
        if (_modalVisible)
        {
            return;
        }
        _modalVisible = true;
        if (_modalVisible && ButtonValidation)
        {
            SummaryValue = Value;
        }
        if (OnOpen.HasDelegate)
        {
            await OnOpen.TryInvokeAsync(App);
        }
    }

    /// <summary>
    /// Value change event in the button validation mode
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private void SummaryValueChanged(ChangeEventArgs args)
    {
        SummaryValue = args.Value?.ToString();
    }

    /// <summary>
    /// Validate change in the button validation mode
    /// </summary>
    private Task ValidateChangeAsync()
    {
        return BaseChangeValueAsync(new ChangeEventArgs
        {
            Value = ButtonValidation ? SummaryValue : Value
        });
    }

    ///<inheritdoc/>
    protected override async Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value?.ToString();
        if (_modalVisible)
        {
            await CloseAsync();
        }
    }

    /// <summary>
    /// Cancel button in the button validation mode
    /// </summary>
    /// <returns></returns>        
    private async Task CloseAsync()
    {
        if (_dropDown is not null)
        {
            await JS.FocusAsync(_dropDown.ElementRef);
        }
        if (!Modal)
        {
            await _dropDown.CloseDropdownAsync(true);
        }
        else
        {
            _modalVisible = false;
        }
        if (OnClose.HasDelegate)
        {
            await OnClose.TryInvokeAsync(App);
        }
        else
        {
            StateHasChanged();
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, out string result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }

    #endregion

}
