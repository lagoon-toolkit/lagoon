namespace Lagoon.UI.Components;

/// <summary>
/// Client remote progress
/// </summary>
internal class RemoteTaskProgress : Progress
{

    /// <summary>
    /// Report progression 
    /// </summary>
    /// <param name="position">progerss position</param>
    /// <param name="message">progress message</param>
    /// <param name="updatePosition">progress update positon?</param>
    /// <param name="updateMessage">progress update message?</param>
    /// <param name="autoEnd">auto end?</param>
    public void RemoteReport(int position, string message, bool updatePosition, bool updateMessage, bool autoEnd = true)
    {

        base.Report(position, message, updatePosition, updateMessage, autoEnd);
    }

}
