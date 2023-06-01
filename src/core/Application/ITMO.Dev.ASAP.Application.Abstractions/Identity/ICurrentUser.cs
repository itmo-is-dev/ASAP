using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid Id { get; }

    bool CanManageStudents { get; }

    bool HasAccessToSubjectCourse(SubjectCourse subjectCourse);

    public SubjectQuery.Builder FilterAvailableSubjects(SubjectQuery.Builder queryBuilder);

    bool CanUpdateAllDeadlines { get; }

    bool CanCreateUserWithRole(string roleName);

    bool CanChangeUserRole(string currentRoleName, string newRoleName);
}