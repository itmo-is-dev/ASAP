using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Queries.GetStudentsBySubjectCourseId;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class GetStudentsBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubUserService _githubUserService;

    public GetStudentsBySubjectCourseIdHandler(IDatabaseContext context, IGithubUserService githubUserService)
    {
        _context = context;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Student> students = await _context
            .SubjectCourses
            .Where(sc => sc.Id == request.SubjectCourseId)
            .SelectMany(sc => sc.Groups)
            .SelectMany(sg => sg.StudentGroup.Students)
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<StudentDto> dto = await _githubUserService
            .MapToStudentDtosAsync(students, cancellationToken);

        return new Response(dto);
    }
}