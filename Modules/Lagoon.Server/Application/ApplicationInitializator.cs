using Microsoft.Extensions.Hosting;

namespace Lagoon.Server.Application;

internal class ApplicationInitializator : IHostedService
{

    #region fields

    /// <summary>
    /// The main application single instance.
    /// </summary>
    private ILgApplication _app;

    /// <summary>
    /// The service provider.
    /// </summary>
    private IServiceProvider _serviceProvider;

    #endregion

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="application">The main application single instance.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public ApplicationInitializator(ILgApplication application, IServiceProvider serviceProvider)
    {
        _app = application;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Method called when the application starts.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _app.StartAsync(_serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Method called when the application is stoping.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _app.StopAsync(cancellationToken);
    }

}
