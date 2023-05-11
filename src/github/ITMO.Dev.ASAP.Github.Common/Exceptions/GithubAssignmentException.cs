namespace ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;

public class GithubAssignmentException : AsapGithubException
{
    private GithubAssignmentException(string? message) : base(message) { }

    public static GithubAssignmentException AssignmentAlreadyExists(Guid assignmentId, string assignmentName)
        => new GithubAssignmentException($"Github assignment {assignmentName} with id: {assignmentId} already exists");
}