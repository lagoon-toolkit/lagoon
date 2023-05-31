namespace Lagoon.UI.Demo.Tests;

[Route(ROUTE)]
public partial class SpeedTest : LgPage
{
    #region URL, Icon and title for this page

    /// <summary>
    /// Route to reach the page.
    /// </summary>
    private const string ROUTE = "SpeedTest";

    /// <summary>
    /// Gets the URL for this page.
    /// </summary>
    /// <returns>The URL for this page.</returns>
    public static LgPageLink Link()
    {
        return new LgPageLink(ROUTE, "SpeedTest", IconNames.All.Flower1);
    }

    #endregion

    private int _loopCount = 100;

    public void DoLoop()
    {
        DateTime start;
        DateTime stop;
        string text0 = "AZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOP";
        string text1 = "AZERTYUIOP";
        string text2 = "AZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOPAZERTYUIOP";
        string text3 = text2[^1] + "X";
        int hash0 = 0;
        int hash1 = 0;
        int hash2 = 0;

#if DEBUG //TOCLEAN
        Lagoon.Helpers.Trace.ToConsole(this, $"{text2.Length} == {text3.Length}");
#endif

        start = DateTime.UtcNow;
        for (int i = 0; i < _loopCount; i++)
        {
            hash0 = text0.GetHashCode();
        }
        stop = DateTime.UtcNow;
#if DEBUG //TOCLEAN
        Lagoon.Helpers.Trace.ToConsole(this, $"{hash0} Time : {stop.Subtract(start).TotalMilliseconds}");
#endif

        start = DateTime.UtcNow;
        for (int i = 0; i < _loopCount; i++)
        {
            hash1 = text1.GetHashCode();
        }
        stop = DateTime.UtcNow;
#if DEBUG //TOCLEAN
        Lagoon.Helpers.Trace.ToConsole(this, $"{hash1} Time : {stop.Subtract(start).TotalMilliseconds}");
#endif

        start = DateTime.UtcNow;
        for (int i = 0; i < _loopCount; i++)
        {
            hash2 = text2.GetHashCode();
        }
        stop = DateTime.UtcNow;
#if DEBUG //TOCLEAN
        Lagoon.Helpers.Trace.ToConsole(this, $"{hash2} Time : {stop.Subtract(start).TotalMilliseconds}");
#endif

    }

}
