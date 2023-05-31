using System.Security.Claims;

namespace Lagoon.Helpers;

/// <summary>
/// Extensions for claims collection.
/// </summary>
public static partial class Extensions
{

    #region GetFirstName

    /// <summary>
    /// Get the user first name.
    /// </summary>
    /// <param name="claims">a list of claims.</param>
    /// <returns>The first name of the user.</returns>
    public static string GetFirstName(this IEnumerable<Claim> claims)
    {
        string value;
        foreach (Claim claim in claims)
        {
            value = claim.Value;
            if (!string.IsNullOrEmpty(value))
            {
                if (IsFirstNameClaim(claim.Type?.ToLowerInvariant() ?? string.Empty))
                {
                    return value;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Indicate if the claim type design the first name.
    /// </summary>
    /// <param name="claimType">Type.</param>
    /// <returns><c>true</c> if the claim type design the first name.</returns>
    private static bool IsFirstNameClaim(string claimType)
    {
        return claimType.EndsWith("givenname") || claimType.EndsWith("gn") || claimType.EndsWith("firstname");
    }

    #endregion

    #region GetLastName

    /// <summary>
    /// Get the user last name.
    /// </summary>
    /// <param name="claims">a list of claims.</param>
    /// <returns>The last name of the user.</returns>
    public static string GetLastName(this IEnumerable<Claim> claims)
    {
        string value;
        foreach (Claim claim in claims)
        {
            value = claim.Value;
            if (!string.IsNullOrEmpty(value))
            {
                if (IsLastNameClaim(claim.Type?.ToLowerInvariant() ?? string.Empty))
                {
                    return value;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Indicate if the claim type design the last name.
    /// </summary>
    /// <param name="claimType">Type.</param>
    /// <returns><c>true</c> if the claim type design the last name.</returns>
    private static bool IsLastNameClaim(string claimType)
    {
        return claimType.EndsWith("surname") || claimType.EndsWith("sn") || claimType.EndsWith("lastname");
    }

    #endregion

    #region GetDisplayName

    /// <summary>
    /// Get the user display name.
    /// </summary>
    /// <param name="claims">a list of claims.</param>
    /// <param name="lastNameFirst">Indicate if the last name must be before the first name (When the display name is not found or ignored).</param>
    /// <param name="ignoreDisplayName">Indicate to ignore the display name.</param>
    /// <returns>The display name of the user.</returns>
    public static string GetDisplayName(this IEnumerable<Claim> claims, bool lastNameFirst = false, bool ignoreDisplayName = false)
    {
        string firstName = null;
        string lastName = null;
        string name = null;
        string nameidentifier = null;
        string type;
        string value;
        foreach (Claim claim in claims)
        {
            value = claim.Value;
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }
            type = claim.Type?.ToLowerInvariant() ?? string.Empty;
            if (!ignoreDisplayName && type.EndsWith("displayname"))
            {
                return value;
            }
            else if (IsFirstNameClaim(type))
            {
                firstName = value;
            }
            else if (IsLastNameClaim(type))
            {
                lastName = value;
            }
            else if (type.EndsWith("name"))
            {
                name = value;
            }
            else if (type.EndsWith("nameidentifier"))
            {
                nameidentifier = value;
            }
        }
        if (firstName is null && lastName is null)
        {
            return name ?? nameidentifier;
        }
        return firstName is null
            ? lastName
            : lastName is null ? firstName : lastNameFirst ? lastName + " " + firstName : firstName + " " + lastName;
    }

    #endregion

    #region GetEmailAddress

    /// <summary>
    /// Get the user email address.
    /// </summary>
    /// <param name="claims">a list of claims.</param>
    /// <returns>The email address of the user.</returns>
    public static string GetEmailAddress(this IEnumerable<Claim> claims)
    {
        string name = null;
        string mail = null;
        string type;
        string value;
        foreach (Claim claim in claims)
        {
            value = claim.Value;
            if (!string.IsNullOrEmpty(value))
            {
                type = claim.Type?.ToLowerInvariant() ?? string.Empty;
                if (type.EndsWith("emailaddress"))
                {
                    return value;
                }
                else if (type == "name" || type.EndsWith("/name"))
                {
                    name = value;
                }
                else if (type.EndsWith("mail"))
                {
                    mail = value;
                }
            }
        }
        return mail ?? name;
    }

    #endregion

    #region ExtractInformations

    /// <summary>
    /// Extract the user informations from the claims.
    /// </summary>
    /// <param name="claims">The claims.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    public static void ExtractInformations(this IEnumerable<Claim> claims,
        out string firstName, out string lastName)
    {
        firstName = claims.GetFirstName();
        lastName = claims.GetLastName();
    }

    /// <summary>
    /// Extract the user informations from the claims.
    /// </summary>
    /// <param name="claims">The claims.</param>
    /// <param name="email">The email address.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    public static void ExtractInformations(this IEnumerable<Claim> claims, 
        out string firstName, out string lastName,
        out string email)
    {
        ExtractInformations(claims, out firstName, out lastName);
        email = claims.GetEmailAddress();
    }

    /// <summary>
    /// Extract the user informations from the claims.
    /// </summary>
    /// <param name="claims">The claims.</param>
    /// <param name="email">The email address.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="displayName">The display name.</param>
    public static void ExtractInformations(this IEnumerable<Claim> claims,
        out string firstName, out string lastName,
        out string email,
        out string displayName)
    {
        ExtractInformations(claims, out firstName, out lastName, out email);
        displayName = claims.GetDisplayName();
    }

    #endregion

}
