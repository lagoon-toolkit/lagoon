using System.ComponentModel.DataAnnotations;

namespace Lagoon.UI.Components;

/// <summary>
/// Search mode for a filter condition.
/// </summary>
public enum FilterTextSearchMode
{
    /// <summary>
    /// The search text is contained in the value.
    /// </summary>
    [Display(Name = "#FilterTextSearchModeContains")]
    Contains,

    /// <summary>
    /// The value starts with the search text.
    /// </summary>
    [Display(Name = "#FilterTextSearchModeStartsWith")]
    StartsWith,

    /// <summary>
    /// The value ends with the search text.
    /// </summary>
    [Display(Name = "#FilterTextSearchModeEndsWith")]
    EndsWith
}
