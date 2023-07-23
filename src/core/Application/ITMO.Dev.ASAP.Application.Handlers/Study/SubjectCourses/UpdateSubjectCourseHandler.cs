using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.UpdateSubjectCourse;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class UpdateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;

    public UpdateSubjectCourseHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.Id, cancellationToken);
        TitleUpdatedEvent evt = subjectCourse.UpdateTitle(request.NewTitle);

        await _context.SubjectCourses.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subjectCourse.ToDto());
    }
}