namespace ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;

public interface IExceptionHandler
{
    Task Handle(Exception exception);
}