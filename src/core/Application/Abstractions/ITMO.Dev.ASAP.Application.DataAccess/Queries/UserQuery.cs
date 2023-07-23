using ITMO.Dev.ASAP.Application.DataAccess.Models;
using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record UserQuery(
    Guid[] Ids,
    string? UniversityIdPattern,
    string? FullNamePattern,
    int[] UniversityIds,
    OrderDirection? OrderByLastName,
    int? Cursor,
    int? Limit);