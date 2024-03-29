using ITMO.Dev.ASAP.Application.Dto.Tables;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Queries;

public static class GetSubmissionsQueue
{
    public record Query(Guid SubjectCourseId, Guid GroupId) : IRequest<Response>;

    public record Response(SubmissionsQueueDto SubmissionsQueue);
}