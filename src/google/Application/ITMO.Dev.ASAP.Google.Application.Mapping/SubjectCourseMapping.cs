using ITMO.Dev.ASAP.Google.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Google.Domain.SubjectCourses;

namespace ITMO.Dev.ASAP.Google.Application.Mapping;

public static class SubjectCourseMapping
{
    public static GoogleSubjectCourseDto ToDto(this GoogleSubjectCourse course)
    {
        return new GoogleSubjectCourseDto(course.Id, course.SpreadsheetId);
    }
}