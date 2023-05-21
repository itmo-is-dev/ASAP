namespace ITMO.Dev.ASAP.Exceptions;

public class StartupException : Exception
{
    internal StartupException()
        : base("Application is unable to startup") { }

    internal StartupException(string message)
        : base(message) { }
}