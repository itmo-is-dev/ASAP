using ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Notifications;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Commands.UpdateSubjectCourseMentorTeam;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class UpdateSubjectCourseMentorTeamHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;
    private readonly IPublisher _publisher;

    public UpdateSubjectCourseMentorTeamHandler(IPersistenceContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        subjectCourse.MentorTeamName = request.MentorsTeamName;

        _context.SubjectCourses.Update(subjectCourse);
        await _context.CommitAsync(cancellationToken);

        var notification = new SubjectCourseMentorTeamUpdated.Notification(subjectCourse.Id);
        await _publisher.Publish(notification, cancellationToken);
    }
}