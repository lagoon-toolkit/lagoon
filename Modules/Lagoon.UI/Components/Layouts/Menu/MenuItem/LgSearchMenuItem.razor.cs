namespace Lagoon.UI.Components;


/// <summary>
/// Item to do a global search.
/// </summary>
public partial class LgSearchMenuItem : LgSearchNavigation<LgPageLink>
{
    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Scrollable = true;
    }
}
