using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(IServiceProvider serviceProvider, ILogger<MessagePublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Send<T>(T message)
    {
        IObserver<T>? observer = _serviceProvider.GetService<IObserver<T>>();

        if (observer is null)
        {
            _logger.LogWarning("No observer found for event of Type = {T}", typeof(T));
        }
        else
        {
            observer.OnNext(message);
        }
    }

    public void SendRange<T>(IEnumerable<T> messages)
    {
        IObserver<T>? observer = _serviceProvider.GetService<IObserver<T>>();

        if (observer is null)
        {
            _logger.LogWarning("No observer found for event of Type = {T}", typeof(T));
        }
        else
        {
            foreach (T message in messages)
            {
                observer.OnNext(message);
            }
        }
    }
}