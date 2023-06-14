using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.UserAssociations;

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