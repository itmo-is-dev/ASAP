namespace ITMO.Dev.ASAP.Exceptions;

public class StartupException : Exception
{
    public StartupException()
        : base("Application is unable to startup") { }

    public StartupException(string message)
        : base(message) { }
}