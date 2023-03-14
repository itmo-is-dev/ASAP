namespace ITMO.Dev.ASAP.Application.Common.Exceptions;

public class UserHasNotAccessException : ApplicationException
{
    private UserHasNotAccessException(string message)
        : base(message) { }

    public static UserHasNotAccessException EmptyAvailableList(Guid userId)
    {
        return new UserHasNotAccessException(
            $"User {userId} doesn't have any available information from the list");
    }

    public static UserHasNotAccessException AccessViolation(Guid userId)
    {
        return new UserHasNotAccessException($"User {userId} has not access to this field");
    }

    public static UserHasNotAccessException AnonymousUserHasNotAccess()
    {
        return new UserHasNotAccessException("Anonymous user cannot have an access to this information");
    }
}