using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using Moq;

namespace ITMO.Dev.ASAP.Tests.Core;

public class CoreTestBase : TestBase
{
    public CoreTestBase()
    {
        AuthorizationServiceMock = new Mock<IAuthorizationService>();
    }

    protected Mock<IAuthorizationService> AuthorizationServiceMock { get; }
}