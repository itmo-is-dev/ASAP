namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface IPrincipalService
{
    Task LoginAsync(string username, string password, CancellationToken cancellationToken);

    Task LogoutAsync(CancellationToken cancellationToken);
}