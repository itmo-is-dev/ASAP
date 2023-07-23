using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Study;

public record struct StudentAssignmentPoints(
    Student Student,
    Assignment Assignment,
    bool IsBanned,
    Points Points,
    Points Penalty,
    DateOnly SubmissionDate);