using FluentAssertions;
using FluentScanning;
using ITMO.Dev.ASAP.Tests.Controllers.TheoryData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Controllers;

public class AuthorizationTest : TestBase
{
    [Theory]
    [ClassData(typeof(ControllersClassesTestData))]
    public void ControllersShouldNotHaveAuthorizeAttribute(AssemblyProvider assemblyProvider)
    {
        var scanner = new AssemblyScanner(assemblyProvider);

        scanner.ScanForTypesThat()
            .AreAssignableTo<ControllerBase>()
            .Should()
            .NotContain(controller => controller.CustomAttributes
                .Any(attrib => attrib.AttributeType.IsAssignableTo(typeof(AuthorizeAttribute))));
    }

    [Theory]
    [ClassData(typeof(ControllersClassesTestData))]
    public void ControllerActionsShouldHaveAuthorizeAttribute(AssemblyProvider assemblyProvider)
    {
        var scanner = new AssemblyScanner(assemblyProvider);
        TypeInfo[] controllers = scanner.ScanForTypesThat().AreAssignableTo<ControllerBase>().ToArray();

        foreach (TypeInfo controller in controllers)
        {
            MethodInfo[] actions = controller
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.CustomAttributes.Any(attrib => attrib.AttributeType.IsAssignableTo(typeof(IRouteTemplateProvider))))
                .Where(method => method.GetCustomAttribute<AllowAnonymousAttribute>() is null)
                .ToArray();

            MethodInfo[] actionWithoutAuthorization = actions
                .Where(action => action.CustomAttributes.All(attrib =>
                    attrib.AttributeType.IsAssignableTo(typeof(AuthorizeAttribute)) is false))
                .ToArray();

            StringBuilder becauseMessageBuilder = actionWithoutAuthorization
                .Select(action => $"{action.DeclaringType?.Name}.{action.Name}")
                .Aggregate(
                    new StringBuilder("Following methods must explicitly specify AuthorizationAttribute:\n"),
                    (builder, actionName) => builder.Append('\t').AppendLine(actionName));

            actionWithoutAuthorization.Should().BeEmpty(becauseMessageBuilder.ToString());
        }
    }
}