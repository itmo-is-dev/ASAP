using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Groups;

public partial class StudentGroupInfo : IEntity<Guid>
{
    public StudentGroupInfo(Guid id, string name) : this(id)
    {
        Name = name;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}