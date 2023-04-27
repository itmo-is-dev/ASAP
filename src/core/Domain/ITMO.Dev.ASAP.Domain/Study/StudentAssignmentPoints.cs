using ITMO.Dev.ASAP.Domain.Users;
using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Study;

public record struct StudentAssignmentPoints(
    Student Student,
    Assignment Assignment,
    bool IsBanned,
    Points Points,
    DateOnly SubmissionDate);