using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study;

public partial class SubjectCourseGroup : IEntity
{
    public SubjectCourseGroup(SubjectCourse subjectCourse, StudentGroup studentGroup)
        : this(subjectCourseId: subjectCourse.Id, studentGroupId: studentGroup.Id)
    {
        ArgumentNullException.ThrowIfNull(subjectCourse);
        ArgumentNullException.ThrowIfNull(studentGroup);

        SubjectCourse = subjectCourse;
        StudentGroup = studentGroup;
    }

    [KeyProperty]
    public virtual SubjectCourse SubjectCourse { get; protected init; }

    [KeyProperty]
    public virtual StudentGroup StudentGroup { get; protected init; }

    public override string ToString()
    {
        return StudentGroup.ToString();
    }
}