using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindStudentsByQuery;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class FindStudentsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IEntityQuery<Student, StudentQueryParameter> _query;
    private readonly IEntityFilter<StudentDto, StudentQueryParameter> _filter;
    private readonly IGithubUserService _githubUserService;

    public FindStudentsByQueryHandler(
        IEntityQuery<Student, StudentQueryParameter> query,
        IDatabaseContext context,
        IEntityFilter<StudentDto, StudentQueryParameter> filter,
        IGithubUserService githubUserService)
    {
        _query = query;
        _context = context;
        _filter = filter;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<Student> query = _context.Students;
        query = _query.Apply(query, request.Configuration);

        List<Student> students = await query.ToListAsync(cancellationToken);

        IReadOnlyCollection<StudentDto> dto = await _githubUserService
            .MapToStudentDtosAsync(students, cancellationToken);

        dto = _filter.Apply(dto, request.Configuration).ToArray();

        return new Response(dto);
    }
}