using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.DataAccess.Models.Users;

public partial class UserModel : IEntity<Guid>
{
    public UserModel(
        Guid id,
        string firstName,
        string middleName,
        string lastName,
        ICollection<UserAssociationModel> associations) : this(id)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Associations = associations;
    }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public virtual ICollection<UserAssociationModel> Associations { get; private init; }

    public virtual ICollection<MentorModel> Mentors { get; private init; }

    public virtual ICollection<StudentModel> Students { get; private init; }
}