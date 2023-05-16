using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record GithubAssignmentQuery(
    IReadOnlyCollection<Guid> Ids,
    IReadOnlyCollection<Guid> SubjectCourseIds,
    IReadOnlyCollection<string> BranchNames,
    IReadOnlyCollection<string> SubjectCourseOrganizationNames);