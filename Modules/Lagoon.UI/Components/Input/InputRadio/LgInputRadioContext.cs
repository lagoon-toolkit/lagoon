namespace Lagoon.UI.Components;

/// <summary>
/// Describes context for an <see cref="LgInputRadio{TValue}"/> component.
/// </summary>
internal class LgInputRadioContext
{
    private readonly LgInputRadioContext _parentContext;

    /// <summary>
    /// Gets the name of the input radio group.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Gets the current selected value in the input radio group.
    /// </summary>
    public object CurrentValue { get; }

    /// <summary>
    /// Gets a css class indicating the validation state of input radio elements.
    /// </summary>
    public string FieldClass { get; }

    /// <summary>
    /// Gets or sets the input css class
    /// </summary>
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the input disabled attribute
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Display Orientation
    /// </summary>
    public DisplayOrientation DisplayOrientation { get; internal set; }

    /// <summary>
    /// Gets or sets the radio button display kind 
    /// </summary>
    public RadioButtonDisplayKind DisplayKind { get; set; }

    /// <summary>
    /// Gets or sets the radio button readonly attribute
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets the event callback to be invoked when the selected value is changed.
    /// </summary>
    public EventCallback<ChangeEventArgs> ChangeEventCallback { get; }

    /// <summary>
    /// Instantiates a new <see cref="LgInputRadioContext" />.
    /// </summary>
    /// <param name="parentContext">The parent <see cref="LgInputRadioContext" />.</param>
    /// <param name="groupName">The name of the input radio group.</param>
    /// <param name="currentValue">The current selected value in the input radio group.</param>
    /// <param name="fieldClass">The css class indicating the validation state of input radio elements.</param>
    /// <param name="changeEventCallback">The event callback to be invoked when the selected value is changed.</param>
    public LgInputRadioContext(
        LgInputRadioContext parentContext,
        string groupName,
        object currentValue,
        string fieldClass,
        EventCallback<ChangeEventArgs> changeEventCallback)
    {
        _parentContext = parentContext;

        GroupName = groupName;
        CurrentValue = currentValue;
        FieldClass = fieldClass;
        ChangeEventCallback = changeEventCallback;
    }

    /// <summary>
    /// Finds an <see cref="LgInputRadioContext"/> in the context's ancestors with the matching <paramref name="groupName"/>.
    /// </summary>
    /// <param name="groupName">The group name of the ancestor <see cref="LgInputRadioContext"/>.</param>
    /// <returns>The <see cref="LgInputRadioContext"/>, or <c>null</c> if none was found.</returns>
    public LgInputRadioContext FindContextInAncestors(string groupName)
    {
        return string.Equals(GroupName, groupName) ? this : _parentContext?.FindContextInAncestors(groupName);
    }



    // public override int GetHashCode()
    // {
    //     return Disabled.GetHashCode() + ;
    // }
}
