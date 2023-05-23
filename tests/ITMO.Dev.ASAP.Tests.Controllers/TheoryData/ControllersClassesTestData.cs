using ITMO.Dev.ASAP.Controllers;
using ITMO.Dev.ASAP.Github.Presentation.Controllers;
using System.Collections;

namespace ITMO.Dev.ASAP.Tests.Controllers.TheoryData;

internal sealed class ControllersClassesTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { typeof(ICoreControllerMarker) };
        yield return new object[] { typeof(IGithubControllerMarker) };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}