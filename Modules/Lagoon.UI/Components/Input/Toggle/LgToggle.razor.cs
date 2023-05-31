using Lagoon.UI.Components.Input.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;

/// <summary>
/// A toggle component.
/// </summary>
public partial class LgToggle : LgInputRenderBase<bool>
{

    #region parameters

    /// <summary>
    /// Gets or sets a callback called before the value is changed.
    /// </summary>
    [Parameter]
    public EventCallback<ChangingEventArgs<bool>> OnChanging { get; set; }

    /// <summary>
    /// Gets or sets the toggle text on.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the toggle text off.
    /// </summary>
    [Parameter]
    public string TextOff { get; set; }

    /// <summary>
    /// Gets or sets the toggle text on.
    /// </summary>
    [Parameter]
    public string TextOn { get; set; }

    /// <summary>
    /// Gets or sets the text disposition
    /// </summary>
    [Parameter]
    public ToggleTextPosition TextPosition { get; set; }

    #endregion

    #region properties

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef => ElementRef;

    /// <summary>
    /// Gets or sets toggle text render
    /// </summary>
    protected RenderFragment TextToggle { get; set; }

    /// <summary>
    /// Gets or sets toggle render
    /// </summary>
    protected RenderFragment Toggle { get; set; }

    #endregion

    #region field

    /// <summary>
    /// Gets or sets the DOM element reference.
    /// </summary>
    private ElementReference ElementRef;

    #endregion

    #region methods

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("form-group-toggle", "form-group", CssClass);
        builder.AddIf(Disabled, "disabled");
        builder.AddIf(ReadOnly, "readonly");
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return false;
    }

    /// <summary>
    /// Click on Text set the toggle value
    /// </summary>
    /// <returns></returns>
    private async Task OnClickTextAsync()
    {
        if (Disabled)
        {
            return;
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.TryInvokeAsync(App, Value);
        }
        EditContext?.NotifyFieldChanged(FieldIdentifier.Value);
        if (OnChange.HasDelegate)
        {
            await OnChange.TryInvokeAsync(App, new ChangeEventArgs() { Value = Value });
        }
    }

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValue = (bool)value;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked when the value is changed.
    /// </summary>
    internal async Task OnValueChangeAsync(ChangeEventArgs args)
    {
        if (OnChanging.HasDelegate)
        {
            await OnChanging.TryInvokeAsync(App, (ChangingEventArgs<bool>)args.Value);
        }
        await BaseChangeValueAsync(args);
    }

    #endregion

    #region render

    ///<inheritdoc/>
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {          
        // Force render of the content
        if(GetValueAttribute(CurrentValueAsString) is not null)
        {
            builder.AddContent(0, Toggle);
        }            
    }

    ///<inheritdoc/>
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out bool result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        throw new NotImplementedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }

    ///<inheritdoc/>
    internal override bool TryParseValueFromString(string value, out bool parsedValue)
    {
        parsedValue = CurrentValue;
        return true;
    }

    #endregion
}
