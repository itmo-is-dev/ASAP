using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.Application.Mapping;

public static class SubjectCourseMapping
{
    public static GithubSubjectCourseDto ToDto(this GithubSubjectCourse subjectCourse)
    {
        return new GithubSubjectCourseDto(
            subjectCourse.Id,
            subjectCourse.OrganizationName,
            subjectCourse.TemplateRepositoryName,
            subjectCourse.MentorTeamName);
    }
}