using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Application.Factories;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.UpdateSubmissionPoints;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public UpdateSubmissionPointsHandler(
        IPermissionValidator permissionValidator,
        IDatabaseContext context,
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

        Fraction? points = request.RatingPercent is null
            ? null
            : Fraction.FromDenormalizedValue(request.RatingPercent.Value);

        Points? extraPoints = request.ExtraPoints;

        submission.UpdatePoints(points, extraPoints);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

        var notification = new SubmissionPointsUpdated.Notification(submission.ToDto());
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}