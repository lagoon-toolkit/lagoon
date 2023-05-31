namespace Lagoon.Helpers;

/// <summary>
/// Text case and accent sensitivity.
/// </summary>
public enum CollationType
{

    /// <summary>
    /// Case sensitive and accent sensitive.
    /// </summary>
    CaseSensitive,

    /// <summary>
    /// Case insensitive and accent sensitive.
    /// </summary>
    IgnoreCase,

    /// <summary>
    /// Case insensitive and accent insensitive.
    /// </summary>
    IgnoreCaseAndAccent

}