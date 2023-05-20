using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.DataAccess;
using ITMO.Dev.ASAP.Github.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Mapping;
using ITMO.Dev.ASAP.Github.Application.Octokit.Services;
using ITMO.Dev.ASAP.Github.Common.Exceptions;
using ITMO.Dev.ASAP.Github.Common.Extensions;
using ITMO.Dev.ASAP.Github.Domain.Users;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;
using MediatR;
using static ITMO.Dev.ASAP.Github.Application.Contracts.Users.Commands.UpdateGithubUser;

namespace ITMO.Dev.ASAP.Github.Application.Handlers.Users;

internal class UpdateGithubUserHandler : IRequestHandler<Command, Response>
{
    private readonly IAsapUserService _asapUserService;
    private readonly IGithubUserService _githubUserService;
    private readonly IPersistenceContext _context;

    public UpdateGithubUserHandler(
        IAsapUserService asapUserService,
        IGithubUserService githubUserService,
        IPersistenceContext context)
    {
        _asapUserService = asapUserService;
        _githubUserService = githubUserService;
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var query = GithubUserQuery.Build(x => x
            .WithUsername(request.GithubUsername)
            .WithLimit(1));

        bool alreadyExists = await _context.Users
            .QueryAsync(query, cancellationToken)
            .AnyAsync(cancellationToken);

        if (alreadyExists)
            throw GithubUserException.UsernameCollision(request.GithubUsername).TaggedWithConflict();

        bool userExists = await _githubUserService.IsUserExistsAsync(request.GithubUsername, cancellationToken);

        if (userExists is false)
            throw GithubUserException.UserDoesNotExist(request.GithubUsername).TaggedWithNotFound();

        GithubUser? githubUser = await _context.Users
            .QueryAsync(GithubUserQuery.Build(x => x.WithId(request.UserId)), cancellationToken)
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

        await _context.CommitAsync(cancellationToken);

        GithubUserDto dto = githubUser.ToDto();

        return new Response(dto);
    }
}