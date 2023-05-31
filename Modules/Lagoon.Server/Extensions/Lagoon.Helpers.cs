using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lagoon.Helpers;

/// <summary>
/// Extension methods class.
/// </summary>
public static class Extensions
{

    #region ModelStateDictionary extension

    /// <summary>
    /// Add the errorsList to the model errors
    /// </summary>
    /// <param name="model">Model where append the errors</param>
    /// <param name="errorsList">Errors to append</param>
    public static void AddModelErrorList(this ModelStateDictionary model, Dictionary<string, List<string>> errorsList)
    {
        foreach (string key in errorsList.Keys)
        {
            foreach (string msg in errorsList[key])
            {
                model.AddModelError(key, msg);
            }
        }
    }

    #endregion

    #region IdentityResult extension

    /// <summary>
    /// Check if result is Succeeded. if not throw an exception message with details
    /// </summary>
    /// <param name="result">IdentityResult to validate</param>
    /// <param name="exceptionMessage">Exception message header if result not Succeeded</param>
    public static void EnsureSuccess(this IdentityResult result, string exceptionMessage)
    {
        if (!result.Succeeded)
        {
            StringBuilder stb = new();
            stb.AppendLine(exceptionMessage);
            foreach (IdentityError error in result.Errors)
            {
                stb.AppendLine($"[{error.Code}] {error.Description}");
            }

            throw new Exception(stb.ToString());
        }
    }

    #endregion

}
