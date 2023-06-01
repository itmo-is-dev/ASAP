using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class AnonymousUser : ICurrentUser
{
#pragma warning disable CA1065
    public Guid Id => throw new UnauthorizedException("Tried to access anonymous user Id");
#pragma warning restore CA1065

    public bool CanUpdateAllDeadlines => false;

    public bool CanManageStudents => false;

    public SubjectQuery.Builder FilterAvailableSubjects(SubjectQuery.Builder queryBuilder)
        => throw UserHasNotAccessException.AnonymousUserHasNotAccess();

    public bool CanCreateUserWithRole(string roleName)
        => throw UserHasNotAccessException.AnonymousUserHasNotAccess();

    public bool CanChangeUserRole(string currentRoleName, string newRoleName)
        => throw UserHasNotAccessException.AnonymousUserHasNotAccess();

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
        => throw UserHasNotAccessException.AnonymousUserHasNotAccess();
}