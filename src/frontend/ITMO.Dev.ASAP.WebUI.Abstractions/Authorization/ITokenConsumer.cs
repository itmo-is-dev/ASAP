namespace ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;

public interface ITokenConsumer
{
    ValueTask OnNextAsync(string token);

    ValueTask OnExpiredAsync();
}