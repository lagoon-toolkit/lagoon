namespace Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Helpers to dynamic render.
/// </summary>
public static class LagoonExtensions
{

    #region Component render

    /// <summary>
    /// Add a cascading element to the render tree.
    /// </summary>
    /// <typeparam name="TValue">Cascading value type.</typeparam>
    /// <param name="builder">Builder manager.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="value">The value to be provided.</param>
    /// <param name="childContent">The content to which the value should be provided.</param>
    /// <param name="isFixed">If true, indicates that Microsoft.AspNetCore.Components.CascadingValue`1.Value
    ///     will not change. This is a performance optimization that allows the framework
    ///     to skip setting up change notifications. Set this flag only if you will not change
    ///     Microsoft.AspNetCore.Components.CascadingValue`1.Value during the component's
    ///     lifetime.</param>
    /// <param name="key">Assigns the specified key value to the CascadingValue.</param>
    /// <param name="name">Assign the specified name value to the CascadingValue.</param>
    public static void AddCascadingValueComponent<TValue>(this RenderTreeBuilder builder, int sequence, TValue value,
        MulticastDelegate childContent, bool isFixed = true, object key = null, string name = null)
    {
        builder.OpenRegion(sequence);
        builder.OpenComponent<CascadingValue<TValue>>(0);
        if (key is not null)
        {
            builder.SetKey(key);
        }
        builder.AddAttribute(1, nameof(CascadingValue<TValue>.Value), value);
        builder.AddAttribute(2, nameof(CascadingValue<TValue>.IsFixed), isFixed);
        builder.AddAttribute(3, nameof(CascadingValue<TValue>.ChildContent), childContent);
        if (name is not null)
        {
            builder.AddAttribute(3, nameof(CascadingValue<TValue>.Name), name);
        }
        builder.CloseComponent();
        builder.CloseRegion();
    }

    #endregion

}