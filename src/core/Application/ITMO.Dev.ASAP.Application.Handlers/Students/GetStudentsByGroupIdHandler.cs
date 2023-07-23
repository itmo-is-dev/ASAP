using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Queries.GetStudentsByGroupId;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class GetStudentsByGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IGithubUserService _githubUserService;

    public GetStudentsByGroupIdHandler(IPersistenceContext context, IGithubUserService githubUserService)
    {
        _context = context;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = StudentQuery.Build(x => x.WithGroupId(request.GroupId));

        Student[] students = await _context.Students
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        IReadOnlyCollection<StudentDto> dto = await _githubUserService
            .MapToStudentDtosAsync(students, cancellationToken);

        return new Response(dto);
    }
}