namespace ITMO.Dev.ASAP.Application.Dto.Study;

public class StudyGroupDto
{
    public StudyGroupDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
}