namespace ITMO.Dev.ASAP.Application.Dto.Users;

public record StudentDto(UserDto User, Guid? GroupId, string GroupName, int? UniversityId, string? GitHubUsername);