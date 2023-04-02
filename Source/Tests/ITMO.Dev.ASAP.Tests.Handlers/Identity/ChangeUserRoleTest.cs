using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Identity;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Identity.Abstractions.Models;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.Identity;

public class ChangeUserRoleTest : TestBase
{
    [Theory]
    [InlineData(AsapIdentityRoleNames.MentorRoleName, AsapIdentityRoleNames.AdminRoleName)]
    [InlineData(AsapIdentityRoleNames.MentorRoleName, AsapIdentityRoleNames.ModeratorRoleName)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName, AsapIdentityRoleNames.MentorRoleName)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName, AsapIdentityRoleNames.ModeratorRoleName)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName, AsapIdentityRoleNames.MentorRoleName)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName, AsapIdentityRoleNames.AdminRoleName)]
    public async Task AdminChangeAnyRole_NoThrow(string currentRole, string newRole)
    {
        var user = new AsapIdentityUserDto(
            Id: Guid.Empty,
            Username: string.Empty);

        var currentUser = new AdminUser(Guid.Empty);

        IdentityServiceMock
            .Setup(x => x.GetUserByNameAsync(user.Username, default))
            .ReturnsAsync(user);

        IdentityServiceMock
            .Setup(x => x.GetUserRoleAsync(user.Id, default))
            .ReturnsAsync(currentRole);

        var command = new ChangeUserRole.Command(user.Username, newRole);
        var handler = new ChangeUserRoleHandler(currentUser, IdentityServiceMock.Object);

        await handler.Handle(command, default);
    }

    [Theory]
    [InlineData(AsapIdentityRoleNames.MentorRoleName, AsapIdentityRoleNames.AdminRoleName)]
    [InlineData(AsapIdentityRoleNames.MentorRoleName, AsapIdentityRoleNames.ModeratorRoleName)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName, AsapIdentityRoleNames.MentorRoleName)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName, AsapIdentityRoleNames.ModeratorRoleName)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName, AsapIdentityRoleNames.MentorRoleName)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName, AsapIdentityRoleNames.AdminRoleName)]
    public async Task MentorChangeAnyRole_ThrowException(string currentRole, string newRole)
    {
        var user = new AsapIdentityUserDto(
            Id: Guid.Empty,
            Username: string.Empty);

        var currentUser = new MentorUser(Guid.Empty);

        IdentityServiceMock
            .Setup(x => x.GetUserByNameAsync(user.Username, default))
            .ReturnsAsync(user);

        IdentityServiceMock
            .Setup(x => x.GetUserRoleAsync(user.Id, default))
            .ReturnsAsync(currentRole);

        var command = new ChangeUserRole.Command(user.Username, newRole);
        var handler = new ChangeUserRoleHandler(currentUser, IdentityServiceMock.Object);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            handler.Handle(command, default));
    }
}
