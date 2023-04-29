using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class GroupAssignmentExtensions
{
    public static Mentor GetMentor(this GroupAssignment assignment)
    {
        return assignment.Assignment.SubjectCourse.Mentors.First();
    }
}