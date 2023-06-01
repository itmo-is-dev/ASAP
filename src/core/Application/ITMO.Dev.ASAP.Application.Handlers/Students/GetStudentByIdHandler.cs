using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Queries.GetStudentById;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class GetStudentByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubUserService _githubUserService;

    public GetStudentByIdHandler(IDatabaseContext context, IGithubUserService githubUserService)
    {
        _context = context;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Student? student = await _context.Students
            .Where(s => s.User.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == default)
            throw new EntityNotFoundException($"Student not found for user {request.UserId}");

        GithubUserDto? username = await _githubUserService.FindByIdAsync(student.UserId, default);
        StudentDto dto = student.ToDto(username?.Username);

        return new Response(dto);
    }
}