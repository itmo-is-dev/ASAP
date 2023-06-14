using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.Models;

public record struct EvaluatedSubmission(Submission Submission, double Value);