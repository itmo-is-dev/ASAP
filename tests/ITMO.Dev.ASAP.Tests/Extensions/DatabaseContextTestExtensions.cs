using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Submissions.States;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class DatabaseContextTestExtensions
{
    public static Task<Submission> GetSubmissionAsync(
        this IDatabaseContext context,
        params ISubmissionState[] states)
    {
        return context.Submissions
            .Where(submission => states.Any(x => x.Equals(submission.State)))
            .Where(x =>
                x.GroupAssignment.Assignment.SubjectCourse.WorkflowType ==
                SubmissionStateWorkflowType.ReviewWithDefense)
            .FirstAsync();
    }
}