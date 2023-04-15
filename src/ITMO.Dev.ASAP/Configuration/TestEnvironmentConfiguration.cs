namespace ITMO.Dev.ASAP.Configuration;

public class TestEnvironmentConfiguration
{
    public IReadOnlyCollection<string> Users { get; init; } = new List<string>();
}