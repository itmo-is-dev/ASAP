namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;

public interface IMessageProvider
{
    IObservable<T> Observe<T>();
}