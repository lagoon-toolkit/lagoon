using Lagoon.Core.Application;
using Lagoon.Server.Helpers;

namespace Lagoon.Server.Application;

internal class ApplicationInformation : Lagoon.Core.Application.ApplicationInformation
{
    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="environmentName">The environment name.</param>
    public ApplicationInformation(LgApplicationBase app, string environmentName)
        : base(app, app.Configuration, environmentName)
    {
    }

    ///<inheritdoc/>
    protected override string DetectPublicUrl()
    {
        string publicURL = base.DetectPublicUrl();
        if (string.IsNullOrEmpty(publicURL))
        {
            // Whe search for the HTTPS public URL
            publicURL = LaunchSettings.GetIISHttpAddress(true);
        }
        return publicURL;
    }
}
