namespace Lagoon.UI.Components.Internal;

/// <summary>
/// Customizable menu set.
/// </summary>
public abstract class LgCustomMenuSet : ComponentBase, IDisposable
{
    #region fields

    private bool _disposedValue;

    #endregion

    #region cascading parameter

    /// <summary>
    /// LgMenuConfiguration application instance
    /// </summary>
    /// <value></value>
    [CascadingParameter]
    private LgMenuConfiguration MenuConfiguration { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// To uniquely identify a menu set
    /// </summary>
    /// <value></value>
    [Parameter]
    public string Id { get; set; }

    #endregion

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Check id existence
        if (string.IsNullOrEmpty(Id))
        {
            throw new InvalidOperationException($"{nameof(LgMenuSet)} must have an {nameof(Id)}");
        }
        // Add the menuset to the global list
        MenuConfiguration.RegisterMenuSet(this);
    }

    /// <summary>
    /// Disposing resources.
    /// </summary>
    /// <param name="disposing">if resource must be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
            if (disposing)
            {
                // Remove the menuset from the global list
                MenuConfiguration.UnregisterMenuSet(this);
            }
        }
    }

    /// <summary>
    /// Disposing resources.
    /// </summary>
    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the content of the menu set.
    /// </summary>
    /// <returns>The content of the menu set</returns>
    internal protected abstract RenderFragment GetContent();

    ///<inheritdoc/>
    protected override bool ShouldRender()
    {
        // Dont render the component: only the ChildContent is rendered in desired place when menu configuration changed.
        return false;
    }

    #endregion

}
