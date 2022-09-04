﻿using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Queries;

public static class GetSubjectCourses
{
    public record Query() : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectCourseDto> Subjects);
}