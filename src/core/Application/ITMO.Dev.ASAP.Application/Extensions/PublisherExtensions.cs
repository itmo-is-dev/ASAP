// ReSharper disable once CheckNamespace

namespace MediatR;

public static class PublisherExtensions
{
    // Extensions for proper copilot autocompletion
    public static Task PublishAsync<TNotification>(
        this IPublisher publisher,
        TNotification notification,
        CancellationToken cancellationToken) where TNotification : INotification
    {
        return publisher.Publish(notification, cancellationToken);
    }
}