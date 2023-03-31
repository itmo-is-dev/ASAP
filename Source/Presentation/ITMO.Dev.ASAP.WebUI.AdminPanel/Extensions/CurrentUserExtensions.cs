using ITMO.Dev.ASAP.WebUI.AdminPanel.Authorization;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;

public static class CurrentUserExtensions
{
    public static bool HasModeratorAccess(this ICurrentUser user)
        => user.IsAdministrator || user.IsModerator;

    public static bool HasMentorAccess(this ICurrentUser user)
        => user.HasModeratorAccess() || user.IsMentor;
}