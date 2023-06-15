using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record SubjectQuery(
    Guid[] Ids,
    string[] Names,
    Guid[] MentorIds);