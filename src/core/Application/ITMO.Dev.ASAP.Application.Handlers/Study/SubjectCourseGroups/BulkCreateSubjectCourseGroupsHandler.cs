using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands.BulkCreateSubjectCourseGroups;
using StudentGroup = ITMO.Dev.ASAP.Domain.Groups.StudentGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class BulkCreateSubjectCourseGroupsHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IPublisher _publisher;

    public BulkCreateSubjectCourseGroupsHandler(IPersistenceContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse course = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        IEnumerable<Guid> groupsToCreateIds = request.GroupIds.Except(course.Groups.Select(x => x.Id));

        var studentGroupsQuery = StudentGroupQuery.Build(x => x.WithIds(groupsToCreateIds));

        StudentGroup[] studentGroups = await _context.StudentGroups
            .QueryAsync(studentGroupsQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        (IReadOnlyCollection<SubjectCourseGroup> groups, ISubjectCourseEvent evt) = course.AddGroups(studentGroups);

        await _context.SubjectCourses.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourseGroupDto[] dtos = groups.Select(x => x.ToDto()).ToArray();

        IEnumerable<SubjectCourseGroupCreated.Notification> notifications = dtos
            .Select(g => new SubjectCourseGroupCreated.Notification(g));

        IEnumerable<Task> tasks = notifications.Select(x => _publisher.PublishAsync(x, cancellationToken));
        await Task.WhenAll(tasks);

        return new Response(dtos);
    }
}