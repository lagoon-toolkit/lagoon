namespace Lagoon.UI.Components;

/// <summary>
/// Browser informations.
/// </summary>
public class WindowInformationData
{

    #region properties

    /// <summary>
    /// Browser window inner height.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Browser window inner width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Device type "Mobile" else "Desktop".
    /// </summary>
    public bool Mobile { get; set; }

    /// <summary>
    /// Orientation "Portrait" else "Landscape".
    /// </summary>
    public bool Portrait { get; set; }

    #endregion

}
