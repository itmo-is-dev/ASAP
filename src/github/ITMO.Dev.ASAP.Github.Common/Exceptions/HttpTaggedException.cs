using System.Net;

namespace ITMO.Dev.ASAP.Github.Common.Exceptions;

public class HttpTaggedException : AsapGithubException
{
    internal HttpTaggedException(AsapGithubException wrapped, HttpStatusCode code)
    {
        Wrapped = wrapped;
        Code = code;
    }

    public AsapGithubException Wrapped { get; }

    public HttpStatusCode Code { get; }

    public override string ToString()
    {
        return $"{Code} - {Wrapped.GetType().Name} = {Wrapped.Message}";
    }
}