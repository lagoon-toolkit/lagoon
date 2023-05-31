namespace Lagoon.UI.Components;

/// <summary>
/// Interface to expose the LgSelect property
/// </summary>
public abstract class LgProgressBase : LgComponentBase
{

    #region parameters

    /// <summary>
    /// Gets or sets the CSS classes.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [Parameter]
    public int Value { get; set; } = 0;

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    [Parameter]
    public int Min { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    [Parameter]
    public int Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    [Parameter]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the kind of the button.
    /// </summary>
    [Parameter]
    public Kind Kind { get; set; } = Kind.Primary;

    #endregion

    #region properties

    /// <summary>
    /// Indicate if the status is indeterminate.
    /// </summary>
    protected bool IsIndeterminate { get; set; }

    /// <summary>
    /// The percentage of the value.
    /// </summary>
    protected int ValuePercent { get; set; }

    /// <summary>
    /// Label to show.
    /// </summary>
    protected string LabelRendering { get; set; }

    /// <summary>
    /// Indicate if a label haven't been set to the component.
    /// </summary>
    protected bool HasAutoLabel { get; set; }

    /// <summary>
    /// Gets progress done.
    /// </summary>
    protected bool IsDone => Value >= Max;

    #endregion

    #region methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Min > Max)
        {
            throw new InvalidOperationException("lgProgressBarErrorMinMax".Translate());
        }
        LabelRendering = Label.CheckTranslate();
        HasAutoLabel = string.IsNullOrEmpty(LabelRendering);
        IsIndeterminate = Max <= Min;
        if(IsIndeterminate)
        {
            ValuePercent = 100;
            if (HasAutoLabel)
            {
                LabelRendering = null;
            }
        }
        else
        {
            ValuePercent = (Math.Clamp(Value, Min, Max) - Min) * 100 / (Max - Min);
            if (HasAutoLabel)
            {
                LabelRendering = $"{ValuePercent}%";
            }
        }
    }

    #endregion
}
