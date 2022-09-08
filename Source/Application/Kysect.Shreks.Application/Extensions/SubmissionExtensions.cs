﻿using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Handlers.Extensions;

public static class SubmissionExtensions
{
    public static Guid GetCourseId(this Submission submission)
        => submission.GroupAssignment.Assignment.SubjectCourse.Id;

    public static Guid GetGroupId(this Submission submission)
        => submission.GroupAssignment.GroupId;
}