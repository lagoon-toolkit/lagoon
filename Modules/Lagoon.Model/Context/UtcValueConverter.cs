using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lagoon.Model.Context;

/// <summary>
/// Force the date to be understood as UTC date when loaded from the database.
/// </summary>
internal class UtcValueConverter : ValueConverter<DateTime, DateTime>
{

    /// <summary>
    /// New instance.
    /// </summary>
    public UtcValueConverter() : base(v => DateTimeToDbDateTime(v), v => DbDateTimeToLocalDateTime(v)) { }

    /// <summary>
    /// Convert DateTime value to UTC.
    /// </summary>
    /// <param name="dt">The source date time.</param>
    /// <returns>A UTC date time.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static DateTime DateTimeToDbDateTime(DateTime dt)
    {
        return dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            _ => dt.ToUniversalTime(),
        };
    }

    /// <summary>
    /// Convert DateTime, presumed to be UTC, to local DateTime.
    /// </summary>
    /// <param name="dt">The source date time from database.</param>
    /// <returns>A local date time.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static DateTime DbDateTimeToLocalDateTime(DateTime dt)
    {
        return dt.Kind switch
        {
            DateTimeKind.Local => dt,
            _ => dt.ToLocalTime(),
        };
    }

}

