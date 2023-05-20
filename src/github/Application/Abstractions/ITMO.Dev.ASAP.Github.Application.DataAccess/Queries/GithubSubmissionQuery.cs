using ITMO.Dev.ASAP.Github.Application.DataAccess.Models;
using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record GithubSubmissionQuery(
    IReadOnlyCollection<Guid> Ids,
    IReadOnlyCollection<string> RepositoryNames,
    IReadOnlyCollection<long> PullRequestNumbers,
    IReadOnlyCollection<string> OrganizationNames,
    IReadOnlyCollection<string> AssignmentBranchNames,
    OrderDirection? OrderByCreatedAt)
{
    public bool HasOrderParameters => OrderByCreatedAt is not null;
}