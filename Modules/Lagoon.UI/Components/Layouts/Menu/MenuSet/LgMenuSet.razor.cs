namespace Lagoon.UI.Components;


/// <summary>
/// Component used to declare a set of menu inside a <see cref="LgMenuConfiguration.MenuSetDeclarations"/> identified by an Id
/// A <see cref="LgMenuSet"/> can specify a PolicyVisible to restrict the menu to an existing Policy
/// Used by <see cref="LgMenuConfiguration" />
/// </summary>
public sealed partial class LgMenuSet : LgCustomMenuSet
{

    #region render fragments

    /// <summary>
    /// Menu set composition.
    /// </summary>
    /// <value></value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected internal override RenderFragment GetContent()
    {
        return ChildContent;
    }

    #endregion

}
