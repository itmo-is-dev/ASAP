using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubmissionMapping
{
    public static SubmissionDto ToDto(this Submission submission, Points points)
    {
        return new SubmissionDto(
            submission.Id,
            submission.Code,
            submission.SubmissionDate.AsDateTime(),
            submission.Student.UserId,
            submission.GroupAssignment.Id.AssignmentId,
            submission.Payload,
            submission.ExtraPoints.AsDto(),
            points.AsDto(),
            submission.GroupAssignment.Assignment.ShortName,
            submission.State.Kind.AsDto());
    }

    public static QueueSubmissionDto ToQueueDto(
        this Submission submission,
        string? studentGithubUsername,
        Points points)
    {
        return new QueueSubmissionDto(
            submission.Student.ToDto(studentGithubUsername),
            submission.ToDto(points));
    }
}