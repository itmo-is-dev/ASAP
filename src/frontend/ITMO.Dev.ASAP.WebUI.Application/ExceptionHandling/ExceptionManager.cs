using ITMO.Dev.ASAP.WebApi.Sdk.Errors;
using ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;
using ITMO.Dev.ASAP.WebUI.Abstractions.Models;

namespace ITMO.Dev.ASAP.WebUI.Application.ExceptionHandling;

internal class ExceptionManager : IExceptionSink, IExceptionStore
{
    private readonly ExceptionDisplayConfiguration _configuration;
    private readonly HashSet<ExceptionMessage> _exceptions;

    public ExceptionManager(ExceptionDisplayConfiguration configuration)
    {
        _configuration = configuration;
        _exceptions = new HashSet<ExceptionMessage>();
    }

    public IReadOnlyCollection<ExceptionMessage> Exceptions => _exceptions;

    public async ValueTask ConsumeAsync(string? title, string? message)
    {
        var value = new ExceptionMessage(title, message);
        _exceptions.Add(value);
        OnExceptionAdded(value);

        await Task.Delay(_configuration.PopupLifetime);
        Dismiss(value);
    }

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