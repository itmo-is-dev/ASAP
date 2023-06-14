using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Users;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Groups;

public partial class StudentGroup : IEntity<Guid>
{
    private readonly HashSet<Guid> _studentIds;

    public StudentGroup(Guid id, string name, HashSet<Guid> studentIds) : this(id)
    {
        Name = name;
        _studentIds = studentIds;
    }

    public string Name { get; set; }

    public IReadOnlyCollection<Guid> Student => _studentIds;

    public StudentGroupInfo Info => new StudentGroupInfo(Id, Name);

    public override string ToString()
    {
        return Name;
    }

    public Student AddStudent(User user)
    {
        if (_studentIds.Contains(user.Id))
            throw new DomainInvalidOperationException($"Student {user} already a member of group {this}");

        _studentIds.Add(user.Id);

        return new Student(user, Info);
    }

    public void RemoveStudent(Student student)
    {
        if (_studentIds.Remove(student.UserId) is false)
            throw new DomainInvalidOperationException($"Removing student {student} from group {this} failed");
    }
}