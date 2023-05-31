namespace Lagoon.Helpers;


/// <summary>
/// The result of the remote task execution.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class TaskEndEventArgs<TResult> : TaskEndEventArgs
{

    /// <summary>
    /// The result of the executed task.
    /// </summary>
    [JsonPropertyName("value")]
    public TResult Result { get; set; }


    /// <summary>
    /// New instance with successful result.
    /// </summary>
    /// <param name="result"></param>
    public TaskEndEventArgs(TResult result)
    {
        Result = result;
    }

}

/// <summary>
/// The result of the remote task execution.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class TaskEndEventArgs : EventArgs
{
    /// <summary>
    /// The error description.
    /// </summary>
    [JsonPropertyName("error")]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// New instance.
    /// </summary>
    protected TaskEndEventArgs()
    {
    }

    /// <summary>
    /// New instance with error.
    /// <paramref name="errorMessage">The error message.</paramref>
    /// </summary>
    public TaskEndEventArgs(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Serialize the result to Json.
    /// </summary>
    /// <returns></returns>
    public string GetJson()
    {
        // We use the GetType() to don't ignore child class properties
        return System.Text.Json.JsonSerializer.Serialize(this, GetType());
    }

}