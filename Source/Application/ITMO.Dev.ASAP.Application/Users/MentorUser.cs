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

    public IQueryable<SubjectCourse> FilterAvailableSubjectCourses(IQueryable<SubjectCourse> subjectCourses)
    {
        return subjectCourses
            .Where(s => s.Mentors.Any(m => m.UserId == Id));
    }

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects)
    {
        return subjects
            .Where(s => s.Courses.SelectMany(c => c.Mentors).Any(m => m.UserId == Id));
    }
}