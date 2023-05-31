using System.Text.RegularExpressions;

namespace Lagoon.UI.Components;

/// <summary>
/// Css unit size
/// </summary>
public readonly struct GridViewCssSize : IEquatable<GridViewCssSize>
{

    #region properties

    /// <summary>
    /// Gets or sets width value 
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Gets or sets width css unit
    /// </summary>
    public GridCssSizeUnit Unit { get; }

    #endregion

    #region constructors

    /// <summary>
    /// New instance implicitly created.
    /// </summary>
    /// <param name="unit">The unit with value 1.</param>
    public static implicit operator GridViewCssSize(GridCssSizeUnit unit)
    {
        return new GridViewCssSize(unit);
    }

    /// <summary>
    /// New instance implicitly created.
    /// </summary>
    /// <param name="value">The string representation of the size.</param>
    public static implicit operator GridViewCssSize(string value)
    {
        return new GridViewCssSize(value);
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="unit">The unit.</param>
    public GridViewCssSize(GridCssSizeUnit unit)
    {
        Unit = unit == GridCssSizeUnit.Auto ? GridCssSizeUnit.Fr : unit;
        Value = 1;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="pixel">The size in pixels.</param>
    /// <param name="minPixelValue">Minimum value if the unit is pixel.</param>
    /// <param name="maxPixelValue">Maximum value if the unit is pixel.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GridViewCssSize(int pixel, int minPixelValue, int maxPixelValue = 0) : this(GridCssSizeUnit.Px, pixel)
    {
        // Apply width limit for pixels
        if (Value < minPixelValue)
        {
            Value = minPixelValue;
        }
        if (maxPixelValue > 0 && Value > maxPixelValue)
        {
            Value = maxPixelValue;
        }
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="pixel">The size in pixels.</param>
    public GridViewCssSize(int pixel) : this(GridCssSizeUnit.Px, pixel) {}

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="unit">The size unit.</param>
    /// <param name="value">The value for the unit.</param>
    public GridViewCssSize(GridCssSizeUnit unit, int value)
    {
        Unit = unit;
        Value = value;
    }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="value">The string representation of the size.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GridViewCssSize(string value)
    {
        string lowerValue = value?.ToLowerInvariant()?.Trim();
        if (string.IsNullOrEmpty(lowerValue))
        {
            Unit = GridCssSizeUnit.Fr;
            Value = 1;
        }
        else
        {
            switch (lowerValue)
            {
                case "auto":
                    Unit = GridCssSizeUnit.Fr;
                    Value = 1;
                    break;
                default:
                    Match regEx = Regex.Match(lowerValue, "([0-9]+)(fr|px|%)");
                    if (regEx.Success)
                    {
                        Value = int.Parse(regEx.Groups[1].Value);
                        Unit = StringToUnit(regEx.Groups[2].Value);
                    }
                    else
                    {
                        throw new ArgumentException(value, nameof(value));
                    }
                    break;
            }
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Convert css unit to GridCssSizeUnit
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private static GridCssSizeUnit StringToUnit(string unit)
    {
        return unit switch
        {
            "fr" => GridCssSizeUnit.Fr,
            "%" => GridCssSizeUnit.Percent,
            _ => GridCssSizeUnit.Px,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Unit switch
        {
            GridCssSizeUnit.Percent => Value + "%",
            _ => Value + Unit.ToString().ToLowerInvariant(),
        };
    }

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Unit, Value);
    }

    ///<inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is GridViewCssSize size && Equals(size);
    }

    /// <summary>
    /// Returns <c>true</c> if the size are equals.
    /// </summary>
    /// <param name="other">The other size.</param>
    /// <returns><c>true</c> if the size are equals.</returns>
    public bool Equals(GridViewCssSize other)
    {
        return other.Unit == Unit && other.Value == Value;
    }

    /// <summary>
    /// Returns <c>true</c> if the size are equals.
    /// </summary>
    /// <param name="size">The other size.</param>
    /// <returns><c>true</c> if the size are equals.</returns>
    public bool Equals(string size)
    {
        GridViewCssSize other = new(size);
        return other.Unit == Unit && other.Value == Value;
    }

    ///<inheritdoc/>
    public static bool operator ==(GridViewCssSize left, GridViewCssSize right)
    {
        return left.Equals(right);
    }

    ///<inheritdoc/>
    public static bool operator !=(GridViewCssSize left, GridViewCssSize right)
    {
        return !left.Equals(right);
    }

    #endregion

}
