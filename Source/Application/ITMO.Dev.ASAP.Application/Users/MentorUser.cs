using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class MentorUser : ICurrentUser
{
    public MentorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public IQueryable<SubjectCourse> FilterAvailableSubjectCourses(Subject subject)
    {
        return subject.Courses.Where(s => s.Mentors.Any(m => m.UserId == Id)).AsQueryable();
    }

    public bool HasAccessToSubject(Subject subject)
    {
        return subject.Courses.Any(HasAccessToSubjectCourse);
    }

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        return subjectCourse.Mentors.Any(m => m.UserId == Id);
    }
}