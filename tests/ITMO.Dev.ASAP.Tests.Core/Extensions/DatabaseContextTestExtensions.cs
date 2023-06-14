using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;

namespace ITMO.Dev.ASAP.Tests.Core.Extensions;

public static class DatabaseContextTestExtensions
{
    public static async Task<Submission> GetSubmissionAsync(
        this IPersistenceContext context,
        params SubmissionStateKind[] states)
    {
        var query = SubmissionQuery.Build(x => x
            .WithSubmissionStates(states)
            .WithSubjectCourseWorkflow(SubmissionStateWorkflowType.ReviewWithDefense)
            .WithLimit(1));

        return await context.Submissions.QueryAsync(query, default).FirstAsync();
    }
}