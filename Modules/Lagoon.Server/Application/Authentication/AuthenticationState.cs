namespace Lagoon.Server.Application.Authentication;

/// <summary>
/// Authentication result for OnSignIn Workflow
/// </summary>
public enum LgAuthenticationState
{
    /// <summary>
    /// Authentication workflow as successfully complete
    /// </summary>
    Success = 1,

    /// <summary>
    /// User is authenticated but must enable an MFA before complete login
    /// </summary>
    RequireMfaActivation = 2,

    /// <summary>
    /// User is authenticated and have a MFA configured but require a valid MFA code to complete login
    /// </summary>
    RequireMfaValidation = 3
}
