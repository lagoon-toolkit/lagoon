namespace Lagoon.UI.Components.Internal;

internal class SelectAllManager
{

    public event Action OnUpdated;

    internal void UpdateRender()
    {
        OnUpdated?.Invoke();
    }
}
