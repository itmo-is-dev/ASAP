using FluentAssertions;
using FluentScanning;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Authorization;
using ITMO.Dev.ASAP.Authorization.Models;
using ITMO.Dev.ASAP.Tests.Controllers.Fixtures;
using ITMO.Dev.ASAP.Tests.Controllers.TheoryData;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Controllers;

public class AuthorizeFeatureTest : TestBase, IClassFixture<AuthorizationFeatureConfigurationFixture>
{
    private readonly AuthorizationConfiguration _configuration;

    public AuthorizeFeatureTest(AuthorizationFeatureConfigurationFixture fixture)
    {
        _configuration = fixture.Configuration;
    }

    [Theory]
    [ClassData(typeof(ControllersClassesTestData))]
    public void FeatureScopesShouldBeDefined(AssemblyProvider assemblyProvider)
    {
        var scanner = new AssemblyScanner(assemblyProvider);
        IScanningQuery controllers = scanner.ScanForTypesThat().AreAssignableTo<ControllerBase>();

        IEnumerable<AuthorizeFeatureAttribute> features = controllers
            .SelectMany(controller => controller.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            .Select(method => method.GetCustomAttribute<AuthorizeFeatureAttribute>())
            .WhereNotNull();

        foreach (AuthorizeFeatureAttribute feature in features)
        {
            _configuration
                .FeatureScopes.Should().ContainKey(feature.Scope)
                .WhoseValue.Should().ContainKey(feature.Feature)
                .WhoseValue.Should().NotBeEmpty();
        }
    }
}