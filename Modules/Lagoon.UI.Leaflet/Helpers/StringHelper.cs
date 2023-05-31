namespace Lagoon.UI.Leaflet.Helpers;

/// <summary>
/// String Helper
/// </summary>
public class StringHelper
{

    private static readonly Random _random = new();

    /// <summary>
    ///  return a random string
    /// </summary>
    /// <param name="length">number of char</param>
    /// <returns>random string.</returns>
    public static string GetRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

}
