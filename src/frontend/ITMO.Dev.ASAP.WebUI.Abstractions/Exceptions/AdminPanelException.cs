namespace ITMO.Dev.ASAP.WebUI.Abstractions.Exceptions;

public abstract class AdminPanelException : Exception
{
    protected AdminPanelException(string? message)
        : base(message) { }
}