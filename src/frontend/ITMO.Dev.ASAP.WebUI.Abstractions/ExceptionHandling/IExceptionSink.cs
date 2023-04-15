namespace ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;

public interface IExceptionSink
{
    ValueTask ConsumeAsync(Exception exception, string? title, string? message);
}