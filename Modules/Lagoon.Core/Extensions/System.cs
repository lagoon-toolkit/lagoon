using Lagoon.Core.Application;
using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// Helper extension methods
/// </summary>
public static class LagoonExtensions
{

    #region Assembly

    /// <summary>
    /// Return the product version of the specified assembly.
    /// </summary>
    /// <param name="asm">The assembly.</param>
    /// <returns>The product version of the specified assembly.</returns>
    public static string GetProductVersion(this Assembly asm)
    {
        try
        {
            // FileVersionInfo.GetVersionInfo(asm.Location).ProductVersion is not compatible with Wasm applications
            return asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Extensions methods for types

    /// <summary>
    /// Returns the type if not <c>Nullable</c>, otherwise the underlying type.
    /// </summary>
    /// <param name="type">Type source.</param>
    /// <returns>The type if not <c>Nullable</c>, otherwise the underlying type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetNullableUnderlyingTypeOrThis(this Type type)
    {
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    /// Return the friendly name for generic types or the type name for others.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static string FriendlyName(this Type type)
    {
        if (type is null)
        {
            return "null";
        }
        if (type.IsGenericType)
        {
            string genericArguments = string.Join(",", type.GetGenericArguments().Select(FriendlyName));
            int pos = type.Name.IndexOf("`");
            if (pos < 1)
            {
                return $"{type.Name}<{genericArguments}>";
            }
            else
            {
                string typeName = type.Name[..pos];
                return typeName == nameof(Nullable) ? $"{genericArguments}?" : $"{typeName}<{genericArguments}>";
            }
        }
        return type.Name;
    }

    /// <summary>
    /// Return the number of digit to use by default for the type of data.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetDefaultDecimalDigits(this Type type)
    {
        return Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type) switch
        {
            TypeCode.Decimal or TypeCode.Double or TypeCode.Single => 2,
            _ => 0,
        };
    }

    #endregion

    #region Extensions methods for the dictionnaries

    /// <summary>
    /// Return the corresponding value from the dictionnary.
    /// </summary>
    /// <param name="key">The key to look up in the dictionary</param>
    /// <returns>The translated value.</returns>
    /// <remarks>Use this method to avoid message "Client projection  contains reference to constant expression..." when used in linq select.</remarks>
    public static string Translate(this string key)
    {
        return LgApplicationBase.Current.Dico(key);
    }

    /// <summary>
    /// Return the corresponding value from the dictionnary. Use <paramref name="args"/> to format the result
    /// </summary>
    /// <param name="key">The key to look up in the dictionary</param>
    /// <param name="args">Optional <see cref="string.Format(string, object[])"/> arguments</param>
    /// <returns>The transalated value.</returns>
    public static string Translate(this string key, params object[] args)
    {
        return LgApplicationBase.Current.Dico(key, args);
    }

    #endregion

    #region Extensions methods for timespan

    /// <summary>
    /// Format the time in :
    /// - Milliseconds if the value is less than 3 seconds
    /// - Seconds if the value is less than 1 minute
    /// - [Hours] minutes and seconds in other cases
    /// </summary>
    /// <param name="timeSpan">The timespan to show.</param>
    /// <returns>The formated timespan.</returns>
    public static string ToSmartFormat(this TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 3)
        {
            // Show milliseconds
            return $"{timeSpan.TotalMilliseconds} ms";
        }
        else if (timeSpan.TotalMinutes < 1)
        {
            // Show seconds
            return $"{timeSpan.TotalSeconds} s";
        }
        else
        {
            // Show (hours,) minutes and seconds
            StringBuilder sb = new(9);
            int totalHours = (int)timeSpan.TotalHours;
            if (totalHours > 0)
            {
                sb.Append(totalHours);
                sb.Append(':');
            }
            sb.Append(timeSpan.Minutes.ToString("mm"));
            sb.Append(':');
            sb.Append(timeSpan.Seconds.ToString("ss"));
            return sb.ToString();
        }
    }

    #endregion

    #region Extensions methods for UriBuilder


    /// <summary>
    /// Gets the absolute URI without adding the port if it is the default one for the schema (unlike the ToString() method).
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    public static string AbsoluteUri(this UriBuilder uriBuilder)
    {
        return uriBuilder?.Uri.AbsoluteUri;
    }

    /// <summary>
    /// Adds a or replaces a parameter in the URI.
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="escape">Indicates if the value should be encoded.</param>
    public static UriBuilder AddParameter(this UriBuilder uriBuilder, string name, string value, bool escape = true)
    {

        if (!string.IsNullOrEmpty(name))
        {
            uriBuilder.RemoveParameter(name);
            if (!string.IsNullOrEmpty(value))
            {
                StringBuilder sb = new(uriBuilder.Query);
                if (sb.Length != 0)
                {
                    sb.Append('&');
                }
                sb.Append(name);
                sb.Append('=');
                sb.Append(escape ? Uri.EscapeDataString(value) : value);
                uriBuilder.Query = sb.ToString();
            }
        }
        return uriBuilder;
    }

    /// <summary>
    /// Adds a or replaces a parameter in the URI.
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="value">Value of the parameter.</param>
    public static UriBuilder AddParameter(this UriBuilder uriBuilder, string name, int value)
    {
        uriBuilder.AddParameter(name, value.ToString(), false);
        return uriBuilder;
    }

    /// <summary>
    /// Adds a or replaces a parameter in the URI.
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="value">Value of the parameter.</param>
    public static UriBuilder AddParameter(this UriBuilder uriBuilder, string name, long value)
    {
        uriBuilder.AddParameter(name, value.ToString(), false);
        return uriBuilder;
    }

    /// <summary>
    /// Adds a or replaces a parameter in the URI.
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="value">Value of the parameter.</param>
    public static UriBuilder AddParameter(this UriBuilder uriBuilder, string name, DateTime value)
    {
        uriBuilder.AddParameter(name, value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK"), false);
        return uriBuilder;
    }

    /// <summary>
    /// Adds a or replaces a parameter in the URI.
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="value">Value of the parameter.</param>
    public static UriBuilder AddParameter(this UriBuilder uriBuilder, string name, object value)
    {
        uriBuilder.AddParameter(name, value?.ToString(), true);
        return uriBuilder;
    }

    /// <summary>
    /// Deletes the parameter whose name is given as a parameter.
    /// </summary>
    /// <param name="uriBuilder">The URI builder.</param>
    /// <param name="name">Parameter name.</param>
    public static UriBuilder RemoveParameter(this UriBuilder uriBuilder, string name)
    {
        string query = uriBuilder.Query;
        if (!string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(name))
        {
            int pos = 0;
            int start = 0;
            while (true)
            {
                pos = query.IndexOf($"{name}=", pos, StringComparison.OrdinalIgnoreCase);
                if (pos == -1)
                {
                    return uriBuilder;
                }
                if (pos == 0)
                {
                    break;
                }
                if (query[pos - 1] is '?' or '&')
                {
                    start = pos - 1;
                    break;
                }
                pos += 2;
            }
            int end = query.IndexOf('&', start + 2);
            if (end == -1)
            {
                end = query.Length;
            }
            uriBuilder.Query = query.Remove(start, end - start);
        }
        return uriBuilder;
    }

    #endregion

}