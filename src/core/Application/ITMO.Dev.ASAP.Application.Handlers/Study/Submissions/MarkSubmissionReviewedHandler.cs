using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.MarkSubmissionReviewed;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class MarkSubmissionReviewedHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public MarkSubmissionReviewedHandler(
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
        await _permissionValidator.EnsureSubmissionMentorAsync(
            request.IssuerId,
            request.SubmissionId,
            cancellationToken);

        Submission submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);
        submission.MarkAsReviewed();

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        Assignment assignment = await _context.Assignments
            .GetByIdAsync(submission.GroupAssignment.Assignment.Id, cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByAssignmentId(assignment.Id, cancellationToken);

        Points points = submission.CalculateEffectivePoints(assignment, subjectCourse.DeadlinePolicy).Points;

        SubmissionDto dto = submission.ToDto(points);

        var notification = new SubmissionStateUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}