using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Github.Presentation.Services.Dummy;

internal class DummyGithubUserService : IGithubUserService
{
    public Task<GithubUserDto?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<GithubUserDto?>(null);
    }

    public Task<IReadOnlyCollection<GithubUserDto>> FindByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<GithubUserDto>>(Array.Empty<GithubUserDto>());
    }
}