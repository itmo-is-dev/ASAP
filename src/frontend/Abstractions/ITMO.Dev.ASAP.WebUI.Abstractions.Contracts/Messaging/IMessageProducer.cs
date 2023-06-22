namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;

public interface IMessageProducer
{
    IObservable<T> Observe<T>();
}