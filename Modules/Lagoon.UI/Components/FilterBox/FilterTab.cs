namespace Lagoon.UI.Components;

/// <summary>
/// Active filters tabs in FilterBox
/// </summary>
[Flags]
public enum FilterTab
{     
    /// <summary>
    /// Filter tab not displayed
    /// </summary>
    None = 0,
    /// <summary>
    /// Values list
    /// </summary>
    Selection,
    /// <summary>
    /// Inputs with operator
    /// </summary>
    Rules
}
