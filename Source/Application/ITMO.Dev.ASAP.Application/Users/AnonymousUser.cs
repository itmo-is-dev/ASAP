using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class AnonymousUser : ICurrentUser
{
#pragma warning disable CA1065
    public Guid Id => throw new UnauthorizedException("Tried to access anonymous user Id");
#pragma warning restore CA1065

    public IQueryable<SubjectCourse> FilterAvailableSubjectCourses(Subject subject)
    {
        throw UserHasNotAccessException.AnonymousUserHasNotAccess();
    }

    public bool HasAccessToSubject(Subject subject)
    {
        throw UserHasNotAccessException.AnonymousUserHasNotAccess();
    }

    public bool HasAccessToSubjectCourse(SubjectCourse subjectCourse)
    {
        throw UserHasNotAccessException.AnonymousUserHasNotAccess();
    }
}