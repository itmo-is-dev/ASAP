using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Queries.GetSubjectCourseGroupsBySubjectCourseId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class GetSubjectCourseGroupsBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetSubjectCourseGroupsBySubjectCourseIdHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        SubjectCourseGroupDto[] subjectCourseGroups = subjectCourse.Groups
            .Select(group => new SubjectCourseGroupDto(subjectCourse.Id, group.Id, group.Name))
            .OrderBy(x => x.StudentGroupName)
            .ToArray();

        return new Response(subjectCourseGroups);
    }
}