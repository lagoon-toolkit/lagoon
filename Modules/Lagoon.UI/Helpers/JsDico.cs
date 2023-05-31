using Lagoon.Core.Application;

namespace Lagoon.UI.Helpers;

/// <summary>
/// Handle multi-langual dictionnary from javascript.
/// </summary>
public class JsDico
{

    /// <summary>
    /// Access dictionnary from javascript side
    /// </summary>
    /// <param name="dicoKey">The key to translate</param>
    /// <param name="args">Args to be replaced in string</param>
    [JSInvokable]
    public static string JsDicoTranslate(string dicoKey, params string[] args)
    {
        return LgApplicationBase.Current.Dico(dicoKey, args);
    }

}
