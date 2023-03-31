using System.Security.Claims;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization.Implementations;

public class CurrentUser : ICurrentUser
{
    public bool IsAdministrator { get; private set; }

    public bool IsModerator { get; private set; }

    public bool IsMentor { get; private set; }

    public bool IsAuthenticated { get; private set; }

    public class Consumer : IPrincipalConsumer
    {
        private readonly CurrentUser _currentUser;

        public Consumer(CurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public ValueTask OnNextAsync(ClaimsPrincipal principal)
        {
            _currentUser.IsAdministrator = principal.IsInRole("Admin");
            _currentUser.IsModerator = principal.IsInRole("Moderator");
            _currentUser.IsMentor = principal.IsInRole("Mentor");
            _currentUser.IsAuthenticated = principal.Identity?.IsAuthenticated is true;

            return ValueTask.CompletedTask;
        }

        public ValueTask OnExpiredAsync()
        {
            _currentUser.IsAdministrator = false;
            _currentUser.IsModerator = false;
            _currentUser.IsMentor = false;
            _currentUser.IsAuthenticated = false;

            return ValueTask.CompletedTask;
        }
    }
}