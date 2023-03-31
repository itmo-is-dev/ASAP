using ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;
using ITMO.Dev.ASAP.WebUI.AdminPanel.SafeExecution;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Tools;
using Microsoft.AspNetCore.Components;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.ExceptionHandling;

public class SafeExecutor : ISafeExecutor
{
    private readonly IExceptionSink _exceptionSink;
    private readonly NavigationManager _navigationManager;
    private readonly EnvironmentConfiguration _configuration;
    private readonly IPrincipalService _principalService;

    public SafeExecutor(
        IExceptionSink exceptionSink,
        NavigationManager navigationManager,
        EnvironmentConfiguration configuration,
        IPrincipalService principalService)
    {
        _exceptionSink = exceptionSink;
        _navigationManager = navigationManager;
        _configuration = configuration;
        _principalService = principalService;
    }

    public ISafeExecutionBuilder Execute(Func<Task> action)
        => new SafeExecutionBuilder(action, _exceptionSink, _navigationManager, _configuration, _principalService);

    public ISafeExecutionBuilder<T> Execute<T>(Func<Task<T>> action)
        => new SafeExecutionBuilder<T>(action, _exceptionSink, _navigationManager, _configuration, _principalService);
}