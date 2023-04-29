namespace ITMO.Dev.ASAP.Google.Application.Abstractions.Providers;

public interface ITablesParentsProvider
{
    IList<string> GetParents();
}