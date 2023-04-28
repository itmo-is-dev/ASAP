namespace ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;

public interface ISubjectCourseUpdateService
{
    void UpdatePoints(Guid subjectCourseId);
}