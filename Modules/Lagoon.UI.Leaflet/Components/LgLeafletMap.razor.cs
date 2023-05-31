namespace Lagoon.UI.Leaflet.Components;

/// <summary>
/// Component Map.
/// </summary>
public partial class LgLeafletMap : LgComponentBase
{
    /// <summary>
    /// Gets or sets css class of the container
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }


    /// <summary>
    /// Gets or sets the Map 
    /// </summary>
    [Parameter]
    public LeafletMap Map { get; set; }

    /// <summary>
    /// Add CSS classes to the "class" attribute.
    /// </summary>
    /// <param name="builder">CSS class builder.</param>
    protected override void OnBuildClassAttribute(LgCssClassBuilder builder)
    {
        builder.Add(CssClass);
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Run(()=>
            {
                LgLeafletInterops.Create(JS, Map);
                Map.RaiseOnInitialized();
            });
        }
    }

}
