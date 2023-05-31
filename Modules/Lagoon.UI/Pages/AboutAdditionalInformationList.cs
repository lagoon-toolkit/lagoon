namespace Lagoon.UI.Components;

/// <summary>
/// Information list.
/// </summary>
public class AboutAdditionalInformationList : List<AboutAdditionalInformation>
{
    /// <summary>
    /// Add new about information.
    /// </summary>
    /// <param name="label">Label.</param>
    /// <param name="text">Information.</param>
    /// <param name="uri">URI</param>
    public void Add(string label, string text, string uri)
    {
        Add(new AboutAdditionalInformation() { Label = label, Name = text, Uri = uri });
    }
}
