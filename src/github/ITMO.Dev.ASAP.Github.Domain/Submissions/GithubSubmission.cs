using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Domain.Submissions;

public partial class GithubSubmission : IEntity<Guid>
{
    public GithubSubmission(
        Guid id,
        Guid assignmentId,
        Guid userId,
        DateTime createdAt,
        string organization,
        string repository,
        long pullRequestNumber) : this(id)
    {
        AssignmentId = assignmentId;
        UserId = userId;
        CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
        Repository = repository;
        PullRequestNumber = pullRequestNumber;
        Organization = organization;
    }

    public Guid AssignmentId { get; protected init; }

    public Guid UserId { get; protected init; }

    public DateTime CreatedAt { get; protected init; }

    public string Organization { get; protected set; }

    public string Repository { get; protected set; }

    public long PullRequestNumber { get; protected set; }
}