namespace Lagoon.Generators;


#if DEBUG
internal static class Log
{

    private const string TRACE_PATH = @"C:\Temp\";
    private static object _fileLock = new();

    /// <summary>
    /// Trace debug informations to the console.
    /// </summary>
    /// <param name="message">Message to show.</param>
    /// <param name="filePath">File name set by the compiler (leave empty).</param>
    /// <param name="memberName">Method name set by the compiler (leave empty).</param>
    public static void ToFile(string message,
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
    {
        try
        {
            string exeName = System.Diagnostics.Process.GetCurrentProcess()?.ProcessName;
            lock (_fileLock)
            {
                File.AppendAllText($"{TRACE_PATH}LRG-{exeName}.txt", $"[{Path.GetFileNameWithoutExtension(filePath).Replace('.', '-')}.{memberName}] {message}\n");
            }
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// Trace debug informations to the console.
    /// </summary>
    /// <param name="caller">Instance calling the trace. Set "this" here.</param>
    /// <param name="message">Message to show.</param>
    /// <param name="memberName">Method name set by the compiler (leave empty).</param>
    public static void ToFile(object caller, string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
    {
        try
        {
            string exeName = System.Diagnostics.Process.GetCurrentProcess()?.ProcessName;
            lock (_fileLock)
            {
                File.AppendAllText($"{TRACE_PATH}LRG-{exeName}.txt", $"[{GetTypeFormattedName(caller.GetType())}.{memberName}] {message}\n");
            }
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// Returns the type name. If this is a generic type, appends
    /// the list of generic type arguments between angle brackets.
    /// (Does not account for embedded / inner generic arguments.)
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>System.String.</returns>
    public static string GetTypeFormattedName(Type type)
    {
        if (type is null)
        {
            return "null";
        }
        if (type.IsGenericType)
        {
            string genericArguments = string.Join(",", type.GetGenericArguments().Select(GetTypeFormattedName));
            int pos = type.Name.IndexOf("`");
            if (pos < 1)
            {
                return $"{type.Name}<{genericArguments}>";
            }
            else
            {
                string typeName = type.Name.Substring(0, pos);
                if (typeName == nameof(Nullable))
                {
                    return $"{genericArguments}?";
                }
                else
                {
                    return $"{typeName}<{genericArguments}>";
                }
            }
        }
        return type.Name;
    }

}

#endif
