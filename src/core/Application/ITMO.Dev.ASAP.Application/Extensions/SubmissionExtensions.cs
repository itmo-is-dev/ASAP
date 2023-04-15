using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class SubmissionExtensions
{
    public static Guid GetSubjectCourseId(this Submission submission)
    {
        return submission.GroupAssignment.Assignment.SubjectCourse.Id;
    }

    public static Guid GetGroupId(this Submission submission)
    {
        return submission.GroupAssignment.GroupId;
    }
}