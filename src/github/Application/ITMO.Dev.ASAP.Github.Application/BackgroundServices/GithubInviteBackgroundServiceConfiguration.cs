namespace ITMO.Dev.ASAP.Github.Application.BackgroundServices;

public class GithubInviteBackgroundServiceConfiguration
{
    public TimeSpan Delay => new TimeSpan(0, Hours, Minutes);

    public int Hours { get; set; }

    public int Minutes { get; set; }
}