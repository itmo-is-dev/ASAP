﻿using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.SubjectCourses.Queries;

internal static class GetSubjectCoursePoints
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(SubjectCoursePointsDto Points);
}