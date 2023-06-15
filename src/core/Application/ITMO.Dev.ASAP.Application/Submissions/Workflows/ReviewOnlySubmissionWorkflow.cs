using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Application.Factories;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Submissions.Workflows;

public class ReviewOnlySubmissionWorkflow : SubmissionWorkflowBase
{
    public ReviewOnlySubmissionWorkflow(
        IPersistenceContext context,
        IPermissionValidator permissionValidator,
        IPublisher publisher) : base(permissionValidator, context, publisher) { }

    public override async Task<SubmissionActionMessageDto> SubmissionApprovedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await PermissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId,
            cancellationToken,
            static x =>
            {
                if (x.Rating is null)
                    x.Rate(Fraction.FromDenormalizedValue(100), 0);
            });

        SubjectCourse subjectCourse = await Context.SubjectCourses
            .GetByAssignmentId(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

        Assignment assignment = await Context.Assignments
            .GetByIdAsync(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

        GroupAssignment groupAssignment = await Context.GroupAssignments
            .GetByIdsAsync(submission.GroupAssignment.Id, cancellationToken);

        SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory
            .CreateFromSubmission(submission, subjectCourse, assignment, groupAssignment);

        string message = UserCommandProcessingMessage.ReviewRatedSubmission(submissionRateDto.TotalPoints ?? 0);

        return new SubmissionActionMessageDto(message);
    }
}