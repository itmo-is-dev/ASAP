using ITMO.Dev.ASAP.Domain.Study;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Users;

public partial class Mentor : IEntity
{
    public Mentor(User user, SubjectCourse course)
        : this(userId: user.Id, courseId: course.Id)
    {
        User = user;
        Course = course;
    }

    [KeyProperty]
    public virtual User User { get; protected init; }

    [KeyProperty]
    public virtual SubjectCourse Course { get; protected init; }
}