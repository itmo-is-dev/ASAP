using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.CreateStudent;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class CreateStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubUserService _githubUserService;

    public CreateStudentHandler(IDatabaseContext context, IGithubUserService githubUserService)
    {
        _context = context;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup? group = await _context.StudentGroups
            .Include(x => x.Students)
            .SingleOrDefaultAsync(x => x.Id.Equals(request.GroupId), cancellationToken);

        if (group is null)
            throw EntityNotFoundException.For<StudentGroup>(request.GroupId);

        var user = new User(Guid.NewGuid(), request.FirstName, request.MiddleName, request.LastName);
        var student = new Student(user, group);

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        StudentDto dto = await _githubUserService.MapToStudentDtoAsync(student, default);

        return new Response(dto);
    }
}