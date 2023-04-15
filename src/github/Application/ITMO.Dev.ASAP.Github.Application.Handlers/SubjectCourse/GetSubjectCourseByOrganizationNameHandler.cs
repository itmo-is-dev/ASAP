using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Github.Application.Contracts.SubjectCourses.Queries.GetSubjectCourseByOrganizationName;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.SubjectCourse;

internal class GetSubjectCourseByOrganizationNameHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectCourseByOrganizationNameHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        GithubSubjectCourse? subjectCourse = await _context.SubjectCourses
            .ForOrganizationName(request.OrganizationName)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw EntityNotFoundException.SubjectCourse(request.OrganizationName).TaggedWithNotFound();

        GithubSubjectCourseDto dto = subjectCourse.ToDto();

        return new Response(dto);
    }
}