using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Github.Domain.SubjectCourses;

public partial class GithubSubjectCourse : IEntity<Guid>
{
    public GithubSubjectCourse(
        Guid id,
        string organizationName,
        string templateRepositoryName,
        string mentorTeamName)
        : this(id)
    {
        OrganizationName = organizationName;
        TemplateRepositoryName = templateRepositoryName;
        MentorTeamName = mentorTeamName;
    }

    public string OrganizationName { get; protected set; }

    public string TemplateRepositoryName { get; protected set; }

    public string MentorTeamName { get; set; }
}