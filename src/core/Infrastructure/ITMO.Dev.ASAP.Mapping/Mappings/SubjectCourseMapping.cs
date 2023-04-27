using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourse;

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