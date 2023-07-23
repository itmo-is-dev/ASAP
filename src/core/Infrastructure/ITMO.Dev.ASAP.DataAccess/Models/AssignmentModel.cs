using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models;

public partial class AssignmentModel : IEntity<Guid>
{
    public AssignmentModel(
        Guid id,
        Guid subjectCourseId,
        string title,
        string shortName,
        int order,
        double minPoints,
        double maxPoints) : this(id)
    {
        SubjectCourseId = subjectCourseId;
        Title = title;
        ShortName = shortName;
        Order = order;
        MinPoints = minPoints;
        MaxPoints = maxPoints;
    }

    public Guid SubjectCourseId { get; set; }

    public virtual SubjectCourseModel SubjectCourse { get; set; }

    public string Title { get; set; }

    public string ShortName { get; set; }

    public int Order { get; set; }

    public double MinPoints { get; set; }

    public double MaxPoints { get; set; }

    public virtual ICollection<GroupAssignmentModel> GroupAssignments { get; set; }
}