using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models.Users;

public partial class StudentModel : IEntity
{
    public StudentModel(Guid userId, Guid? studentGroupId) : this(userId)
    {
        StudentGroupId = studentGroupId;
    }

    [KeyProperty]
    public virtual UserModel User { get; set; }

    public Guid? StudentGroupId { get; set; }

    public virtual StudentGroupModel? StudentGroup { get; set; }

    public virtual ICollection<SubmissionModel> Submissions { get; init; }
}