using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Application.Factories;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePolicies;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Tools;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Submissions.Workflows;

#pragma warning disable CA1506
public abstract class SubmissionWorkflowBase : ISubmissionWorkflow
{
    private readonly IPublisher _publisher;

    protected SubmissionWorkflowBase(
        IPermissionValidator permissionValidator,
        IPersistenceContext context,
        IPublisher publisher)
    {
        PermissionValidator = permissionValidator;
        Context = context;
        _publisher = publisher;
    }

    protected IPermissionValidator PermissionValidator { get; }

    protected IPersistenceContext Context { get; }

    public abstract Task<SubmissionActionMessageDto> SubmissionApprovedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken);

    public async Task<SubmissionActionMessageDto> SubmissionNotAcceptedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await PermissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId,
            cancellationToken,
            static x => x.Rate(0, 0));

        SubjectCourse subjectCourse = await Context.SubjectCourses
            .GetByAssignmentId(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

        Assignment assignment = await Context.Assignments
            .GetByIdAsync(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

        GroupAssignment groupAssignment = await Context.GroupAssignments
            .GetByIdsAsync(submission.GroupAssignment.Id, cancellationToken);

        SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory
            .CreateFromSubmission(submission, subjectCourse, assignment, groupAssignment);

        string message = UserCommandProcessingMessage.SubmissionMarkAsNotAccepted(submission.Code);

        message = $"{message}\n{submissionRateDto.ToDisplayString()}";

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionReactivatedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x => x.Activate());

        string message = UserCommandProcessingMessage.SubmissionActivatedSuccessfully();
        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAcceptedAsync(
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

        string message = UserCommandProcessingMessage.SubmissionRated(submissionRateDto.ToDisplayString());

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionRejectedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await PermissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId,
            cancellationToken,
            static x => x.Deactivate());

        string message = UserCommandProcessingMessage.ClosePullRequestWithUnratedSubmission(submission.Code);

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAbandonedAsync(
        Guid issuerId,
        Guid submissionId,
        bool isTerminal,
        CancellationToken cancellationToken)
    {
        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId,
            cancellationToken,
            x =>
            {
                if (isTerminal)
                    x.Complete();
                else
                    x.Deactivate();
            });

        string message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionUpdateResult> SubmissionUpdatedAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        string payload,
        CancellationToken cancellationToken)
    {
        SubmissionStateKind[] acceptedStates =
        {
            SubmissionStateKind.Active,
            SubmissionStateKind.Reviewed,
        };

        var submissionsQuery = SubmissionQuery.Build(x => x
            .WithUserId(userId)
            .WithAssignmentId(assignmentId)
            .WithSubmissionStates(acceptedStates)
            .WithOrderByCode(OrderDirection.Descending)
            .WithLimit(1));

        Submission? submission = await Context.Submissions
            .QueryAsync(submissionsQuery, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        SubjectCourse subjectCourse = await Context.SubjectCourses.GetByAssignmentId(assignmentId, cancellationToken);

        bool triggeredByMentor = subjectCourse.Mentors.Any(x => x.UserId.Equals(issuerId));
        bool triggeredByAnotherUser = issuerId.Equals(userId) is false;

        if (submission is null || submission.IsRated)
        {
            if (triggeredByAnotherUser && triggeredByMentor is false)
            {
                string message = $"User {issuerId} is not allowed to create new submission for user {userId}";
                throw new UnauthorizedException(message);
            }

            var submissionCodeQuery = SubmissionQuery.Build(x => x
                .WithUserId(userId)
                .WithAssignmentId(assignmentId));

            int code = await Context.Submissions.CountAsync(submissionCodeQuery, cancellationToken);

            Student student = await Context.Students.GetByIdAsync(userId, cancellationToken);

            if (student.Group is null)
                throw new EntityNotFoundException("Assignment not found");

            SubjectCourseAssignment? subjectCourseAssignment = subjectCourse.Assignments
                .Where(x => x.AssignmentId.Equals(assignmentId))
                .SingleOrDefault(x => x.Groups.Any(g => g.Id.Equals(student.Group.Id)));

            if (subjectCourseAssignment is null)
                throw EntityNotFoundException.For<Assignment>(assignmentId);

            var groupAssignmentId = new GroupAssignmentId(student.Group.Id, subjectCourseAssignment.AssignmentId);

            GroupAssignment groupAssignment = await Context.GroupAssignments
                .GetByIdsAsync(groupAssignmentId, cancellationToken);

            submission = new Submission(
                Guid.NewGuid(),
                code + 1,
                student,
                Calendar.CurrentDateTime,
                payload,
                groupAssignment);

            Context.Submissions.Add(submission);
            await Context.SaveChangesAsync(cancellationToken);

            Assignment assignment = await Context.Assignments
                .GetByIdAsync(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

            SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory
                .CreateFromSubmission(submission, subjectCourse, assignment, groupAssignment);

            return new SubmissionUpdateResult(submissionRateDto, true);
        }

        if (triggeredByMentor is false)
        {
            submission.UpdateDate(Calendar.CurrentDateTime);

            Context.Submissions.Update(submission);
            await Context.SaveChangesAsync(cancellationToken);

            Assignment assignment = await Context.Assignments
                .GetByIdAsync(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

            await NotifySubmissionUpdated(submission, assignment, subjectCourse.DeadlinePolicy, cancellationToken);

            if (triggeredByAnotherUser)
                throw new UnauthorizedException("Submission updated by another user");

            GroupAssignment groupAssignment = await Context.GroupAssignments
                .GetByIdsAsync(submission.GroupAssignment.Id, cancellationToken);

            SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory
                .CreateFromSubmission(submission, subjectCourse, assignment, groupAssignment);

            return new SubmissionUpdateResult(submissionRateDto, false);
        }

        // TODO: Proper mentor update handling
        {
            Assignment assignment = await Context.Assignments
                .GetByIdAsync(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

            GroupAssignment groupAssignment = await Context.GroupAssignments
                .GetByIdsAsync(submission.GroupAssignment.Id, cancellationToken);

            SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory
                .CreateFromSubmission(submission, subjectCourse, assignment, groupAssignment);

            return new SubmissionUpdateResult(submissionRateDto, false);
        }
    }

    protected async Task<Submission> ExecuteSubmissionCommandAsync(
        Guid submissionId,
        CancellationToken cancellationToken,
        Action<Submission> action)
    {
        Submission submission = await Context.Submissions.GetByIdAsync(submissionId, cancellationToken);
        action(submission);

        Context.Submissions.Update(submission);
        await Context.SaveChangesAsync(cancellationToken);

        Assignment assignment = await Context.Assignments
            .GetByIdAsync(submission.GroupAssignment.Assignment.Id, cancellationToken);

        SubjectCourse subjectCourse = await Context.SubjectCourses
            .GetByAssignmentId(submission.GroupAssignment.Assignment.Id, cancellationToken);

        await NotifySubmissionUpdated(submission, assignment, subjectCourse.DeadlinePolicy, cancellationToken);

        return submission;
    }

    protected async Task NotifySubmissionUpdated(
        Submission submission,
        Assignment assignment,
        DeadlinePolicy deadlinePolicy,
        CancellationToken cancellationToken)
    {
        Points points = submission.CalculateEffectivePoints(assignment, deadlinePolicy).Points;
        var notification = new SubmissionUpdated.Notification(submission.ToDto(points));

        await _publisher.Publish(notification, cancellationToken);
    }
}