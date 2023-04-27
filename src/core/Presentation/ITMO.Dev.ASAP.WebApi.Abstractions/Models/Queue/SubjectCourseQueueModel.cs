using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.Queue;

public record SubjectCourseQueueModel(Guid SubjectCourseId, Guid StudyGroupId, SubmissionsQueueDto Queue);