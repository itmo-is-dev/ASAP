namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface IPrincipalService
{
    Task LoginAsync(string username, string password, CancellationToken cancellationToken);

    Task LogoutAsync(CancellationToken cancellationToken);

    Task UpdateUsernameAsync(string username, CancellationToken cancellationToken);

    Task UpdatePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken);
}