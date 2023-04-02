namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface IAuthorizationService
{
    Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken);

    Task<string> GetUserTokenAsync(string username, CancellationToken cancellationToken);
}