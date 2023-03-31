namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

public interface ITokenConsumer
{
    ValueTask OnNextAsync(string token);

    ValueTask OnExpiredAsync();
}