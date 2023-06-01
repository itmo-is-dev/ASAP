using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Study;

public partial class Subject : IEntity<Guid>
{
    public Subject(Guid id, string title) : this(id)
    {
        ArgumentNullException.ThrowIfNull(title);

        Title = title;
    }

    public string Title { get; set; }

    public SubjectCourse AddCourse(SubjectCourse.SubjectCourseBuilder builder)
    {
        return builder.Build(this);
    }
}