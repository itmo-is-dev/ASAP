using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record GithubUserQuery(IReadOnlyCollection<Guid> Ids, IReadOnlyCollection<string> Usernames, int? Limit);