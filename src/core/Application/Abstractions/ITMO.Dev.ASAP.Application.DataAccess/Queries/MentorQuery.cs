using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record MentorQuery(Guid[] UserIds, Guid[] SubjectCourseIds);