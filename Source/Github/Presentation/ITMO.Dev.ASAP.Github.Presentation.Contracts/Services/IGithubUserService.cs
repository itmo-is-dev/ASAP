using ITMO.Dev.ASAP.Github.Application.Dto.Users;

namespace ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;

public interface IGithubUserService
{
    Task<IReadOnlyCollection<GithubUserDto>> GetAllAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken);

    Task<GithubUserDto> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<GithubUserDto?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GithubUserDto>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}