using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record StudentQuery(
    Guid[] Ids,
    Guid[] GroupIds,
    Guid[] AssignmentIds,
    Guid[] SubjectCourseIds,
    string? GroupNamePattern,
    string? UniversityIdPattern,
    string? FullNamePattern);