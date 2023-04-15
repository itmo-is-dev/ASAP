namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;

public record UpdatePasswordRequest(string CurrentPassword, string NewPassword);