using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.Presentation.Contracts.Services;

public interface IAsapUserService
{
    Task<UserDto> CreateUserAsync(
        string firstName,
        string middleName,
        string lastName,
        CancellationToken cancellationToken);

    Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
}