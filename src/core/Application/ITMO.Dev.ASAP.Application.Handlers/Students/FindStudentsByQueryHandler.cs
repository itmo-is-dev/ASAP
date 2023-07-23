using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindStudentsByQuery;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class FindStudentsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IEntityQuery<StudentQuery.Builder, StudentQueryParameter> _query;
    private readonly IEntityFilter<StudentDto, StudentQueryParameter> _filter;
    private readonly IGithubUserService _githubUserService;

    public FindStudentsByQueryHandler(
        IEntityQuery<StudentQuery.Builder, StudentQueryParameter> query,
        IPersistenceContext context,
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
        var queryBuilder = new StudentQuery.Builder();
        queryBuilder = _query.Apply(queryBuilder, request.Configuration);

        Student[] students = await _context.Students
            .QueryAsync(queryBuilder.Build(), cancellationToken)
            .ToArrayAsync(cancellationToken);

        IReadOnlyCollection<StudentDto> dto = await _githubUserService
            .MapToStudentDtosAsync(students, cancellationToken);

        dto = _filter.Apply(dto, request.Configuration).ToArray();

        return new Response(dto);
    }
}