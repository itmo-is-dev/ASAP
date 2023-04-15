namespace ITMO.Dev.ASAP.Github.Application.Dto.PullRequests;

public record PullRequestDto(
    string Sender,
    string Payload,
    string Organization,
    string Repository,
    string BranchName,
    long PullRequestNumber)
{
    public override string ToString()
    {
        return $"{Payload} with branch {BranchName} from {Sender}";
    }
}