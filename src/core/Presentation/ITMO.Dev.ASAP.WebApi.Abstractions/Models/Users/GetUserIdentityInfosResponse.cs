using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.Users;

public record GetUserIdentityInfosResponse(IReadOnlyCollection<UserIdentityInfoDto> Users, int PageCount);