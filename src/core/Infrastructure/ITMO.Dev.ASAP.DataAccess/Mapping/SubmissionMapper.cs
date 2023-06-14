using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Models;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study.GroupAssignments;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Submissions.States;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class SubmissionMapper
{
    public static Submission MapTo(
        SubmissionModel model,
        GroupAssignment groupAssignment)
    {
        ISubmissionState state = model.State switch
        {
            SubmissionStateKind.Active => new ActiveSubmissionState(),
            SubmissionStateKind.Inactive => new InactiveSubmissionState(),
            SubmissionStateKind.Deleted => new DeletedSubmissionState(),
            SubmissionStateKind.Completed => new CompletedSubmissionState(),
            SubmissionStateKind.Reviewed => new ReviewedSubmissionState(),
            SubmissionStateKind.Banned => new BannedSubmissionState(),

            _ => throw new ArgumentOutOfRangeException(
                nameof(model.State),
                model.State,
                "Invalid submission state value"),
        };

        Student student = StudentMapper.MapTo(model.Student);

        return new Submission(
            model.Id,
            model.Code,
            student,
            model.SubmissionDate,
            model.Payload,
            groupAssignment,
            state);
    }

    public static SubmissionModel MapFrom(Submission entity)
    {
        return new SubmissionModel(
            entity.Id,
            entity.Code,
            entity.Payload,
            entity.Rating?.Value,
            entity.ExtraPoints?.Value,
            entity.SubmissionDate,
            entity.Student.UserId,
            entity.GroupAssignment.Id.StudentGroupId,
            entity.GroupAssignment.Id.AssignmentId,
            entity.State.Kind);
    }
}