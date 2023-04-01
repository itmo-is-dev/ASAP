namespace ITMO.Dev.ASAP.Identity.Abstractions.Models;

public record AuthenticationResult(
    string Token,
    string UserRole,
    DateTime Expires);
