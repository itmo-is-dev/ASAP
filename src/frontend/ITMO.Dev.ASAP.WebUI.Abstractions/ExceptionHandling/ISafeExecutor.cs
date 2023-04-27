using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.ExceptionHandling;

public interface ISafeExecutor
{
    ISafeExecutionBuilder Execute(Func<Task> action);

    ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action);
}