using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
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
            associations.ToArray());
    }

    public static SubjectCourseDto ToDto(this SubjectCourse subjectCourse)
        => subjectCourse.ToDto(Enumerable.Empty<SubjectCourseAssociationDto>());

    public static SubjectCourseAssociationDto ToAssociationDto(this GithubSubjectCourseDto course)
    {
        return new GithubSubjectCourseAssociationDto(
            course.Id,
            course.OrganizationName,
            course.TemplateRepositoryName,
            course.MentorTeamName);
    }

    public static SubjectCourseAssociationDto ToAssociationDto(this GoogleSubjectCourseDto course)
    {
        return new GoogleSubjectCourseAssociationDto(course.Id, course.SpreadsheetId);
    }
}