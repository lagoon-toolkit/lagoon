namespace Lagoon.Core.Helpers;

/// <summary>
/// Defines an object wrapper.
/// </summary>
public class ObjectWrapper : IEqualityComparer<ObjectWrapper>
{

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectWrapper"/> class.
    /// </summary>
    /// <param name="value">The value of the object.</param>
    public ObjectWrapper(object value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value?.ToString();
    }

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x">The first object of type T to compare.</param>
    /// <param name="y">The second object of type T to compare.</param>
    /// <returns></returns>
    public bool Equals(ObjectWrapper x, ObjectWrapper y)
    {
        if (x is null)
        {
            return y is null;
        }
        else
        {
            return x.Value.Equals(y?.Value);
        }
    }

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(ObjectWrapper obj)
    {
        return obj?.Value?.GetHashCode() ?? 0;
    }

}