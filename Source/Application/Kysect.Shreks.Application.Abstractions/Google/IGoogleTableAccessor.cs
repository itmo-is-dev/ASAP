﻿using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Abstractions.Google;

public record AssignmentPoints(Assignment Assignment, DateOnly Date, double Points);

public record StudentPoints(Student Student, IReadOnlyCollection<AssignmentPoints> Points);

public record struct Points(IReadOnlyCollection<Assignment> Assignments, IReadOnlyCollection<StudentPoints> StudentsPoints);

public record struct Queue(IReadOnlyCollection<Submission> Submissions);

public interface IGoogleTableAccessor
{
    Task UpdateQueueAsync(Queue queue, CancellationToken token = default);
    Task UpdatePointsAsync(Points points, CancellationToken token = default);
}