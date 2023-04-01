using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Identity;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Identity.Abstractions.Entities;
using Moq;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.Identity;

public class ChangeUserRoleTest : TestBase
{
    [Theory]
    [InlineData(AsapIdentityRole.MentorRoleName, AsapIdentityRole.AdminRoleName)]
    [InlineData(AsapIdentityRole.MentorRoleName, AsapIdentityRole.ModeratorRoleName)]
    [InlineData(AsapIdentityRole.AdminRoleName, AsapIdentityRole.MentorRoleName)]
    [InlineData(AsapIdentityRole.AdminRoleName, AsapIdentityRole.ModeratorRoleName)]
    [InlineData(AsapIdentityRole.ModeratorRoleName, AsapIdentityRole.MentorRoleName)]
    [InlineData(AsapIdentityRole.ModeratorRoleName, AsapIdentityRole.AdminRoleName)]
    public async Task AdminChangeAnyRole_NoThrow(string currentRole, string newRole)
    {
        var user = new AsapIdentityUser
        {
            Id = Guid.Empty,
            UserName = string.Empty,
        };

        var currentUser = new AdminUser(Guid.Empty);

        IdentityServiceMock
            .Setup(x => x.GetUserByNameAsync(user.UserName, default))
            .ReturnsAsync(user);

        IdentityServiceMock
            .Setup(x => x.GetUserRoleAsync(user, default))
            .ReturnsAsync(currentRole);

        var command = new ChangeUserRole.Command(user.UserName, newRole);
        var handler = new ChangeUserRoleHandler(currentUser, IdentityServiceMock.Object);

        await handler.Handle(command, default);
    }

    [Theory]
    [InlineData(AsapIdentityRole.MentorRoleName, AsapIdentityRole.AdminRoleName)]
    [InlineData(AsapIdentityRole.MentorRoleName, AsapIdentityRole.ModeratorRoleName)]
    [InlineData(AsapIdentityRole.AdminRoleName, AsapIdentityRole.MentorRoleName)]
    [InlineData(AsapIdentityRole.AdminRoleName, AsapIdentityRole.ModeratorRoleName)]
    [InlineData(AsapIdentityRole.ModeratorRoleName, AsapIdentityRole.MentorRoleName)]
    [InlineData(AsapIdentityRole.ModeratorRoleName, AsapIdentityRole.AdminRoleName)]
    public async Task MentorChangeAnyRole_ThrowException(string currentRole, string newRole)
    {
        var user = new AsapIdentityUser
        {
            Id = Guid.Empty,
            UserName = string.Empty,
        };

        var currentUser = new MentorUser(Guid.Empty);

        IdentityServiceMock
            .Setup(x => x.GetUserByNameAsync(user.UserName, default))
            .ReturnsAsync(user);

        IdentityServiceMock
            .Setup(x => x.GetUserRoleAsync(user, default))
            .ReturnsAsync(currentRole);

        var command = new ChangeUserRole.Command(user.UserName, newRole);
        var handler = new ChangeUserRoleHandler(currentUser, IdentityServiceMock.Object);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            handler.Handle(command, default));
    }
}
