namespace ITMO.Dev.ASAP.WebApi.Configuration;

public class TestEnvironmentConfiguration
{
    public IReadOnlyCollection<string> Users { get; init; } = new List<string>();
}