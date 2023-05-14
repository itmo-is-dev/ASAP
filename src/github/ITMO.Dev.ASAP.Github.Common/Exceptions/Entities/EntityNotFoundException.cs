namespace ITMO.Dev.ASAP.Github.Common.Exceptions.Entities;

public class EntityNotFoundException : AsapGithubException
{
    private EntityNotFoundException(string? message) : base(message) { }

    public static EntityNotFoundException Create<TKey, TEntity>(TKey key)
        => new EntityNotFoundException($"Entity of type {typeof(TEntity).Name} with id {key} not found");

    public static EntityNotFoundException SubjectCourse(string organizationName)
        => new EntityNotFoundException($"Subject course for {organizationName} was not found");

    public static EntityNotFoundException SubjectCourse()
        => new EntityNotFoundException($"Subject course was not found");

    public static EntityNotFoundException Submission()
        => new EntityNotFoundException("Could not find submission");

    public static EntityNotFoundException Assignment()
        => new EntityNotFoundException("Could not find assignment");

    public static EntityNotFoundException User()
        => new EntityNotFoundException("Could not find user");
}