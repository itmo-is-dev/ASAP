using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITMO.Dev.ASAP.DataAccess.Tools;

public static class RepositoryTools
{
    public static EntityEntry<T> GetEntry<T>(DbContext context, Func<T, bool> selector, Func<T> factory) where T : class
    {
        DbSet<T> set = context.Set<T>();

        T? existing = set.Local.FirstOrDefault(selector);

        return existing is not null
            ? context.Entry(existing)
            : set.Attach(factory.Invoke());
    }
}