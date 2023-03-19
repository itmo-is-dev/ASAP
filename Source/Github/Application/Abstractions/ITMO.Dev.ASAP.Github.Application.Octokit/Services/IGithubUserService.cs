namespace ITMO.Dev.ASAP.Github.Application.Octokit.Services;

public interface IGithubUserService
{
    Task<bool> IsUserExistsAsync(string username, CancellationToken cancellationToken);
}