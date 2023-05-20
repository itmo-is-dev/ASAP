using ITMO.Dev.ASAP.Github.Common.Resources;

namespace ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;

public class EntityNotFoundException : AsapGithubException
{
    private EntityNotFoundException(string? message) : base(message) { }

    public static EntityNotFoundException Create<TKey, TEntity>(TKey key)
    {
        return new EntityNotFoundException($"Entity of type {typeof(TEntity).Name} with id {key} not found");
    }

    public static EntityNotFoundException SubjectCourse(string organizationName)
    {
        return new EntityNotFoundException($"Subject course for {organizationName} was not found");
    }

    public static EntityNotFoundException SubjectCourse()
    {
        return new EntityNotFoundException($"Subject course was not found");
    }

    public static EntityNotFoundException Submission()
    {
        return new EntityNotFoundException("Could not find submission");
    }

    public static EntityNotFoundException Assignment()
    {
        return new EntityNotFoundException("Could not find assignment");
    }

    public static EntityNotFoundException User()
    {
        return new EntityNotFoundException("Could not find user");
    }

    public static EntityNotFoundException AssignmentWasNotFound(
        string branchName,
        string subjectCourseTitle,
        string subjectCourseAssignments)
    {
        return new EntityNotFoundException(string.Format(
            GithubUserMessages.AssignmentNotFound,
            branchName,
            subjectCourseTitle,
            subjectCourseAssignments));
    }
}