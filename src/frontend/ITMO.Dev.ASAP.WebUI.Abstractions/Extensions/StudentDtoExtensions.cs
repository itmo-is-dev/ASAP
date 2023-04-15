using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Extensions;

public static class StudentDtoExtensions
{
    public static string FullName(this StudentDto student)
        => student.User.FullName();

    public static string DisplayString(this StudentDto student)
        => $"{student.UniversityId} {student.FullName()}";
}