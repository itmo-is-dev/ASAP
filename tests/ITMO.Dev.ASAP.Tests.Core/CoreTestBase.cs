using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using Moq;
using Serilog;
using Xunit.Abstractions;

namespace ITMO.Dev.ASAP.Tests.Core;

public abstract class CoreTestBase : TestBase
{
    protected CoreTestBase(ITestOutputHelper? output = null)
    {
        AuthorizationServiceMock = new Mock<IAuthorizationService>();

        if (output is not null)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output)
                .CreateLogger();
        }
    }

    protected Mock<IAuthorizationService> AuthorizationServiceMock { get; }
}