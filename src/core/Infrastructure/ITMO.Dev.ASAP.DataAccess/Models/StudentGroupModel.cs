using ITMO.Dev.ASAP.DataAccess.Models.Users;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models;

public partial class StudentGroupModel : IEntity<Guid>
{
    public StudentGroupModel(Guid id, string name) : this(id)
    {
        Name = name;
    }

    public string Name { get; set; }

    public virtual ICollection<StudentModel> Students { get; init; }

    public virtual ICollection<SubjectCourseGroupModel> SubjectCourseGroups { get; init; }
}