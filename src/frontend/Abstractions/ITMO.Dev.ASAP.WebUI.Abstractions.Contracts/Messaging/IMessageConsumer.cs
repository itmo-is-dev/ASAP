namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;

public interface IMessageConsumer
{
    void Send<T>(T message);

    void SendRange<T>(IEnumerable<T> messages);
}