using System.Reactive.Subjects;
using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization.Implementations;

public class CurrentUser : ICurrentUser, IDisposable
{
    private readonly Subject<string> _nameSubject;

    public CurrentUser()
    {
        _nameSubject = new Subject<string>();
    }

    public string? Name { get; private set; }

    public IObservable<string> OnNameChanged => _nameSubject;

    public bool IsAdministrator { get; private set; }

    public bool IsModerator { get; private set; }

    public bool IsMentor { get; private set; }

    public bool IsAuthenticated { get; private set; }

    public void Dispose()
    {
        _nameSubject.Dispose();
    }

    public class Consumer : IPrincipalConsumer
    {
        private readonly CurrentUser _currentUser;

        public Consumer(CurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public ValueTask OnNextAsync(ClaimsPrincipal principal)
        {
            Claim? nameClaim = principal.Claims.SingleOrDefault(
                x => x.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase));

            _currentUser.Name = nameClaim?.Value;
            _currentUser._nameSubject.OnNext(nameClaim?.Value ?? string.Empty);
            _currentUser.IsAdministrator = principal.IsInRole("Admin");
            _currentUser.IsModerator = principal.IsInRole("Moderator");
            _currentUser.IsMentor = principal.IsInRole("Mentor");
            _currentUser.IsAuthenticated = principal.Identity?.IsAuthenticated is true;

            return ValueTask.CompletedTask;
        }

        public ValueTask OnExpiredAsync()
        {
            _currentUser.Name = null;
            _currentUser._nameSubject.OnNext(string.Empty);
            _currentUser.IsAdministrator = false;
            _currentUser.IsModerator = false;
            _currentUser.IsMentor = false;
            _currentUser.IsAuthenticated = false;

            return ValueTask.CompletedTask;
        }
    }
}