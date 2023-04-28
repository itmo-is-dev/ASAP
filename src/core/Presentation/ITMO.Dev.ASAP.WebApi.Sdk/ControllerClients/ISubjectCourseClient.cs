using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Github;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface ISubjectCourseClient
{
    Task<GithubSubjectCourseDto> CreateForGithubAsync(
        CreateGithubSubjectCourseRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectCourseDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> UpdateAsync(
        Guid id,
        UpdateSubjectCourseRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudentDto>> GetStudentsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AssignmentDto>> GetAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectCourseGroupDto>> GetGroupsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SubmissionsQueueDto> GetStudyGroupQueueAsync(
        Guid subjectCourseId,
        Guid studyGroupId,
        CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> AddGithubAssociationAsync(
        Guid id,
        AddSubjectCourseGithubAssociationRequest request,
        CancellationToken cancellationToken = default);

    Task AddFractionDeadlinePolicyAsync(
        Guid id,
        AddFractionPolicyRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateMentorsTeamNameAsync(
        Guid id,
        UpdateMentorsTeamNameRequest request,
        CancellationToken cancellationToken = default);

    Task ForceSyncSubjectCoursePointsAsync(Guid subjectCourseId, CancellationToken cancellationToken = default);
}