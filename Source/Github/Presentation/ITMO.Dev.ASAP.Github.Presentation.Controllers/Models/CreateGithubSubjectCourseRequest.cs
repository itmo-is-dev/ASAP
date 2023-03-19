using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.Github.Presentation.Controllers.Models;

public record CreateGithubSubjectCourseRequest(
    Guid SubjectId,
    string Name,
    SubmissionStateWorkflowTypeDto WorkflowType,
    string GithubOrganizationName,
    string TemplateRepositoryName,
    string MentorTeamName);