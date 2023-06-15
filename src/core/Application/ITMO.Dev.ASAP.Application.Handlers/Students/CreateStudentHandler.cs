using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.CreateStudent;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class CreateStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IPersistenceContext _context;
    private readonly IGithubUserService _githubUserService;

    public CreateStudentHandler(IPersistenceContext context, IGithubUserService githubUserService)
    {
        _context = context;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups
            .GetByIdAsync(request.GroupId, cancellationToken);

        var user = new User(Guid.NewGuid(), request.FirstName, request.MiddleName, request.LastName);
        var student = new Student(user, group.Info);

        _context.Users.Add(user);
        _context.Students.Add(student);

        await _context.SaveChangesAsync(cancellationToken);

        StudentDto dto = await _githubUserService.MapToStudentDtoAsync(student, default);

        return new Response(dto);
    }
}