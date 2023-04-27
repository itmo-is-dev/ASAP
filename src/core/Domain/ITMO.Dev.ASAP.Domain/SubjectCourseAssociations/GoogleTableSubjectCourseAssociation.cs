using ITMO.Dev.ASAP.Domain.Study;

namespace ITMO.Dev.ASAP.Domain.SubjectCourseAssociations;

public partial class GoogleTableSubjectCourseAssociation : SubjectCourseAssociation
{
    public GoogleTableSubjectCourseAssociation(Guid id, SubjectCourse subjectCourse, string spreadsheetId)
        : base(id, subjectCourse)
    {
        SpreadsheetId = spreadsheetId;
    }

    public string SpreadsheetId { get; protected set; }
}