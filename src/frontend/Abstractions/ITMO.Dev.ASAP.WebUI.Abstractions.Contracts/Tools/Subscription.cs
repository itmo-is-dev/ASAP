namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;

public class Subscription : IDisposable
{
    private readonly IReadOnlyCollection<IDisposable> _disposables;

    public Subscription(IReadOnlyCollection<IDisposable> disposables)
    {
        _disposables = disposables;
    }

    public void Dispose()
    {
        foreach (IDisposable disposable in _disposables)
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