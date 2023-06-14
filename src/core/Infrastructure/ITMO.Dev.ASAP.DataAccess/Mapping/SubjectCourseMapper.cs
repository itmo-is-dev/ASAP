using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Groups;
using ITMO.Dev.ASAP.Domain.Study.Assignments;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Users;

namespace ITMO.Dev.ASAP.DataAccess.Mapping;

public static class SubjectCourseMapper
{
    public static SubjectCourse MapTo(
        SubjectCourseModel model,
        HashSet<StudentGroupInfo> groups,
        HashSet<DeadlinePenalty> penalties,
        HashSet<SubjectCourseAssignment> assignments,
        HashSet<Mentor> mentors)
    {
        return new SubjectCourse(
            model.Id,
            model.SubjectId,
            model.Title,
            model.WorkflowType,
            groups,
            penalties,
            assignments,
            mentors);
    }

    public static SubjectCourseModel MapFrom(SubjectCourse entity)
    {
        return new SubjectCourseModel(entity.Id, entity.SubjectId, entity.Title, entity.WorkflowType);
    }
}