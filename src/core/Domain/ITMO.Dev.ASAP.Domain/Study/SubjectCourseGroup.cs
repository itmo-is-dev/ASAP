using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study;

public partial class SubjectCourseGroup : IEntity
{
    public SubjectCourseGroup(Guid subjectCourseId, Guid studentGroupId)
    {
        SubjectCourseId = subjectCourseId;
        StudentGroupId = studentGroupId;
    }

    [KeyProperty]
    public Guid SubjectCourseId { get; }

    [KeyProperty]
    public Guid StudentGroupId { get; }
}