namespace ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;

public record GithubSubjectCourseDto(
    Guid Id,
    string OrganizationName,
    string TemplateRepositoryName,
    string MentorTeamName);