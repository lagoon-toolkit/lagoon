namespace Lagoon.UI.Components.Internal;

internal interface ILgCheckBox<TValue>
{
    #region properties

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    public string CssClass { get; }

    /// <summary>
    /// Gets or sets the tooltip.
    /// </summary>
    public string Tooltip { get; set; }

    /// <summary>
    /// Gets or sets if the tooltip contents use the HTML format; else, the content use a raw text format.
    /// </summary>
    [Parameter]
    public bool TooltipIsHtml { get; set; }


    /// <summary>
    /// Gets the DOM element id.
    /// </summary>
    protected string ElementId { get; }

    /// <summary>
    /// Component is disabled.
    /// </summary>
    bool Disabled { get; set; }

    /// <summary>
    /// Value bound.
    /// </summary>
    TValue Value { get; set; }

    /// <summary>
    /// Checkbox description.
    /// </summary>
    public string Text { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Render the list item component.
    /// </summary>
    internal void OnRenderComponent(RenderTreeBuilder builder, bool isReadOnly, bool isRequired, bool isChecked,
        IReadOnlyDictionary<string, object> additionalAttributes,
        EventCallback onChangeCallback,
        DisplayOrientation orientation, CheckBoxKind kind, CheckBoxTextPosition textPosition, bool disabled,
        Action<ElementReference> elementReferenceCaptureAction,
        string ariaLabel, string ariaLabelledBy)
    {
        bool isDisabled = disabled || Disabled;

        // Build CSS class list
        LgCssClassBuilder cssClassBuilder = new("custom-control ", CssClass, "custom-checkbox");
        cssClassBuilder.AddIf(orientation == DisplayOrientation.Horizontal, "d-inline-block", "d-block");
        cssClassBuilder.AddIf(kind == CheckBoxKind.Check, "custom-checkbox", "custom-switch");
        cssClassBuilder.AddIf(textPosition == CheckBoxTextPosition.Left, "custom-control-right", "custom-control");
        cssClassBuilder.AddIf(kind == CheckBoxKind.Toggle && String.IsNullOrEmpty(Text), "custom-switch-noLbl", "custom-control");

        if (!isReadOnly)
        {

            //div
            builder.OpenElement(0, "div");
            builder.AddAttribute(4, "class", cssClassBuilder.ToString());
            //input
            builder.OpenElement(10, "input");
            builder.AddMultipleAttributes(11, additionalAttributes);
            builder.AddAttribute(12, "id", ElementId);
            builder.AddAttribute(13, "type", "checkbox");
            builder.AddAttribute(14, "checked", isChecked);
            builder.AddAttribute(15, "class", "custom-control-input");
            builder.AddAttribute(16, "disabled", isDisabled);
            builder.AddEventPreventDefaultAttribute(18, "onchange", true);
            builder.AddEventStopPropagationAttribute(19, "onchange", true);
            if (!isDisabled)
            {
                builder.AddAttribute(19, "onchange", onChangeCallback);
            }
            builder.AddEventPreventDefaultAttribute(20, "onkeydown", isReadOnly);
            builder.AddAttribute(21, "aria-label", ariaLabel);
            builder.AddAttribute(22, "aria-labelledby", ariaLabelledBy);
            if (isRequired)
            {
                builder.AddAttribute(23, "aria-required", "true");
            }

            if (elementReferenceCaptureAction is null)
            {
                // CheckBoxList
                builder.AddAttribute(25, "value", Value);
                builder.AddAttribute(26, "role", "option");
            }
            else
            {
                //CheckBox
                builder.AddElementReferenceCapture(27, elementReferenceCaptureAction);
            }
            builder.CloseElement(); //input
                                    //label
            builder.OpenElement(30, "label");
            builder.AddAttribute(31, "class", $"custom-control-label {(isRequired ? "mandatory" : "optional")}");
            builder.AddMultipleAttributes(32, LgComponentBase.GetTooltipAttributes(Tooltip, TooltipIsHtml));
            builder.AddAttribute(33, "style", "user-select:none");
            builder.AddAttribute(34, "for", ElementId);
            if (!isDisabled)
            {
                builder.AddAttribute(35, "onclick", onChangeCallback);
            }
            builder.AddEventPreventDefaultAttribute(36, "onclick", true);
            builder.AddEventStopPropagationAttribute(37, "onclick", true);
            builder.AddContent(38, Text.CheckTranslate());
            builder.CloseElement(); //label
            builder.CloseElement(); //div
        }
        else
        {
            // Readonly render
            bool? value;
            value = isChecked;
            string textValue = value switch
            {
                true => "lgCheckBoxCheckedText",
                false => "lgCheckBoxUncheckedText",
                null => "emptyReadonlyValue",
            };
            //label
            builder.OpenElement(0, "label");
            if (string.IsNullOrEmpty(Tooltip))
            {
                // No explicit tooltip, using the text-truncate class to show a tooltip with text content if necessary
                builder.AddAttribute(1, "class", $"chk-ro custom-control-label text-truncate " + cssClassBuilder.ToString());
            }
            else
            {
                // Using the explicit tooltip
                builder.AddAttribute(1, "class", $"chk-ro custom-control-label " + cssClassBuilder.ToString());
                builder.AddMultipleAttributes(2, LgComponentBase.GetTooltipAttributes(Tooltip, TooltipIsHtml));
            }
            builder.AddContent(3, textValue.Translate());
            builder.CloseElement(); //label
        }

    }

    #endregion

}
