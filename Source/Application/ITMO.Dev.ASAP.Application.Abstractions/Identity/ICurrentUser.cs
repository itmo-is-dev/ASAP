using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid Id { get; }

    bool HasAccessToSubject(Subject subject);

    bool HasAccessToSubjectCourse(SubjectCourse subjectCourse);

    IQueryable<Subject> FilterAvailableSubjects(IQueryable<Subject> subjects);
}