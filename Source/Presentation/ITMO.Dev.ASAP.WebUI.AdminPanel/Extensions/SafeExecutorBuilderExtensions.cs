using ITMO.Dev.ASAP.WebUI.AdminPanel.SafeExecution;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;

public static class SafeExecutorBuilderExtensions
{
    public static void OnFailAsync(this ISafeExecutionBuilder builder, Func<Exception, Task> action)
        => builder.OnFailAsync<Exception>(action.Invoke);

    public static void OnFailAsync(this ISafeExecutionBuilder builder, Func<Task> action)
        => builder.OnFailAsync(_ => action.Invoke());

    public static void OnFail(this ISafeExecutionBuilder builder, Action action)
    {
        builder.OnFailAsync(() =>
        {
            action.Invoke();
            return Task.CompletedTask;
        });
    }

    public static void OnSuccess(this ISafeExecutionBuilder builder, Action action)
    {
        builder.OnSuccessAsync(() =>
        {
            action.Invoke();
            return Task.CompletedTask;
        });
    }

    public static void OnSuccess<T>(this ISafeExecutionBuilder<T> builder, Action<T> action)
    {
        builder.OnSuccessAsync(x =>
        {
            action.Invoke(x);
            return Task.CompletedTask;
        });
    }
}