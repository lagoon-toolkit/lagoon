namespace Lagoon.Helpers;

/// <summary>
/// Indicate if the property must be ignored when exporting object.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExportIgnoreAttribute : Attribute
{
}
