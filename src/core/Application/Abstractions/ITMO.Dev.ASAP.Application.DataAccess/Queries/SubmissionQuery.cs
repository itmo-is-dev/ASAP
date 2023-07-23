using ITMO.Dev.ASAP.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using SourceKit.Generators.Builder.Annotations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Queries;

[GenerateBuilder]
public partial record SubmissionQuery(
    Guid[] Ids,
    int[] Codes,
    Guid[] UserIds,
    Guid[] SubjectCourseIds,
    Guid[] AssignmentIds,
    Guid[] StudentGroupIds,
    SubmissionStateKind[] SubmissionStates,
    SubmissionStateWorkflowType[] SubjectCourseWorkflows,
    OrderDirection? OrderByCode,
    int? Limit);