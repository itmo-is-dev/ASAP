using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Google.DataAccess.Models;

public record GoogleSubjectCourseModel(Guid Id, string SpreadsheetId)
{
    public GoogleSubjectCourse ToEntity()
    {
        return new GoogleSubjectCourse(Id, SpreadsheetId);
    }
}