using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class GroupAssignmentExtensions
{
    public static Mentor GetMentor(this GroupAssignment assignment)
    {
        return assignment.Assignment.SubjectCourse.Mentors.First();
    }
}