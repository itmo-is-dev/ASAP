namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface ICurrentUser
{
    string? Name { get; }

    IObservable<string> OnNameChanged { get; }

    bool IsAdministrator { get; }

    bool IsModerator { get; }

    bool IsMentor { get; }

    bool IsAuthenticated { get; }
}