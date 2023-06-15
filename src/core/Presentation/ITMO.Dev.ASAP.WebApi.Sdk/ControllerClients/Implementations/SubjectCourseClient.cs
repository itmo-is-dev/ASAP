using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Github;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class SubjectCourseClient : ISubjectCourseClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public SubjectCourseClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<GithubSubjectCourseDto> CreateForGithubAsync(
        CreateGithubSubjectCourseRequest request,
        CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/SubjectCourse/github")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<GithubSubjectCourseDto>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}");
        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> UpdateAsync(
        Guid id,
        UpdateSubjectCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/SubjectCourse/{id}")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentDto>> GetStudentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}/students");
        return await _handler.SendAsync<IReadOnlyCollection<StudentDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AssignmentDto>> GetAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}/assignments");
        return await _handler.SendAsync<IReadOnlyCollection<AssignmentDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SubjectCourseGroupDto>> GetGroupsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}/groups");
        return await _handler.SendAsync<IReadOnlyCollection<SubjectCourseGroupDto>>(message, cancellationToken);
    }

    public async Task<SubmissionsQueueDto> GetStudyGroupQueueAsync(
        Guid subjectCourseId,
        Guid studyGroupId,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/SubjectCourse/{subjectCourseId}/groups/{studyGroupId}/queue";
        using var message = new HttpRequestMessage(HttpMethod.Get, uri);

        return await _handler.SendAsync<SubmissionsQueueDto>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> AddGithubAssociationAsync(
        Guid id,
        AddSubjectCourseGithubAssociationRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/SubjectCourse/{id}/association/github")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task AddFractionDeadlinePolicyAsync(
        Guid id,
        AddFractionPolicyRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/SubjectCourse/{id}/deadline/fraction")
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task UpdateMentorsTeamNameAsync(
        Guid id,
        UpdateMentorsTeamNameRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/SubjectCourse/{id}/github/mentor-team")
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task ForceSyncSubjectCoursePointsAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/SubjectCourse/{subjectCourseId}/points/force-sync";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }
}