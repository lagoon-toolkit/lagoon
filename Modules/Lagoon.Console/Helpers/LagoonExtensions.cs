namespace System;

internal static class ConsoleEx
{

    public static void WriteError(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public static void Write(string message)
    {
        Console.Write(message);
    }
    public static void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

}
