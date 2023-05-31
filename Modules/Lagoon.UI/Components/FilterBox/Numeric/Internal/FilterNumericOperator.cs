using System.ComponentModel.DataAnnotations;

namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Numeric search operator
/// </summary>
internal enum FilterNumericOperator
{
    [Display(Name = "#FilterBoxNumericEqual")]
    Equal,
    [Display(Name = "#FilterBoxNumericNotEqual")]
    NotEqual,
    [Display(Name = "#FilterBoxNumericLesser")]
    LessThan,
    [Display(Name = "#FilterBoxNumericGreater")]
    GreaterThan,
    [Display(Name = "#FilterBoxNumericLesserOrEqual")]
    LessThanOrEqual,
    [Display(Name = "#FilterBoxNumericGreaterOrEqual")]
    GreaterThanOrEqual
}
