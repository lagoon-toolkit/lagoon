using Lagoon.UI.Components.Input.Internal;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Lagoon.UI.Components;

/// <summary>
/// Groups child <see cref="LgInputRadio{TValue}"/> components.
/// </summary>
public class LgInputRadioGroup<TValue> : LgInputRenderBase<TValue>
{

    #region Public properties

    /// <summary>
    /// Gets or sets the child content to be rendering inside the <see cref="LgInputRadioGroup{TValue}"/>.
    /// </summary>
    [Parameter]
    public RenderFragment Items { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [Parameter]
    public string Name { get; set; }

    /// <summary>
    /// Display Orientation
    /// </summary>
    [Parameter]
    public DisplayOrientation DisplayOrientation { get; set; }

    /// <summary>
    /// Gets or sets the radio button display kind
    /// </summary>
    [Parameter]
    public RadioButtonDisplayKind DisplayKind { get; set; }

    ///<inheritdoc/>
    protected override ElementReference FocusElementRef { get; }

    #endregion

    #region Private properties

    private readonly string _defaultGroupName = $"grp{Guid.NewGuid():N}";

    private LgInputRadioContext _context;

    [CascadingParameter]
    private LgInputRadioContext CascadedContext { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize a context which will be provided to childrens by cascading
    /// </summary>
    private void InitContext()
    {
        var groupName = !string.IsNullOrEmpty(Name) ? Name : _defaultGroupName;
        var fieldClass = EditContext?.FieldCssClass(FieldIdentifier.Value);
#if DEBUG //TOCLEAN
        Lagoon.Helpers.Trace.ToConsole(this, $"InitContext !");
#endif
        EventCallback<ChangeEventArgs> changeEventCallback = EventCallback.Factory.Create<ChangeEventArgs>(this, BaseChangeValueAsync);
        _context = new LgInputRadioContext(CascadedContext, groupName, CurrentValue, fieldClass, changeEventCallback)
        {
            CssClass = CssClass,
            Disabled = Disabled,
            DisplayOrientation = DisplayOrientation,
            DisplayKind = DisplayKind,
            ReadOnly = ReadOnly
        };
    }

    #endregion

    #region Render

    /// <inheritdoc />
    protected override void OnRenderComponent(RenderTreeBuilder builder)
    {
        InitContext();
        if (!ReadOnly)
        {
            Debug.Assert(_context != null);
            builder.OpenElement(1, "div");
            builder.AddAttribute(2, "role", "listbox");
            builder.AddAttribute(3, "class", $"form-input");
            var index = RenderAccessibilityAttribute(builder, 100);
            // Add attribute for validation
            if (AdditionalAttributes != null)
            {
                foreach (KeyValuePair<string, object> attribute in AdditionalAttributes)
                {
                    builder.AddAttribute(index++, attribute.Key, attribute.Value);
                }
            }
            builder.AddCascadingValueComponent(4, _context, Items, true, _context);
            builder.CloseElement();
        }
        else
        {
            // Readonly render
            if (Value is null)
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(4, "class", $"rbg-ro");
                RenderAccessibilityAttribute(builder, 100);
                builder.AddContent(6, "emptyReadonlyValue".Translate());
                builder.CloseElement();
            }
            else
            {
                builder.AddCascadingValueComponent(4, _context, Items, true, ElementId);
            }
        }
    }

    ///<inheritdoc/>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add("form-group-chk", "form-group", CssClass);
        builder.AddIf(ReadOnly, "form-group-ro");
    }

    /// <inheritdoc/>
    protected override bool HasActiveLabel()
    {
        return false;
    }
    #endregion

    #region Format

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        return TryParseSelectableValueFromString(value, out result, out validationErrorMessage);
    }

    private bool TryParseSelectableValueFromString(string value, out TValue result, out string validationErrorMessage)
    {
        try
        {
            // We special-case bool values because BindConverter reserves bool conversion for conditional attributes.
            if (typeof(TValue) == typeof(bool))
            {
                if (TryConvertToBool(value, out result))
                {
                    validationErrorMessage = null;
                    return true;
                }
            }
            else if (typeof(TValue) == typeof(bool?))
            {
                if (TryConvertToNullableBool(value, out result))
                {
                    validationErrorMessage = null;
                    return true;
                }
            }
            else if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.CurrentCulture, out var parsedValue))
            {
                result = parsedValue;
                validationErrorMessage = null;
                return true;
            }

            result = default;
            validationErrorMessage = $"The \"{FieldIdentifier?.FieldName}\" field is not valid.";
            return false;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"{GetType()} does not support the type '{typeof(TValue)}'.", ex);
        }
    }


    private static bool TryConvertToBool(string value, out TValue result)
    {
        if (bool.TryParse(value, out var @bool))
        {
            result = (TValue)(object)@bool;
            return true;
        }
        result = default;
        return false;
    }

    private static bool TryConvertToNullableBool(string value, out TValue result)
    {
        if (string.IsNullOrEmpty(value))
        {
            result = default;
            return true;
        }
        return TryConvertToBool(value, out result);
    }

    #endregion

    ///<inheritdoc/>
    protected override Task ChangeValueAsync(object value)
    {
        CurrentValueAsString = value?.ToString();
        return Task.CompletedTask;
    }
}
