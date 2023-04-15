using ITMO.Dev.ASAP.Github.Application.Dto.Users;

namespace ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

public interface IGithubUserService
{
    Task<GithubUserDto?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GithubUserDto>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}