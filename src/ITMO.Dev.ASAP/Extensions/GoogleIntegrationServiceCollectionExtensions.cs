using Google.Apis.Auth.OAuth2;
using ITMO.Dev.ASAP.Application.Google.Extensions;
using ITMO.Dev.ASAP.Configuration;

namespace ITMO.Dev.ASAP.Extensions;

internal static class GoogleIntegrationServiceCollectionExtensions
{
    internal static IServiceCollection AddGoogleIntegrationServices(
        this IServiceCollection serviceCollection,
        WebApiConfiguration webApiConfiguration)
    {
        if (webApiConfiguration.GoogleIntegrationConfiguration.EnableGoogleIntegration is false)
            return serviceCollection.AddDummyGoogleIntegration();

        return serviceCollection
            .AddGoogleIntegration(o => o
                .ConfigureGoogleCredentials(
                    GoogleCredential.FromJson(webApiConfiguration.GoogleIntegrationConfiguration.ClientSecrets))
                .ConfigureDriveId(webApiConfiguration.GoogleIntegrationConfiguration.GoogleDriveId));
    }
}