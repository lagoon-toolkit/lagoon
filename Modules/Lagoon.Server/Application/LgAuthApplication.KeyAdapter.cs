using Lagoon.Server.Services;
using Lagoon.Server.Services.LagoonSettings;
using OpenIddict.EntityFrameworkCore.Models;

namespace Lagoon.Server.Application;

internal interface IUserKeyAdapter
{
    void ConfigureIdentity(IdentityBuilder identityBuilder);
    void ConfigureOpenIddictEFCore(OpenIddictEntityFrameworkCoreBuilder openIddictOptions);
    Type GetSettingsManagerType();
    Type GetLgOpenIddictManagerType();

}

public abstract partial class LgAuthApplication<TDbContext, TUser>
{

    private class KeyAdapter<TKey> : IUserKeyAdapter
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Indicate if the primary key is a string.
        /// </summary>
        private bool _hasStringKey = typeof(TKey) == typeof(string);

        /// <summary>
        /// Adds roles related services for TRole, including IRoleStore, IRoleValidator, and RoleManager.
        /// </summary>
        /// <param name="identityBuilder">Options to configure identity services.</param>
        public void ConfigureIdentity(IdentityBuilder identityBuilder)
        {
            if (_hasStringKey)
            {
                identityBuilder.AddRoles<IdentityRole>();
            }
            else
            {
                identityBuilder.AddRoles<IdentityRole<TKey>>();
            }
        }

        /// <summary>
        /// Configure OpenIddict to use the default entities with a custom key type if TKey is not string.
        /// </summary>
        /// <param name="options">Options to configure the OpenIddict Entity Framework Core services.</param>
        public void ConfigureOpenIddictEFCore(OpenIddictEntityFrameworkCoreBuilder options)
        {
            options.ApplyKeyType<TKey>();
        }

        /// <summary>
        /// Gets the Lagoon settings manager type.
        /// </summary>
        /// <returns>The Lagoon settings manager type.</returns>
        public Type GetSettingsManagerType()
        {
            return _hasStringKey ? typeof(LagoonSettingsManager<TUser>) : typeof(LagoonSettingsManagerGuid<TUser>);
        }

        /// <summary>
        /// Get the Lagoon OpenIddict manager.
        /// </summary>
        /// <returns></returns>
        public Type GetLgOpenIddictManagerType()
        {
            return _hasStringKey
                ? typeof(LgOpenIddictManager<,,,>).MakeGenericType(
                    typeof(OpenIddictEntityFrameworkCoreApplication),
                    typeof(string),
                    typeof(OpenIddictEntityFrameworkCoreAuthorization),
                    typeof(OpenIddictEntityFrameworkCoreToken))
                : typeof(LgOpenIddictManager<,,,>).MakeGenericType(
                    typeof(OpenIddictEntityFrameworkCoreApplication<TKey>),
                    typeof(TKey),
                    typeof(OpenIddictEntityFrameworkCoreAuthorization<TKey>),
                    typeof(OpenIddictEntityFrameworkCoreToken<TKey>));
        }

    }

}
