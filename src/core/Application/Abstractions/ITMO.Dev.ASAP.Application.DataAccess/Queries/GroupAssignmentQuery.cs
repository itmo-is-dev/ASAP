using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record GroupAssignmentQuery(
    IReadOnlyCollection<Guid> GroupIds,
    IReadOnlyCollection<Guid> AssignmentIds,
    IReadOnlyCollection<GroupAssignmentId> GroupAssignmentIds,
    IReadOnlyCollection<Guid> SubjectCourseIds);