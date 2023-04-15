using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;

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

    public static IQueryable<bool> IsMentor(this IQueryable<SubjectCourse> queryable, Guid mentorId)
        => queryable.SelectMany(x => x.Mentors).Select(x => x.UserId.Equals(mentorId));
}