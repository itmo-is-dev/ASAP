using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record AssignmentQuery(IReadOnlyCollection<Guid> Ids, IReadOnlyCollection<Guid> SubjectCourseIds);