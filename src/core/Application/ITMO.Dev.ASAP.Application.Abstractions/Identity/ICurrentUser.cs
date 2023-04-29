using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid Id { get; }

    bool HasAccessToSubject(Subject subject);

    bool HasAccessToSubjectCourse(SubjectCourse subjectCourse);

    IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects);

    bool CanUpdateAllDeadlines { get; }

    bool CanCreateUserWithRole(string roleName);

    bool CanChangeUserRole(string currentRoleName, string newRoleName);
}