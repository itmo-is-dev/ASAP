namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;

public record CreateUserAccountRequest(string Username, string Password, string RoleName);
