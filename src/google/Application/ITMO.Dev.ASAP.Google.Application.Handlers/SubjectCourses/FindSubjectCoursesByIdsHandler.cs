using ITMO.Dev.ASAP.Google.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Google.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Application.Mapping;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;
using MediatR;
using static ITMO.Dev.ASAP.Google.Application.Contracts.SubjectCourses.FindSubjectCoursesById;

namespace ITMO.Dev.ASAP.Google.Application.Handlers.SubjectCourses;

internal class FindSubjectCoursesByIdsHandler : IRequestHandler<Query, Response>
{
    private readonly ISubjectCourseRepository _subjectCourseRepository;

    public FindSubjectCoursesByIdsHandler(ISubjectCourseRepository subjectCourseRepository)
    {
        _subjectCourseRepository = subjectCourseRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = SubjectCourseQuery.Build(x => x.WithIds(request.Ids));
        IEnumerable<GoogleSubjectCourse> courses = await _subjectCourseRepository.QueryAsync(query, cancellationToken);

        IEnumerable<GoogleSubjectCourseDto> dto = courses.Select(x => x.ToDto());

        return new Response(dto);
    }
}