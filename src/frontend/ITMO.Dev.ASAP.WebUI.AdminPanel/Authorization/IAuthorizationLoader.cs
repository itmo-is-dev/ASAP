namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface IAuthorizationLoader
{
    ValueTask LoadAsync();
}