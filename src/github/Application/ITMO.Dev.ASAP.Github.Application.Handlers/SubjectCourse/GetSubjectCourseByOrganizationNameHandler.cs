using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries.GetSubjectCourseByOrganizationName;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class GetSubjectCourseByOrganizationNameHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;

    public GetSubjectCourseByOrganizationNameHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = GithubSubjectCourseQuery.Build(x => x
            .WithOrganizationName(request.OrganizationName)
            .WithLimit(2));

        GithubSubjectCourse? subjectCourse = await _context.SubjectCourses
            .QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw EntityNotFoundException.SubjectCourse(request.OrganizationName).TaggedWithNotFound();

        GithubSubjectCourseDto dto = subjectCourse.ToDto();

        return new Response(dto);
    }
}