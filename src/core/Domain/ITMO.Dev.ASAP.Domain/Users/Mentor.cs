using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Users;

public partial class Mentor : IEntity
{
    public Mentor(Guid userId, Guid subjectCourseId)
    {
        UserId = userId;
        SubjectCourseId = subjectCourseId;
    }

    [KeyProperty]
    public Guid UserId { get; protected init; }

    [KeyProperty]
    public Guid SubjectCourseId { get; protected init; }
}