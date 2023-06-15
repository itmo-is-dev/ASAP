using ITMO.Dev.ASAP.Domain.ValueObject;

namespace ITMO.Dev.ASAP.Domain.Submissions;

public record struct RatedSubmission(Submission Submission, Points Points);