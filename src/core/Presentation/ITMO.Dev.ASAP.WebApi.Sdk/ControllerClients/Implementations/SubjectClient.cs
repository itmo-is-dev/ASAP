using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Subjects;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class SubjectClient : ISubjectClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public SubjectClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<SubjectDto> CreateAsync(
        CreateSubjectRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/Subject")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SubjectDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/Subject");
        return await _handler.SendAsync<IReadOnlyCollection<SubjectDto>>(message, cancellationToken);
    }

    public async Task<SubjectDto> UpdateAsync(
        UpdateSubjectRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, "api/Subject")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SubjectCourseDto>> GetCoursesAsync(
        Guid subjectId,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/Subject/{subjectId}/courses");
        return await _handler.SendAsync<IReadOnlyCollection<SubjectCourseDto>>(message, cancellationToken);
    }
}