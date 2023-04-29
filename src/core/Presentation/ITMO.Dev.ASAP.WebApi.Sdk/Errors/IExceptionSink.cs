namespace ITMO.Dev.ASAP.WebApi.Sdk.Errors;

public interface IExceptionSink
{
    ValueTask ConsumeAsync(string? title, string? message);
}