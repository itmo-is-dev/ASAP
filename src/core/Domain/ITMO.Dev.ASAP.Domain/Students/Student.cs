using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Students.Events;
using ITMO.Dev.ASAP.Domain.Users;
using RichEntity.Annotations;
using System.Text;
using StudentGroup = ITMO.Dev.ASAP.Domain.Groups.StudentGroup;

namespace ITMO.Dev.ASAP.Domain.Students;

public partial class Student : IEntity
{
    public Student(User user, StudentGroupInfo? group)
        : this(user.Id)
    {
        User = user;
        Group = group;
    }

    [KeyProperty]
    public User User { get; }

    public StudentGroupInfo? Group { get; private set; }

    public void DismissFromStudyGroup(StudentGroup group)
    {
        if (Group is null)
            throw new DomainInvalidOperationException("Student is not in any group");

        if (group.Id.Equals(Group.Id) is false)
            throw new DomainInvalidOperationException("Trying to dismiss student from invalid group");

        group.RemoveStudent(this);
        Group = null;
    }

    public (Student Student, IStudentEvent Event) TransferToAnotherGroup(StudentGroup? from, StudentGroup to)
    {
        if (Nullable.Equals(Group?.Id, from?.Id) is false)
            throw new DomainInvalidOperationException("Trying to transfer student from invalid group");

        Student student = to.AddStudent(User);
        from?.RemoveStudent(this);

        Group = to.Info;

        var evt = new StudentTransferredEvent(this);

        return (student, evt);
    }

    public override string ToString()
    {
        var builder = new StringBuilder($"{User.FirstName} {User.LastName}");

        if (Group is not null)
            builder.Append($" from Group = {Group.Name}({Group.Id}), UserId = ({UserId})");

        return builder.ToString();
    }
}