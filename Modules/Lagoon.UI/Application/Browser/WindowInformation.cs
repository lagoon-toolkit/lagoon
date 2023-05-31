namespace Lagoon.UI.Components;

/// <summary>
/// Browser informations.
/// </summary>
public class WindowInformation
{
    #region fields

    private readonly WindowInformationData _data;

    #endregion

    #region properties

    /// <summary>
    /// Browser window height (change when the keyboard is opened on mobile devices).
    /// </summary>
    public int Height => _data.Height;

    /// <summary>
    /// Browser window height (change when the keyboard is opened on mobile devices).
    /// </summary>
    public int Width => _data.Width;

    /// <summary>
    /// Orientation "Landscape" or "Portrait".
    /// </summary>
    public MediaOrientation MediaOrientation => _data.Portrait ? MediaOrientation.Portrait : MediaOrientation.Landscape;

    /// <summary>
    /// Media type detected.
    /// </summary>
    public MediaType MediaType => _data.Mobile ? MediaType.Mobile : MediaType.Desktop;

    #endregion

    #region constructors

    internal WindowInformation(WindowInformationData data)
    {
        _data = data;
    }

    #endregion

    #region methods

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"Height: {Height}, Width: {Width}, MediaOrientation: {MediaOrientation}, MediaType: {MediaType}";
    }

    #endregion

}
