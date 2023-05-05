using ITMO.Dev.ASAP.Domain.Study;
using Student = ITMO.Dev.ASAP.Domain.Users.Student;

namespace ITMO.Dev.ASAP.Application.Specifications;

public static class SubjectCourseSpecifications
{
    public static IQueryable<Student> GetAllStudents(this IQueryable<SubjectCourse> queryable)
    {
        return queryable
            .SelectMany(x => x.Groups)
            .Select(x => x.StudentGroup)
            .SelectMany(x => x.Students);
    }
}