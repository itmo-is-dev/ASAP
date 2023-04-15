using ITMO.Dev.ASAP.Github.Common.Exceptions;
using System.Net;

namespace ITMO.Dev.ASAP.Github.Common.Extensions;

public static class AsapGithubExtensions
{
    public static AsapGithubException TaggedWithNotFound(this AsapGithubException exception)
        => exception.TaggedWith(HttpStatusCode.NotFound);

    public static AsapGithubException TaggedWithBadRequest(this AsapGithubException exception)
        => exception.TaggedWith(HttpStatusCode.BadRequest);

    public static AsapGithubException TaggedWithConflict(this AsapGithubException exception)
        => exception.TaggedWith(HttpStatusCode.Conflict);

    private static AsapGithubException TaggedWith(this AsapGithubException exception, HttpStatusCode code)
        => new HttpTaggedException(exception, code);
}