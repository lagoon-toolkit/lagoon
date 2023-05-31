namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Component that provide a page default layout.
/// </summary>
internal interface IPageDefaultLayoutProvider
{

    /// <summary>
    /// Return the default layout to use.
    /// </summary>
    /// <returns></returns>
    Type GetDefaultLayout();
}
