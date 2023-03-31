namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface ICurrentUser
{
    bool IsAdministrator { get; }

    bool IsModerator { get; }

    bool IsMentor { get; }

    bool IsAuthenticated { get; }
}