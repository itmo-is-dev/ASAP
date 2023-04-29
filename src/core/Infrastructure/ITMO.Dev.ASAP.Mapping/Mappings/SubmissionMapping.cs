using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubmissionMapping
{
    public static SubmissionDto ToDto(this Submission submission)
    {
        return new SubmissionDto(
            submission.Id,
            submission.Code,
            submission.SubmissionDate.AsDateTime(),
            submission.Student.UserId,
            submission.GroupAssignment.AssignmentId,
            submission.Payload,
            submission.ExtraPoints.AsDto(),
            submission.Points.AsDto(),
            submission.GroupAssignment.Assignment.ShortName,
            submission.State.Kind.AsDto());
    }

    public static QueueSubmissionDto ToQueueDto(this Submission submission, string? studentGithubUsername)
    {
        return new QueueSubmissionDto(submission.Student.ToDto(studentGithubUsername), submission.ToDto());
    }
}