namespace Lagoon.UI.Components;

/// <summary>
/// Exception raised when an HTTP request failed.
/// </summary>
public class LgHttpFetchException : Exception
{
    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="ex">Exception.</param>
    public LgHttpFetchException(Exception ex) : base("Failed to fetch.", ex)
    {
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return GetType().ToString() + " > " + InnerException.GetType().FullName + ": " + InnerException.Message + "\n" + StackTrace;
    }

}
