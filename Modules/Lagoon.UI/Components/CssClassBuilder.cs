namespace Lagoon.UI.Components;

/// <summary>
/// Helper to concatenate CSS classes.
/// </summary>
public class LgCssClassBuilder
{

    #region private fields

    /// <summary>
    /// The collection of CSS classes.
    /// </summary>
    private readonly HashSet<string> _cssClasses = new(8);

    #endregion

    #region contructor

    /// <summary>
    /// Initializes a new instance of the <see cref="LgCssClassBuilder"/> class.
    /// </summary>
    /// <param name="cssClasses">The collection of CSS classes to add.</param>
    public LgCssClassBuilder(params string[] cssClasses)
    {
        Add(cssClasses);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Add new class to class attribute.
    /// </summary>
    /// <param name="cssClasses">New CSS classes to add.</param>
    public void Add(params string[] cssClasses)
    {
        foreach (string cssClass in cssClasses)
        {
            if (!string.IsNullOrEmpty(cssClass))
            {
                _cssClasses.Add(cssClass);
            }
        }
    }

    /// <summary>
    /// Add new class to class attribute.
    /// </summary>
    /// <param name="condition">Indicate if the following classes must be added.</param>
    /// <param name="trueCssClass">New CSS classes to add.</param>
    public void AddIf(bool condition, string trueCssClass)
    {
        if (condition)
        {
            if (!string.IsNullOrEmpty(trueCssClass))
            {
                _cssClasses.Add(trueCssClass);
            }
        }
    }

    /// <summary>
    /// Add new class to class attribute.
    /// </summary>
    /// <param name="condition">Indicate if the following classes must be added.</param>
    /// <param name="trueCssClass">New CSS classes to add if condition is true.</param>
    /// <param name="falseCssClass">New CSS classes to add if condition is false.</param>
    public void AddIf(bool condition, string trueCssClass, string falseCssClass)
    {
        if (condition)
        {
            if (!string.IsNullOrEmpty(trueCssClass))
            {
                _cssClasses.Add(trueCssClass);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(falseCssClass))
            {
                _cssClasses.Add(falseCssClass);
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.Join(' ', _cssClasses);
    }

    #endregion
}