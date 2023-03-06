using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Users;

internal class AnonymousUser : ICurrentUser
{
#pragma warning disable CA1065
    public Guid Id => throw new UnauthorizedException("Tried to access anonymous user Id");
#pragma warning restore CA1065

    public IQueryable<SubjectCourse> FilterAvailableSubjectCourses(IQueryable<SubjectCourse> subjectCourses)
    {
        return new List<SubjectCourse>().AsQueryable();
    }

    public IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjectCourses)
    {
        return new List<Subject>().AsQueryable();
    }
}