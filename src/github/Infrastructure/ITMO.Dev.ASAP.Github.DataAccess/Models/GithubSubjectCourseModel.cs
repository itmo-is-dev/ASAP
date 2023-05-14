using ITMO.Dev.ASAP.Github.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Github.DataAccess.Models;

public record GithubSubjectCourseModel(
    Guid Id,
    string OrganizationName,
    string TemplateRepositoryName,
    string MentorTeamName)
{
    public GithubSubjectCourse ToEntity()
    {
        return new GithubSubjectCourse(Id, OrganizationName, TemplateRepositoryName, MentorTeamName);
    }
}