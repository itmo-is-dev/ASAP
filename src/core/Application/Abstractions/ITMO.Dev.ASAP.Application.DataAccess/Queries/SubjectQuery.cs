using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record SubjectQuery(
    IReadOnlyCollection<Guid> Ids,
    IReadOnlyCollection<string> Names,
    IReadOnlyCollection<Guid> MentorIds);