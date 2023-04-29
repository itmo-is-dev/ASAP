using ITMO.Dev.ASAP.Application.Dto.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;
using ITMO.Dev.ASAP.WebApi.Sdk.HubClients;
using ITMO.Dev.ASAP.WebApi.Sdk.HubClients.Implementations;
using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.WebApi.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAsapSdk(this IServiceCollection collection, Uri baseAddress)
    {
        void AddControllerClient<TClient, TImplementation>()
            where TClient : class
            where TImplementation : class, TClient
        {
            collection
                .AddHttpClient<TClient, TImplementation>(client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler(provider =>
                {
                    ITokenProvider identityProvider = provider.GetRequiredService<ITokenProvider>();
                    return new AuthorizationMessageHandlerDecorator(identityProvider);
                });
        }

        void AddHubClient<TClient, TImplementation>(string address)
            where TClient : IHubClient
            where TImplementation : IHubConnectionCreatable<TClient>
        {
            collection.AddScoped<IHubClientProvider<TClient>>(p =>
            {
                ITokenProvider tokenProvider = p.GetRequiredService<ITokenProvider>();

                return new HubClientProvider<TClient>(
                    new Uri(baseAddress, address),
                    connection => TImplementation.Create(p, connection),
                    tokenProvider);
            });
        }

        AddControllerClient<IAssignmentClient, AssignmentClient>();
        AddControllerClient<IGithubManagementClient, GithubManagementClient>();
        AddControllerClient<IGroupAssignmentClient, GroupAssignmentClient>();
        AddControllerClient<IIdentityClient, IdentityClient>();
        AddControllerClient<IStudentClient, StudentClient>();
        AddControllerClient<IStudyGroupClient, StudyGroupClient>();
        AddControllerClient<ISubjectClient, SubjectClient>();
        AddControllerClient<ISubjectCourseClient, SubjectCourseClient>();
        AddControllerClient<ISubjectCourseGroupClient, SubjectCourseGroupClient>();
        AddControllerClient<IUserClient, UserClient>();

        AddHubClient<IQueueHubClient, QueueHubClient>("hubs/queue");

        collection.AddDtoConfiguration();

        return collection;
    }
}