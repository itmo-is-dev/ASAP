using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record StudentGroupQuery(
    Guid[] Ids,
    Guid[] StudentIds,
    string? NamePattern);