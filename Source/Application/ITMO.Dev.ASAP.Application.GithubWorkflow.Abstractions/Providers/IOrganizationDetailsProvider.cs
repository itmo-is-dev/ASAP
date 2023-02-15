namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;

public interface IOrganizationDetailsProvider
{
    Task<IReadOnlyCollection<string>> GetOrganizationOwners(string organizationName);

    Task<IReadOnlyCollection<string>> GetOrganizationTeamMembers(string organizationName, string teamName);
}