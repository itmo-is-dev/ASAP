using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubjectCourseMapping
{
    public static SubjectCourseDto ToDto(
        this SubjectCourse subjectCourse,
        IEnumerable<SubjectCourseAssociationDto> associations)
    {
        return new SubjectCourseDto(
            subjectCourse.Id,
            subjectCourse.Subject.Id,
            subjectCourse.Title,
            subjectCourse.WorkflowType?.AsDto(),
            subjectCourse.Associations.Select(x => x.ToDto()).Concat(associations).ToArray());
    }

    public static SubjectCourseDto ToDto(this SubjectCourse subjectCourse)
        => subjectCourse.ToDto(Enumerable.Empty<SubjectCourseAssociationDto>());
}