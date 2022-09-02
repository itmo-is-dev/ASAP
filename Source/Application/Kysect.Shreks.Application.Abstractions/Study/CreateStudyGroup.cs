﻿using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study;

public static class CreateStudyGroup
{
    public record Command(string Name) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}