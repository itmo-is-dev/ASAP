using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Models;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.SubjectCourses;

public interface ISubjectCourse
{
    IObservable<SubjectCourseDto> SubjectCourse { get; }

    IObservable<SubjectCourseSelection> Selection { get; }

    ValueTask SelectSubjectCourseAsync(Guid subjectCourseId);

    void SelectTab(SubjectCourseSelection selection);

    ValueTask CreateAssignmentAsync(
        string title,
        string shortName,
        int order,
        double minPoints,
        double maxPoints,
        CancellationToken cancellationToken);
}