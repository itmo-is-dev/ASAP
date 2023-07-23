namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;

public class Disposable
{
    public static IDisposable From(params IDisposable[] disposables)
    {
        return new CollectionDisposable(disposables);
    }

    public static IDisposable Empty { get; } = new NoOpDisposable();

    private record CollectionDisposable(IEnumerable<IDisposable> Disposables) : IDisposable
    {
        public void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch
                {
                    // suppress
                }
            }
        }
    }

    private class NoOpDisposable : IDisposable
    {
        public void Dispose() { }
    }
}