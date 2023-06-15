using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands.DeleteSubjectCourseGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class DeleteSubjectCourseGroupHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;

    public DeleteSubjectCourseGroupHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        ISubjectCourseEvent evt = subjectCourse.RemoveGroup(request.StudentGroupId);

        await _context.SubjectCourses.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}