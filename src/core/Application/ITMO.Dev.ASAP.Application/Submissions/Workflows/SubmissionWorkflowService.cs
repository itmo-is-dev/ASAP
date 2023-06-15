using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.SubmissionStateWorkflows;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Submissions.Workflows;

public class SubmissionWorkflowService : ISubmissionWorkflowService
{
    private readonly IPersistenceContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public SubmissionWorkflowService(
        IPersistenceContext context,
        IPermissionValidator permissionValidator,
        IPublisher publisher)
    {
        _context = context;
        _permissionValidator = permissionValidator;
        _publisher = publisher;
    }

    public async Task<ISubmissionWorkflow> GetSubmissionWorkflowAsync(
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetBySubmissionIdAsync(submissionId, cancellationToken);

        return GetSubmissionWorkflow(subjectCourse);
    }

    public async Task<ISubmissionWorkflow> GetAssignmentWorkflowAsync(
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByAssignmentId(assignmentId, cancellationToken);
        return GetSubmissionWorkflow(subjectCourse);
    }

    private ISubmissionWorkflow GetSubmissionWorkflow(SubjectCourse subjectCourse)
    {
        return subjectCourse.WorkflowType switch
        {
            null or SubmissionStateWorkflowType.ReviewOnly
                => new ReviewOnlySubmissionWorkflow(_context, _permissionValidator, _publisher),

            SubmissionStateWorkflowType.ReviewWithDefense
                => new ReviewWithDefenceSubmissionWorkflow(_permissionValidator, _context, _publisher),

            _ => throw new ArgumentOutOfRangeException(
                nameof(subjectCourse),
                $@"Invalid WorkflowType {subjectCourse.WorkflowType:G}"),
        };
    }
}