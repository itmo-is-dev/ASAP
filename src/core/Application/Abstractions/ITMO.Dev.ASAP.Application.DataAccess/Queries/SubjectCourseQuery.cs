using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record SubjectCourseQuery(
    Guid[] Ids,
    Guid[] SubjectIds,
    Guid[] AssignmentIds,
    Guid[] StudentGroupIds,
    Guid[] SubmissionIds);