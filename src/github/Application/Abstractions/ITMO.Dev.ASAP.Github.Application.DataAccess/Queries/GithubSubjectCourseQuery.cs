using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record GithubSubjectCourseQuery(
    IReadOnlyCollection<Guid> Ids,
    IReadOnlyCollection<string> OrganizationNames,
    IReadOnlyCollection<string> TemplateRepositoryNames,
    IReadOnlyCollection<string> MentorTeamNames,
    int? Limit);