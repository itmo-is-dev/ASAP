namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Tools;

public class SubscriptionBuilder
{
    private readonly List<IDisposable> _disposables;

    public SubscriptionBuilder()
    {
        _disposables = new List<IDisposable>();
    }

    public SubscriptionBuilder Subscribe(IDisposable subscription)
    {
        _disposables.Add(subscription);
        return this;
    }

    public Subscription Build()
    {
        return new Subscription(_disposables);
    }
}