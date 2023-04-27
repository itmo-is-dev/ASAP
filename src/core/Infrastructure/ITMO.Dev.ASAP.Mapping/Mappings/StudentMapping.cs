using ITMO.Dev.ASAP.Application.Dto.Users;
using IsuUserAssociation = ITMO.Dev.ASAP.Domain.UserAssociations.IsuUserAssociation;
using Student = ITMO.Dev.ASAP.Domain.Users.Student;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class StudentMapping
{
    public static StudentDto ToDto(this Student student, string? githubUsername)
    {
        IsuUserAssociation? isuAssociation = student.User.FindAssociation<IsuUserAssociation>();

        return new StudentDto(
            student.User.ToDto(),
            student.Group?.Name ?? string.Empty,
            isuAssociation?.UniversityId,
            githubUsername);
    }
}