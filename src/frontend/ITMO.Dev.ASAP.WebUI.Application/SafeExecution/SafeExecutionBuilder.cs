using ITMO.Dev.ASAP.WebApi.Sdk.Errors;
using ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;
using ITMO.Dev.ASAP.WebUI.Abstractions.Authorization;
using ITMO.Dev.ASAP.WebUI.Abstractions.Models;
using ITMO.Dev.ASAP.WebUI.Abstractions.SafeExecution;
using Microsoft.AspNetCore.Components;

namespace ITMO.Dev.ASAP.WebUI.Application.SafeExecution;

public class SafeExecutionBuilder : ISafeExecutionBuilder
{
    private readonly Func<Task> _action;
    private readonly List<IExceptionHandler> _errorHandlers;
    private readonly IExceptionSink _exceptionSink;
    private readonly NavigationManager _navigationManager;
    private readonly List<Func<Task>> _successHandlers;
    private readonly EnvironmentConfiguration _configuration;
    private readonly IPrincipalService _principalService;

    public SafeExecutionBuilder(
        Func<Task> action,
        IExceptionSink exceptionSink,
        NavigationManager navigationManager,
        EnvironmentConfiguration configuration,
        IPrincipalService principalService)
    {
        _exceptionSink = exceptionSink;
        _navigationManager = navigationManager;
        _configuration = configuration;
        _principalService = principalService;
        _action = action;

        _successHandlers = new List<Func<Task>>();
        _errorHandlers = new List<IExceptionHandler>();
    }

    public string? Title { get; set; }

    public bool ShowExceptionDetails { get; set; }

    public void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception
    {
        _errorHandlers.Add(new ExceptionHandler<TException>(action));
    }

    public void OnFailAsync(Func<Exception, Task> action)
    {
        _errorHandlers.Add(new ExceptionHandler<Exception>(action));
    }

    public void OnSuccessAsync(Func<Task> action)
    {
        _successHandlers.Add(action);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _action.Invoke();
            IEnumerable<Task> handleTasks = _successHandlers.Select(x => x.Invoke());

            await Task.WhenAll(handleTasks);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception e)
        {
            IEnumerable<Task> handleTasks = _errorHandlers.Select(x => x.Handle(e));
            await Task.WhenAll(handleTasks);

            if (e is UnauthorizedException)
            {
                await _principalService.LogoutAsync(default);
                _navigationManager.NavigateTo("/");

                return;
            }

            string? message = ShowExceptionDetails || _configuration.IsDevelopment ? e.Message : null;

            if (_configuration.IsDevelopment)
            {
                Console.WriteLine(e.Message);
            }

            await _exceptionSink.ConsumeAsync(Title, message);
        }
    }
}

#pragma warning disable SA1402
public class SafeExecutionBuilder<T> : ISafeExecutionBuilder<T>
#pragma warning restore SA1402
{
    private readonly Func<Task<T>> _action;
    private readonly List<IExceptionHandler> _errorHandlers;
    private readonly IExceptionSink _exceptionSink;
    private readonly NavigationManager _navigationManager;
    private readonly List<Func<T, Task>> _successHandlers;
    private readonly EnvironmentConfiguration _configuration;
    private readonly IPrincipalService _principalService;

    public SafeExecutionBuilder(
        Func<Task<T>> action,
        IExceptionSink exceptionSink,
        NavigationManager navigationManager,
        EnvironmentConfiguration configuration,
        IPrincipalService principalService)
    {
        _exceptionSink = exceptionSink;
        _navigationManager = navigationManager;
        _configuration = configuration;
        _principalService = principalService;
        _action = action;

        _successHandlers = new List<Func<T, Task>>();
        _errorHandlers = new List<IExceptionHandler>();
    }

    public string? Title { get; set; }

    public bool ShowExceptionDetails { get; set; }

    public void OnFailAsync<TException>(Func<TException, Task> action) where TException : Exception
    {
        _errorHandlers.Add(new ExceptionHandler<TException>(action));
    }

    public void OnFailAsync(Func<Exception, Task> action)
    {
        _errorHandlers.Add(new ExceptionHandler<Exception>(action));
    }

    public void OnSuccessAsync(Func<T, Task> action)
    {
        _successHandlers.Add(action);
    }

    public void OnSuccessAsync(Func<Task> action)
    {
        _successHandlers.Add(_ => action());
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            T value = await _action.Invoke();
            IEnumerable<Task> handleTasks = _successHandlers.Select(x => x.Invoke(value));

            await Task.WhenAll(handleTasks);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception e)
        {
            IEnumerable<Task> handleTasks = _errorHandlers.Select(x => x.Handle(e));
            await Task.WhenAll(handleTasks);

            if (e is UnauthorizedException)
            {
                await _principalService.LogoutAsync(default);
                _navigationManager.NavigateTo("/");

                return;
            }

            string? message = ShowExceptionDetails || _configuration.IsDevelopment ? e.Message : null;

            if (_configuration.IsDevelopment)
            {
                Console.WriteLine(e.Message);
            }

            await _exceptionSink.ConsumeAsync(Title, message);
        }
    }
}