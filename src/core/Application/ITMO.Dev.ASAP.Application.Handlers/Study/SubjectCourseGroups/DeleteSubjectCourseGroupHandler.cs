using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Extensions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands.DeleteSubjectCourseGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class DeleteSubjectCourseGroupHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;

    public DeleteSubjectCourseGroupHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse =
            await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        SubjectCourseGroup? subjectCourseGroup =
            subjectCourse.Groups.FirstOrDefault(g => g.StudentGroupId == request.StudentGroupId);

        if (subjectCourseGroup is null)
            throw new EntityNotFoundException($"SubjectCourseGroup with id {request.StudentGroupId} not found");

        subjectCourse.RemoveGroup(subjectCourseGroup);

        _context.SubjectCourseGroups.Update(subjectCourseGroup);
        await _context.SaveChangesAsync(cancellationToken);
    }
}