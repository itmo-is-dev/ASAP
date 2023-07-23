using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Application.Factories;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.UpdateSubmissionPoints;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public UpdateSubmissionPointsHandler(
        IPermissionValidator permissionValidator,
        IPersistenceContext context,
        IPublisher publisher)
    {
        _permissionValidator = permissionValidator;
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions
            .GetSubmissionForCodeOrLatestAsync(request.UserId, request.AssignmentId, request.Code, cancellationToken);

        await _permissionValidator.EnsureSubmissionMentorAsync(
            request.IssuerId,
            submission.Id,
            cancellationToken);

        Fraction? rating = request.RatingPercent is null
            ? null
            : Fraction.FromDenormalizedValue(request.RatingPercent.Value);

        Points? extraPoints = request.ExtraPoints;

        submission.UpdatePoints(rating, extraPoints);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByAssignmentId(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

        Assignment assignment = await _context.Assignments
            .GetByIdAsync(submission.GroupAssignment.Id.AssignmentId, cancellationToken);

        GroupAssignment groupAssignment = await _context.GroupAssignments
            .GetByIdsAsync(submission.GroupAssignment.Id, cancellationToken);

        SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory
            .CreateFromSubmission(submission, subjectCourse, assignment, groupAssignment);

        Points points = submission.CalculateEffectivePoints(assignment, subjectCourse.DeadlinePolicy).Points;

        var notification = new SubmissionPointsUpdated.Notification(submission.ToDto(points));
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(submissionRateDto);
    }
}