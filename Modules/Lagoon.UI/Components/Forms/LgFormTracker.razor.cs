using Lagoon.UI.Components.Forms;

namespace Lagoon.UI.Components;


/// <summary>
/// Component used to track edit contexts modifications 
/// </summary>
public partial class LgFormTracker : LgComponentBase, IDisposable
{

    #region fields

    /// <summary>
    /// List of LgEditForm found by the FormTracker
    /// </summary>
    private readonly List<IFormTrackerComponent> _formTrackerCompnents = new();

    /// <summary>
    /// Previous modification state
    /// </summary>
    private bool? _hasModification = null;

    #endregion

    #region cascading parameters

    /// <summary>
    /// Handler for FormTrackers.
    /// </summary>
    [CascadingParameter]
    public IFormTrackerHandler FormTrackerHandler { get; set; }

    #endregion

    #region parameters

    /// <summary>
    /// Get or set the CascadingPolicy content
    /// </summary>
    /// <value>Renderfragment</value>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Get or set a value to set the modifcation pending state
    /// </summary>
    [Parameter]
    public bool? HasModification { get; set; } = null;

    /// <summary>
    /// Get or set binding for <see cref="HasModification"/>
    /// </summary>
    [Parameter]
    public EventCallback<bool?> HasModificationChanged { get; set; }

    #endregion

    /// <summary>
    /// On parameter set, check if the modification flag as been modified outside the component
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // First init
        if (HasModification.HasValue && !_hasModification.HasValue)
        {
            _hasModification = HasModification;
        }
        // Check if the modification flag has been set
        if (HasModification.HasValue && _hasModification.HasValue && _hasModification != HasModification)
        {
            foreach (IFormTrackerComponent ftc in _formTrackerCompnents)
            {
                ftc.SetModifiedState(HasModification.Value);
            }
            SetExplicitModificationStateAsync(HasModification.Value);
        }
    }

    #region methods

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        FormTrackerHandler.RegisterFormTracker(this);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        FormTrackerHandler.UnregisterFormTracker(this);
        // rq: when the LgFormTracker is used manually (eg with @bind-HasModification=...) we have to reset the HasModification property
        // (but with an LgEditContext it will be done automatically)
        HasModification = null;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Add IModifier to the monitoring list.
    /// </summary>
    /// <param name="component">EditForm.</param>
    internal void RegisterComponent(IFormTrackerComponent component)
    {
        _formTrackerCompnents.Add(component);
    }

    /// <summary>
    /// Remove IModifier from the monitoring list.
    /// </summary>
    /// <param name="component">EditForm.</param>
    internal void UnregisterComponent(IFormTrackerComponent component)
    {
        _formTrackerCompnents.Remove(component);
    }

    /// <summary>
    /// Check if one form into formtracker is modified ?
    /// </summary>
    /// <returns><c>true</c> if there are pending modification. <c>false</c> otherwise</returns>
    internal bool IsModified()
    {
        if (HasModification.HasValue)
        {
            return HasModification.Value;
        }
        foreach (IFormTrackerComponent ftc in _formTrackerCompnents)
        {
            if (ftc.IsModified())
            {
                return true;
            }
        }
        return false;
    }

    //https://github.com/dotnet/aspnetcore/issues/25696
#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
    internal async void SetExplicitModificationStateAsync(bool hasChanges = true)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
    {
        try
        {
            if (HasModification.HasValue)
            {
                HasModification = hasChanges;
                _hasModification = hasChanges;
                await HasModificationChanged.TryInvokeAsync(App, HasModification);
                FormTrackerHandler?.RaiseFieldChanged();
            }
            else if (!HasModification.HasValue)
            {
                FormTrackerHandler?.RaiseFieldChanged();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("LgFormTracker - An error occured", ex.Message);
        }
    }

    #endregion

}
