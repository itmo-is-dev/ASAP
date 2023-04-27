namespace ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

public interface IAuthorizationLoader
{
    ValueTask LoadAsync();
}