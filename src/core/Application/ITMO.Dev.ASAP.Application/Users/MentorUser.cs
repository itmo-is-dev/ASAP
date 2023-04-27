using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class MentorUser : ICurrentUser
{
    public MentorUser(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public bool HasAccessToSubject(Subject subject)
    {
        return subject.Courses.SelectMany(x => x.Mentors).Any(x => x.UserId.Equals(Id));
    }

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        return subjectCourse.Mentors.Any(m => m.UserId == Id);
    }

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects)
    {
        return subjects
            .SelectMany(x => x.Courses, (subject, course) => new { subject, course })
            .SelectMany(x => x.course.Mentors, (tuple, mentor) => new { tuple.subject, mentor })
            .Where(x => x.mentor.UserId.Equals(Id))
            .Select(x => x.subject);
    }

    public bool CanUpdateAllDeadlines => false;

    public bool CanCreateUserWithRole(string roleName)
    {
        return false;
    }

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
    {
        return false;
    }
}