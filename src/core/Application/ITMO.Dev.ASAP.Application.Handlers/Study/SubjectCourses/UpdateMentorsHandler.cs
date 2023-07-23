using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using ITMO.Dev.ASAP.Domain.Users;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.UpdateMentors;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class UpdateMentorsHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;

    public UpdateMentorsHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var userQuery = UserQuery.Build(x => x.WithIds(request.UserIds));

        User[] users = await _context.Users
            .QueryAsync(userQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        ISubjectCourseEvent evt = subjectCourse.UpdateMentors(users);

        await _context.SubjectCourses.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}