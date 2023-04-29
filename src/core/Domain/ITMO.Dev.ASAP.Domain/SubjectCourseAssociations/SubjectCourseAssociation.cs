using ITMO.Dev.ASAP.Domain.Study;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.SubjectCourseAssociations;

public abstract partial class SubjectCourseAssociation : IEntity<Guid>
{
    protected SubjectCourseAssociation(Guid id, SubjectCourse subjectCourse) : this(id)
    {
        SubjectCourse = subjectCourse;
    }

    public virtual SubjectCourse SubjectCourse { get; protected init; }
}