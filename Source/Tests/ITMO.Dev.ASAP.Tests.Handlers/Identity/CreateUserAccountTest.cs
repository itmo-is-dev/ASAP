﻿using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;
using ITMO.Dev.ASAP.Application.Handlers.Identity;
using ITMO.Dev.ASAP.Application.Users;
using ITMO.Dev.ASAP.Identity.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.Identity;

public class CreateUserAccountTest : TestBase
{
    [Theory]
    [InlineData(AsapIdentityRoleNames.MentorRoleName)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName)]
    public async Task AdminCreateAnyRole_NoThrow(string roleName)
    {
        string username = string.Empty;
        string password = string.Empty;

        Guid userId = await Context.Users
            .Select(x => x.Id)
            .FirstAsync();

        var admin = new AdminUser(Guid.Empty);

        var command = new CreateUserAccount.Command(userId, username, password, roleName);
        var handler = new CreateUserAccountHandler(Context, admin, IdentityServiceMock.Object);

        await handler.Handle(command, default);
    }

    [Theory]
    [InlineData(AsapIdentityRoleNames.MentorRoleName)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName)]
    public async Task MentorCreateAnyRole_ThrowException(string roleName)
    {
        string username = string.Empty;
        string password = string.Empty;

        Guid userId = await Context.Users
            .Select(x => x.Id)
            .FirstAsync();

        var admin = new MentorUser(Guid.Empty);

        var command = new CreateUserAccount.Command(userId, username, password, roleName);
        var handler = new CreateUserAccountHandler(Context, admin, IdentityServiceMock.Object);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            handler.Handle(command, default));
    }

    [Theory]
    [InlineData(AsapIdentityRoleNames.MentorRoleName, false)]
    [InlineData(AsapIdentityRoleNames.ModeratorRoleName, true)]
    [InlineData(AsapIdentityRoleNames.AdminRoleName, true)]
    public async Task ModeratorCanCreateOnlyMentor(string roleName, bool throwExpected)
    {
        string username = string.Empty;
        string password = string.Empty;

        Guid userId = await Context.Users
            .Select(x => x.Id)
            .FirstAsync();

        var admin = new ModeratorUser(Guid.Empty);

        var command = new CreateUserAccount.Command(userId, username, password, roleName);
        var handler = new CreateUserAccountHandler(Context, admin, IdentityServiceMock.Object);

        if (throwExpected)
        {
            await Assert.ThrowsAsync<AccessDeniedException>(() =>
                handler.Handle(command, default));
        }
        else
        {
            await handler.Handle(command, default);
        }
    }
}