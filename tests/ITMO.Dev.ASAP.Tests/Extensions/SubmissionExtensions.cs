using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class SubmissionExtensions
{
    public static Mentor GetMentor(this Submission submission)
    {
        return submission.GroupAssignment.GetMentor();
    }
}