﻿using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

internal static class CreateSubjectCourse
{
    public record Command
        (Guid SubjectId, string Title, SubmissionStateWorkflowTypeDto WorkflowType) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}