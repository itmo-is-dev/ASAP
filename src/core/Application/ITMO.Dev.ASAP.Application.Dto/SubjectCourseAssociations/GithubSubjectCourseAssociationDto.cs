namespace ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;

public record GithubSubjectCourseAssociationDto(
    Guid SubjectCourseId,
    string GithubOrganizationName,
    string TemplateRepositoryName,
    string MentorTeamName) : SubjectCourseAssociationDto(SubjectCourseId);