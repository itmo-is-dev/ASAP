using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Application.Factories;

public static class SubmissionRateDtoFactory
{
    public static SubmissionRateDto CreateFromSubmission(
        Submission submission,
        SubjectCourse subjectCourse,
        Assignment assignment,
        GroupAssignment groupAssignment)
    {
        double? rating = null;

        if (submission.Rating is not null)
            rating = submission.Rating * 100;

        DeadlinePenalty? penalty = subjectCourse.DeadlinePolicy
            .FindEffectiveDeadlinePenalty(groupAssignment.Deadline, submission.SubmissionDateOnly);

        Points rawPoints = assignment.RatedWith(submission.Rating);
        Points points = penalty?.Apply(rawPoints) ?? rawPoints;
        Points penaltyPoints = rawPoints - points;

        points += submission.ExtraPoints ?? Points.None;

        var dto = new SubmissionRateDto(
            submission.Id,
            submission.Code,
            submission.State.Kind.ToString(),
            submission.SubmissionDate.Value,
            rating,
            rawPoints.Value,
            assignment.MaxPoints.Value,
            submission.ExtraPoints?.Value,
            penaltyPoints.Value,
            points.Value);

        return dto;
    }
}