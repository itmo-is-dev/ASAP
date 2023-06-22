using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace ITMO.Dev.ASAP.WebUI.Application.ViewModels.Messaging;

public class MessageProducer : IMessageProducer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(IServiceProvider serviceProvider, ILogger<MessageProducer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IObservable<T> Observe<T>()
    {
        IObservable<T>? observable = _serviceProvider.GetService<IObservable<T>>();

        if (observable is not null)
            return observable;

        _logger.LogWarning("No observer found for Type = {T}", typeof(T));
        return Observable.Empty<T>();
    }
}