using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Extensions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.UpdateMentors;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class UpdateMentorsHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;

    public UpdateMentorsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.Mentors)
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        IEnumerable<Mentor> mentorsToRemove = subjectCourse.Mentors
            .Where(x => request.UserIds.Contains(x.UserId) is false);

        foreach (Mentor mentor in mentorsToRemove)
        {
            subjectCourse.RemoveMentor(mentor);
            _context.Mentors.Remove(mentor);
        }

        IEnumerable<Guid> mentorsToAddIds = request.UserIds.Except(subjectCourse.Mentors.Select(x => x.UserId));

        List<User> users = await _context.Users
            .Where(x => mentorsToAddIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        IEnumerable<Mentor> addedMentors = users
            .Select(x => subjectCourse.AddMentor(x));

        _context.Mentors.AddRange(addedMentors);

        await _context.SaveChangesAsync(cancellationToken);
    }
}