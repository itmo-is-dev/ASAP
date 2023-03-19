using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Application.Specifications;
using ITMO.Dev.ASAP.Github.Common.Exceptions;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Users.Commands.UpdateGithubUser;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Users;

internal class UpdateGithubUserHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IAsapUserService _asapUserService;
    private readonly IGithubUserService _githubUserService;

    public UpdateGithubUserHandler(
        IDatabaseContext context,
        IAsapUserService asapUserService,
        IGithubUserService githubUserService)
    {
        _context = context;
        _asapUserService = asapUserService;
        _githubUserService = githubUserService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        bool alreadyExists = await _context.Users.ForUsername(request.GithubUsername).AnyAsync(cancellationToken);

        if (alreadyExists)
            throw GithubUserException.UsernameCollision(request.GithubUsername).TaggedWithConflict();

        bool userExists = await _githubUserService.IsUserExistsAsync(request.GithubUsername, cancellationToken);

        if (userExists is false)
            throw GithubUserException.UserDoesNotExist(request.GithubUsername).TaggedWithNotFound();

        GithubUser? githubUser = await _context.Users
            .Where(x => x.Id.Equals(request.UserId))
            .SingleOrDefaultAsync(cancellationToken);

        if (githubUser is not null)
        {
            githubUser.Username = request.GithubUsername.ToLower();
            _context.Users.Update(githubUser);
        }
        else
        {
            UserDto user = await _asapUserService.GetByIdAsync(request.UserId, cancellationToken);
            githubUser = new GithubUser(user.Id, request.GithubUsername);

            _context.Users.Add(githubUser);
        }

        await _context.SaveChangesAsync(cancellationToken);

        GithubUserDto dto = githubUser.ToDto();

        return new Response(dto);
    }
}