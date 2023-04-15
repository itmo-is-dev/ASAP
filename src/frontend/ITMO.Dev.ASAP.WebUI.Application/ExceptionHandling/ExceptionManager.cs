using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Models;
using ITMO.Dev.ASAP.WebUI.Application.Tools;

namespace ITMO.Dev.ASAP.WebUI.Application.ExceptionHandling;

internal class ExceptionManager : IExceptionSink, IExceptionStore
{
    private readonly ExceptionDisplayConfiguration _configuration;
    private readonly HashSet<ExceptionMessage> _exceptions;

    public ExceptionManager(ExceptionDisplayConfiguration configuration)
    {
        _configuration = configuration;
        IEqualityComparer<ExceptionMessage> comparer =
            EqualityComparerFactory.Create<ExceptionMessage>((a, b) => a.Exception.Equals(b.Exception));
        _exceptions = new HashSet<ExceptionMessage>(comparer);
    }

    public async ValueTask ConsumeAsync(Exception exception, string? title, string? message)
    {
        var value = new ExceptionMessage(title, message, exception);
        _exceptions.Add(value);
        OnExceptionAdded(value);

        await Task.Delay(_configuration.PopupLifetime);
        Dismiss(new ExceptionMessage(title, message, exception));
    }

    public IReadOnlyCollection<ExceptionMessage> Exceptions => _exceptions;

    public void Dismiss(ExceptionMessage exception)
    {
        _exceptions.Remove(exception);
        OnExceptionDismissed(exception);
    }

    public event Action<ExceptionMessage>? ExceptionAdded;

    public event Action<ExceptionMessage>? ExceptionDismissed;

    protected virtual void OnExceptionAdded(ExceptionMessage obj)
    {
        ExceptionAdded?.Invoke(obj);
    }

    protected virtual void OnExceptionDismissed(ExceptionMessage obj)
    {
        ExceptionDismissed?.Invoke(obj);
    }
}